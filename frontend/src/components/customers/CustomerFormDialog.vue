<script setup lang="ts">
import { reactive, watch, computed } from 'vue'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

import type { Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../../types/customer'

const props = defineProps<{
  visible: boolean
  customer: Customer | null
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  save: [data: CreateCustomerRequest | UpdateCustomerRequest]
}>()

const isEdit = computed(() => props.customer !== null)

const form = reactive({
  customerId: '',
  customerName: '',
})

const errors = reactive({
  customerId: '',
  customerName: '',
})

watch(
  () => [props.visible, props.customer] as const,
  ([visible, customer]) => {
    if (!visible) return

    form.customerId = customer?.cusId ?? ''
    form.customerName = customer?.cusName ?? ''

    clearErrors()
  },
  { immediate: true },
)

function clearErrors() {
  errors.customerId = ''
  errors.customerName = ''
}

// Customer ID: Required, exactly 5 characters
function validateCustomerId(): boolean {
  if (isEdit.value) return true

  const value = form.customerId.trim()

  if (!value) {
    errors.customerId = 'กรุณาระบุรหัสลูกค้า'
  } else if (value.length !== 5) {
    errors.customerId = 'รหัสลูกค้าต้องมีความยาว 5 ตัวอักษรพอดี'
  } else {
    errors.customerId = ''
  }

  return !errors.customerId
}

// Customer Name: Required, maximum 30 characters
function validateCustomerName(): boolean {
  const value = form.customerName.trim()

  if (!value) {
    errors.customerName = 'กรุณาระบุชื่อลูกค้า'
  } else if (value.length > 30) {
    errors.customerName = 'ชื่อลูกค้าต้องไม่เกิน 30 ตัวอักษร'
  } else {
    errors.customerName = ''
  }

  return !errors.customerName
}

function validate(): boolean {
  const isCustomerIdValid = validateCustomerId()
  const isCustomerNameValid = validateCustomerName()

  return isCustomerIdValid && isCustomerNameValid
}

function submit() {
  if (!validate()) return

  if (isEdit.value) {
    emit('save', {
      cusName: form.customerName.trim(),
    })
  } else {
    emit('save', {
      cusId: form.customerId.trim(),
      cusName: form.customerName.trim(),
    })
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? 'Edit Customer' : 'Add Customer'"
    modal
    :style="{ width: '450px' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="customer-form" @submit.prevent="submit">
      <!-- Customer ID -->
      <div class="form-field">
        <label for="customerId">Customer ID</label>

        <InputText
          id="customerId"
          v-model="form.customerId"
          :disabled="isEdit"
          maxlength="5"
          placeholder="Enter customer ID"
          :invalid="!!errors.customerId"
          fluid
          @input="validateCustomerId"
        />

        <small v-if="errors.customerId" class="error-text">
          {{ errors.customerId }}
        </small>
      </div>

      <!-- Customer Name -->
      <div class="form-field">
        <label for="customerName">Customer Name</label>

        <InputText
          id="customerName"
          v-model="form.customerName"
          maxlength="30"
          placeholder="Enter customer name"
          :invalid="!!errors.customerName"
          fluid
          @input="validateCustomerName"
        />

        <small v-if="errors.customerName" class="error-text">
          {{ errors.customerName }}
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
.customer-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>
