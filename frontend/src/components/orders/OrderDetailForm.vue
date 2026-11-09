<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import DatePicker from 'primevue/datepicker'
import Button from 'primevue/button'

import { goodsService } from '../../services/goodsService'
import type { OrderDetail, CreateDOrderRequest } from '../../types/order'
import { dateToIsoDateString, isoDateStringToDate, isSameOrAfterDate } from '@/utils/date'
import { formatNumber } from '@/utils/number'

const props = withDefaults(
  defineProps<{
    orderNo: number
    cusId: string
    cusName: string
    orderDate: string
    mode?: 'create' | 'edit'
    detail?: OrderDetail | null
    existingGoodsIds?: string[]
  }>(),
  {
    mode: 'create',
    detail: null,
    existingGoodsIds: () => [],
  },
)

const emit = defineEmits<{
  submit: [data: CreateDOrderRequest]
  cancel: []
}>()

const goodsId = ref('')
const goodsName = ref('')
const costUnit = ref<number | null>(null)

const ordDate = ref<Date | null>(new Date())
const finDate = ref<Date | null>(null)
const amount = ref<number | null>(null)

const goodsValid = ref(false)
const goodsChecking = ref(false)
const goodsError = ref('')

const validationRequest = ref(0)

const orderDateValue = computed(() => isoDateStringToDate(props.orderDate))

// ตรวจสอบจำนวนที่สั่ง
const amountError = computed(() => {
  const value = amount.value

  if (value === null || !Number.isFinite(value)) {
    return 'กรุณาระบุจำนวนที่สั่ง'
  }

  if (value <= 0) {
    return 'จำนวนที่สั่งต้องมากกว่า 0'
  }

  if (value > 9_999_999.99) {
    return 'จำนวนที่สั่งต้องไม่เกิน 9,999,999.99'
  }

  if (Math.abs(value * 100 - Math.round(value * 100)) > 1e-8) {
    return 'จำนวนที่สั่งต้องมีทศนิยมไม่เกิน 2 ตำแหน่ง'
  }

  return ''
})

// ตรวจสอบวันที่
const dateError = computed(() => {
  const orderDate = orderDateValue.value

  if (!(orderDate instanceof Date) || Number.isNaN(orderDate.getTime())) {
    return 'วันที่สั่ง Order ไม่ถูกต้อง'
  }

  if (!(ordDate.value instanceof Date) || Number.isNaN(ordDate.value.getTime())) {
    return 'กรุณาระบุวันที่กำหนดส่ง'
  }

  if (!isSameOrAfterDate(ordDate.value, orderDate)) {
    return 'วันที่กำหนดส่งต้องไม่ก่อนวันที่สั่ง Order'
  }

  if (finDate.value !== null) {
    if (!(finDate.value instanceof Date) || Number.isNaN(finDate.value.getTime())) {
      return 'วันที่ส่งสินค้าจริงไม่ถูกต้อง'
    }

    if (!isSameOrAfterDate(finDate.value, ordDate.value)) {
      return 'วันที่ส่งจริงต้องไม่ก่อนวันที่กำหนดส่ง'
    }
  }

  return ''
})

// คำนวณราคารวม
const totalPrice = computed(() => {
  if (
    amount.value === null ||
    costUnit.value === null ||
    !Number.isFinite(amount.value) ||
    !Number.isFinite(costUnit.value)
  ) {
    return 0
  }

  return amount.value * costUnit.value
})

// ตรวจสอบความพร้อมก่อน Submit
const canSubmit = computed(
  () =>
    goodsValid.value &&
    !goodsChecking.value &&
    !goodsError.value &&
    !amountError.value &&
    !dateError.value,
)

// Normalize Goods ID ก่อนเปรียบเทียบ
function normalizeGoodsId(value: string): string {
  return value.trim().toUpperCase()
}

// ตรวจสอบสินค้าซ้ำภายใน Order
function isDuplicateGoodsId(value: string): boolean {
  const normalizedId = normalizeGoodsId(value)

  const currentId =
    props.mode === 'edit' && props.detail ? normalizeGoodsId(props.detail.goodsId) : undefined

  if (normalizedId === currentId) return false

  return props.existingGoodsIds.some((id) => normalizeGoodsId(id) === normalizedId)
}

