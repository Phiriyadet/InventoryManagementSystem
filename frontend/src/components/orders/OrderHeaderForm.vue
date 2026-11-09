<script setup lang="ts">
import { ref } from 'vue'
import InputText from 'primevue/inputtext'
import DatePicker from 'primevue/datepicker'
import Button from 'primevue/button'

import { customerService } from '../../services/customerService'
import type { CreateHOrderRequest } from '../../types/order'
import { dateToIsoDateString } from '@/utils/date'

const emit = defineEmits<{
  submit: [data: CreateHOrderRequest]
  cancel: []
}>()

const cusId = ref('')
const cusName = ref('')
const orderDate = ref<Date | null>(new Date())

const checkingCustomer = ref(false)
const customerValid = ref(false)
const errorMessage = ref('')
const orderDateError = ref('')

// ป้องกันผลลัพธ์จาก Request เก่าทับข้อมูลใหม่
const validationRequest = ref(0)

// เก็บ ID ที่ตรวจสอบผ่านแล้ว ป้องกัน API ซ้ำ
const validatedCustomerId = ref('')

function handleCustomerIdChange() {
  validationRequest.value++

  customerValid.value = false
  checkingCustomer.value = false
  validatedCustomerId.value = ''

  cusName.value = ''
  errorMessage.value = ''
}

async function validateCustomer() {
  const id = cusId.value.trim()
  const requestId = ++validationRequest.value

  customerValid.value = false
  cusName.value = ''
  errorMessage.value = ''

  if (!id) {
    checkingCustomer.value = false
    errorMessage.value = 'กรุณาระบุรหัสลูกค้า'
    return
  }

  if (id.length !== 5) {
    checkingCustomer.value = false
    errorMessage.value = 'รหัสลูกค้าต้องมีความยาว 5 ตัวอักษรพอดี'
    return
  }

  // ใช้ผลตรวจสอบเดิมหากเป็น ID เดียวกัน
  if (validatedCustomerId.value === id) {
    customerValid.value = true
    return
  }

  checkingCustomer.value = true

  try {
    const customer = await customerService.getById(id)

    // ป้องกัน Response เก่าทับข้อมูลปัจจุบัน
    if (requestId !== validationRequest.value) return

    cusName.value = customer.cusName
    customerValid.value = true
    validatedCustomerId.value = id
  } catch {
    if (requestId !== validationRequest.value) return

    errorMessage.value = 'ไม่พบรหัสลูกค้าที่ระบุ'
  } finally {
    if (requestId === validationRequest.value) {
      checkingCustomer.value = false
    }
  }
}

function validateOrderDate(): boolean {
  const value = orderDate.value

  if (!(value instanceof Date) || Number.isNaN(value.getTime())) {
    orderDateError.value = 'กรุณาระบุวันที่สั่งซื้อ'
    return false
  }

  orderDateError.value = ''
  return true
}

function handleSubmit() {
  // ตรวจสอบข้อมูลอีกครั้งก่อนส่ง
  if (!cusId.value.trim()) {
    errorMessage.value = 'กรุณาระบุรหัสลูกค้า'
    customerValid.value = false
    return
  }

  if (cusId.value.trim().length !== 5) {
    errorMessage.value = 'รหัสลูกค้าต้องมีความยาว 5 ตัวอักษรพอดี'
    customerValid.value = false
    return
  }

  if (!customerValid.value || checkingCustomer.value) return

  if (validatedCustomerId.value !== cusId.value.trim()) return

  if (!validateOrderDate()) return

  emit('submit', {
    cusId: cusId.value.trim(),
    orderDate: dateToIsoDateString(orderDate.value!),
  })
}
</script>

<template>
  <div class="form-card">
    <h2 class="form-heading">เพิ่มข้อมูลการสั่งซื้อสินค้า</h2>

    <div class="form-section">
      <h3>ข้อมูลการสั่งซื้อ</h3>

      <div class="form-grid">
        <!-- Customer ID -->
        <div class="form-field">
          <label for="cusId"> รหัสลูกค้า <span class="required">*</span> </label>

          <InputText
            id="cusId"
            v-model="cusId"
            maxlength="5"
            placeholder="กรอกรหัสลูกค้า"
            :invalid="!!errorMessage"
            fluid
            @input="handleCustomerIdChange"
            @blur="validateCustomer"
          />

          <small v-if="checkingCustomer" class="helper-text"> กำลังตรวจสอบรหัสลูกค้า... </small>

          <small v-if="errorMessage" class="error-text">
            {{ errorMessage }}
          </small>
        </div>

        <!-- Customer Name -->
        <div class="form-field">
          <label for="cusName">ชื่อลูกค้า</label>

          <InputText
            id="cusName"
            :model-value="cusName"
            placeholder="ชื่อจะแสดงเมื่อระบุรหัสลูกค้า"
            disabled
            fluid
          />
        </div>

        <!-- Order Date -->
        <div class="form-field">
          <label for="orderDate"> วันที่สั่งซื้อ <span class="required">*</span> </label>

          <DatePicker
            id="orderDate"
            v-model="orderDate"
            date-format="dd/mm/yy"
            show-icon
            :invalid="!!orderDateError"
            fluid
            @update:model-value="orderDateError = ''"
          />

          <small v-if="orderDateError" class="error-text">
            {{ orderDateError }}
          </small>
        </div>
      </div>
    </div>

    <div class="form-actions">
      <Button label="ยกเลิก" severity="secondary" outlined @click="emit('cancel')" />

      <Button
        label="เพิ่มข้อมูล"
        icon="pi pi-plus"
        :disabled="!customerValid || checkingCustomer"
        :loading="checkingCustomer"
        @click="handleSubmit"
      />
    </div>
  </div>
</template>
