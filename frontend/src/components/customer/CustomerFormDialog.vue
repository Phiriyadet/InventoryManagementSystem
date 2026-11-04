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

watch(
  () => [props.visible, props.customer] as const,
  ([visible, customer]) => {
    if (!visible) return

    form.customerId = customer?.cusId ?? ''
    form.customerName = customer?.cusName ?? ''
  },
  { immediate: true },
)

function submit() {
  if (!form.customerName.trim()) return

  if (!isEdit.value && !form.customerId.trim()) return

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
      <div class="form-field">
        <label for="customerId">Customer ID</label>

        <InputText
          id="customerId"
          v-model="form.customerId"
          :disabled="isEdit"
          maxlength="10"
          placeholder="Enter customer ID"
          fluid
        />
      </div>

      <div class="form-field">
        <label for="customerName">Customer Name</label>

        <InputText
          id="customerName"
          v-model="form.customerName"
          maxlength="30"
          placeholder="Enter customer name"
          fluid
          required
        />
      </div>

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

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  margin-top: 1rem;
}
</style>