function handleGoodsIdChange() {
  validationRequest.value++

  goodsValid.value = false
  goodsChecking.value = false

  goodsName.value = ''
  costUnit.value = null
  goodsError.value = ''
}

async function validateGoods() {
  const id = goodsId.value.trim()
  const requestId = ++validationRequest.value

  goodsValid.value = false
  goodsName.value = ''
  costUnit.value = null
  goodsError.value = ''

  if (!id) {
    goodsChecking.value = false
    goodsError.value = 'กรุณาระบุรหัสสินค้า'
    return
  }

  if (id.length !== 10) {
    goodsChecking.value = false
    goodsError.value = 'รหัสสินค้าต้องมีความยาว 10 ตัวอักษรพอดี'
    return
  }

  if (isDuplicateGoodsId(id)) {
    goodsChecking.value = false
    goodsError.value = 'สินค้านี้มีอยู่ใน Order แล้ว ไม่สามารถเพิ่มซ้ำได้'
    return
  }

  goodsChecking.value = true

  try {
    const goods = await goodsService.getById(id)

    if (requestId !== validationRequest.value) return

    goodsName.value = goods.goodsName
    costUnit.value = goods.costUnit
    goodsValid.value = true
  } catch {
    if (requestId !== validationRequest.value) return

    goodsError.value = 'ไม่พบรหัสสินค้านี้ในระบบ'
  } finally {
    if (requestId === validationRequest.value) {
      goodsChecking.value = false
    }
  }
}

function resetForm() {
  validationRequest.value++

  goodsId.value = ''
  goodsName.value = ''
  costUnit.value = null

  ordDate.value = new Date()
  finDate.value = null
  amount.value = null

  goodsValid.value = false
  goodsChecking.value = false
  goodsError.value = ''
}

function handleSubmit() {
  // ตรวจสอบ Goods ID อีกครั้ง
  const id = goodsId.value.trim()

  if (!id) {
    goodsError.value = 'กรุณาระบุรหัสสินค้า'
    goodsValid.value = false
    return
  }

  if (id.length !== 10) {
    goodsError.value = 'รหัสสินค้าต้องมีความยาว 10 ตัวอักษรพอดี'
    goodsValid.value = false
    return
  }

  if (isDuplicateGoodsId(id)) {
    goodsError.value = 'สินค้านี้มีอยู่ใน Order แล้ว ไม่สามารถเพิ่มซ้ำได้'
    goodsValid.value = false
    return
  }

  if (!canSubmit.value) return

  // ตรวจสอบวันที่และจำนวนอีกครั้งก่อนส่ง
  const currentAmount = amount.value
  const currentOrdDate = ordDate.value

  if (currentAmount === null || currentOrdDate === null) return

  emit('submit', {
    orderNo: props.orderNo,
    goodsId: id,
    ordDate: dateToIsoDateString(currentOrdDate),
    finDate: finDate.value ? dateToIsoDateString(finDate.value) : null,
    amount: currentAmount,
  })
}

watch(
  () => [props.mode, props.detail] as const,
  ([mode, detail]) => {
    validationRequest.value++

    if (mode === 'edit' && detail) {
      goodsId.value = detail.goodsId
      ordDate.value = isoDateStringToDate(detail.ordDate)

      finDate.value =
        detail.finDate?.substring(0, 10) === '2000-01-01'
          ? null
          : detail.finDate
            ? isoDateStringToDate(detail.finDate)
            : null

      amount.value = detail.amount

      goodsName.value = ''
      costUnit.value = null
      goodsValid.value = false
      goodsChecking.value = false
      goodsError.value = ''

      validateGoods()
    } else {
      resetForm()
    }
  },
  { immediate: true },
)

defineExpose({ resetForm })
</script>

