<script setup lang="ts">
import { reactive, watch, computed } from 'vue'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'

import type { Goods, CreateGoodsRequest, UpdateGoodsRequest } from '../../types/goods'

const props = defineProps<{
  visible: boolean
  goods: Goods | null
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  save: [data: CreateGoodsRequest | UpdateGoodsRequest]
}>()

const isEdit = computed(() => props.goods !== null)

const form = reactive<{
  goodsId: string
  goodsName: string
  costUnit: number | null
}>({
  goodsId: '',
  goodsName: '',
  costUnit: 0,
})

const errors = reactive({
  goodsId: '',
  goodsName: '',
  costUnit: '',
})

watch(
  () => [props.visible, props.goods] as const,
  ([visible, goods]) => {
    if (!visible) return

    form.goodsId = goods?.goodsId ?? ''
    form.goodsName = goods?.goodsName ?? ''
    form.costUnit = goods?.costUnit ?? 0

    clearErrors()
  },
  { immediate: true },
)

function clearErrors() {
  errors.goodsId = ''
  errors.goodsName = ''
  errors.costUnit = ''
}

// Goods ID: Required, exactly 10 characters
function validateGoodsId(): boolean {
  if (isEdit.value) return true

  const value = form.goodsId.trim()

  if (!value) {
    errors.goodsId = 'กรุณาระบุรหัสสินค้า'
  } else if (value.length !== 10) {
    errors.goodsId = 'รหัสสินค้าต้องมีความยาว 10 ตัวอักษรพอดี'
  } else {
    errors.goodsId = ''
  }

  return !errors.goodsId
}

// Goods Name: Required, maximum 30 characters
function validateGoodsName(): boolean {
  const value = form.goodsName.trim()

  if (!value) {
    errors.goodsName = 'กรุณาระบุชื่อสินค้า'
  } else if (value.length > 30) {
    errors.goodsName = 'ชื่อสินค้าต้องไม่เกิน 30 ตัวอักษร'
  } else {
    errors.goodsName = ''
  }

  return !errors.goodsName
}

// Cost Unit: 0.01 - 999,999.99, maximum 2 decimal places
function validateCostUnit(): boolean {
  const value = form.costUnit

  if (value === null || !Number.isFinite(value)) {
    errors.costUnit = 'กรุณาระบุราคาเป็นตัวเลข'
  } else if (value <= 0) {
    errors.costUnit = 'ราคาต้องมากกว่า 0'
  } else if (value > 999999.99) {
    errors.costUnit = 'ราคาต้องไม่เกิน 999,999.99'
  } else if (Math.abs(value * 100 - Math.round(value * 100)) > 1e-8) {
    errors.costUnit = 'ราคาต้องมีทศนิยมไม่เกิน 2 ตำแหน่ง'
  } else {
    errors.costUnit = ''
  }

  return !errors.costUnit
}

function validate(): boolean {
  const isGoodsIdValid = validateGoodsId()
  const isGoodsNameValid = validateGoodsName()
  const isCostUnitValid = validateCostUnit()

  return isGoodsIdValid && isGoodsNameValid && isCostUnitValid
}

function submit() {
  if (!validate()) return

  // ป้องกัน null ก่อนส่งข้อมูล
  const costUnit = form.costUnit

  if (costUnit === null) return

  if (isEdit.value) {
    emit('save', {
      goodsName: form.goodsName.trim(),
      costUnit,
    })
  } else {
    emit('save', {
      goodsId: form.goodsId.trim(),
      goodsName: form.goodsName.trim(),
      costUnit,
    })
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? 'Edit Goods' : 'Add Goods'"
    modal
    :style="{ width: '450px' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="goods-form" @submit.prevent="submit">
      <!-- Goods ID -->
      <div class="form-field">
        <label for="goodsId">Goods ID</label>

        <InputText
          id="goodsId"
          v-model="form.goodsId"
          :disabled="isEdit"
          maxlength="10"
          placeholder="Enter goods ID"
          :invalid="!!errors.goodsId"
          fluid
          @input="validateGoodsId"
        />

        <small v-if="errors.goodsId" class="error-text">
          {{ errors.goodsId }}
        </small>
      </div>

      <!-- Goods Name -->
      <div class="form-field">
        <label for="goodsName">Goods Name</label>

        <InputText
          id="goodsName"
          v-model="form.goodsName"
          maxlength="30"
          placeholder="Enter goods name"
          :invalid="!!errors.goodsName"
          fluid
          @input="validateGoodsName"
        />

        <small v-if="errors.goodsName" class="error-text">
          {{ errors.goodsName }}
        </small>
      </div>

      <!-- Cost Unit -->
      <div class="form-field">
        <label for="costUnit">Cost Unit</label>

        <InputNumber
          id="costUnit"
          v-model="form.costUnit"
          :min="0.01"
          :max="999999.99"
          :minFractionDigits="2"
          :maxFractionDigits="2"
          mode="decimal"
          :invalid="!!errors.costUnit"
          fluid
          @update:modelValue="validateCostUnit"
        />

        <small v-if="errors.costUnit" class="error-text">
          {{ errors.costUnit }}
        </small>
      </div>

      <!-- Actions -->
      <div class="form-actions">
        <Button
          label="Cancel"
          severity="secondary"
          outlined
          type="button"
          @click="emit('update:visible', false)"
        />

        <Button :label="isEdit ? 'Update' : 'Create'" icon="pi pi-check" type="submit" />
      </div>
    </form>
  </Dialog>
</template>

<style scoped>
.goods-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>