<template>
  <div class="order-detail-form">
    <h2 class="form-heading">
      {{ mode === 'edit' ? 'แก้ไขรายการสินค้า' : 'เพิ่มรายการสินค้า' }}
    </h2>

    <div class="form-section">
      <h3>ข้อมูล Order</h3>

      <div class="form-grid">
        <div class="form-field">
          <label>Order No.</label>
          <InputText :model-value="String(orderNo)" disabled />
        </div>

        <div class="form-field">
          <label>รหัสลูกค้า</label>
          <InputText :model-value="cusId" disabled />
        </div>

        <div class="form-field">
          <label>ชื่อลูกค้า</label>
          <InputText :model-value="cusName" disabled />
        </div>

        <div class="form-field">
          <label>วันที่สั่ง Order</label>
          <DatePicker :model-value="orderDateValue" date-format="dd/mm/yy" disabled show-icon />
        </div>
      </div>
    </div>

    <div class="form-section">
      <h3>ข้อมูลรายการสินค้า</h3>

      <div class="form-grid">
        <div class="form-field">
          <label>รหัสสินค้า <span class="required">*</span></label>

          <InputText
            v-model="goodsId"
            maxlength="10"
            placeholder="กรอกรหัสสินค้า"
            :invalid="!!goodsError"
            fluid
            @input="handleGoodsIdChange"
            @blur="validateGoods"
          />

          <small v-if="goodsChecking" class="helper-text"> กำลังตรวจสอบสินค้า... </small>

          <small v-if="goodsError" class="error-text">
            {{ goodsError }}
          </small>
        </div>

        <div class="form-field">
          <label>ชื่อสินค้า</label>
          <InputText :model-value="goodsName" disabled />
        </div>

        <div class="form-field">
          <label>ราคาต่อหน่วย</label>
          <InputNumber
            :model-value="costUnit"
            mode="decimal"
            :min-fraction-digits="2"
            :max-fraction-digits="2"
            disabled
          />
        </div>

        <div class="form-field">
          <label>จำนวนที่สั่ง <span class="required">*</span></label>

          <InputNumber
            v-model="amount"
            class="amount-input"
            :min="0"
            :max="9999999.99"
            :min-fraction-digits="0"
            :max-fraction-digits="2"
            :use-grouping="true"
            placeholder="ระบุจำนวน"
            :invalid="!!amountError"
            fluid
          />

          <small v-if="amountError" class="error-text">
            {{ amountError }}
          </small>
        </div>

        <div class="form-field">
          <label>วันที่กำหนดส่ง <span class="required">*</span></label>

          <DatePicker
            v-model="ordDate"
            date-format="dd/mm/yy"
            :min-date="orderDateValue ?? undefined"
            :invalid="!!dateError"
            show-icon
            fluid
          />
        </div>

        <div class="form-field">
          <label>วันที่ส่งสินค้าจริง</label>

          <DatePicker
            v-model="finDate"
            date-format="dd/mm/yy"
            :min-date="ordDate ?? undefined"
            :invalid="!!dateError"
            show-icon
            placeholder="ยังไม่ระบุวันที่ส่ง"
            show-button-bar
            fluid
          />

          <small class="helper-text"> ไม่ระบุได้ และสามารถเลือกวันที่ในอนาคตได้ </small>
        </div>
      </div>

      <small v-if="dateError" class="error-text">
        {{ dateError }}
      </small>
    </div>

    <div class="total-section">
      <span>รวมเป็นเงิน</span>
      <strong>{{ formatNumber(totalPrice) }}</strong>
    </div>

    <div class="form-actions">
      <Button label="ยกเลิก" severity="secondary" outlined @click="emit('cancel')" />

      <Button
        :label="mode === 'edit' ? 'บันทึกการแก้ไข' : 'เพิ่มรายการ'"
        icon="pi pi-check"
        :disabled="!canSubmit"
        :loading="goodsChecking"
        @click="handleSubmit"
      />
    </div>
  </div>
</template>

<style scoped>
.order-detail-form {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: 24px;
}

.form-section h3 {
  margin: 0 0 16px;
  font-size: 16px;
  font-weight: 600;
}

.form-field :deep(.p-inputtext),
.form-field :deep(.p-inputnumber),
.form-field :deep(.p-datepicker),
.form-field :deep(.p-inputnumber-input) {
  width: 100%;
}

.total-section {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 24px;
  padding: 16px 0;
  border-top: 1px solid var(--color-border);
  font-size: 15px;
}

.total-section strong {
  font-size: 20px;
  color: var(--color-primary-hover);
}

@media (max-width: 640px) {
  .order-detail-form {
    padding: 16px;
  }
}
</style>
