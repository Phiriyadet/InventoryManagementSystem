<script setup lang="ts">
import { ref, onMounted } from 'vue'

import Button from 'primevue/button'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'

import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'

import CustomerTable from '../components/customer/CustomersTable.vue'
import CustomerFormDialog from '../components/customer/CustomerFormDialog.vue'

import { customerService } from '../services/customerService'

import type { Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../types/customer'

const confirm = useConfirm()
const toast = useToast()

const customerList = ref<Customer[]>([])
const loading = ref(false)

const dialogVisible = ref(false)
const selectedCustomer = ref<Customer | null>(null)

async function loadCustomer() {
  loading.value = true

  try {
    customerList.value = await customerService.getAll()
  } catch (error) {
    console.error(error)

    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Unable to load customer data.',
      life: 3000,
    })
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  selectedCustomer.value = null
  dialogVisible.value = true
}

function openEditDialog(customer: Customer) {
  selectedCustomer.value = { ...customer }
  dialogVisible.value = true
}

async function saveCustomer(data: CreateCustomerRequest | UpdateCustomerRequest) {
  try {
    if (selectedCustomer.value) {
      await customerService.update(selectedCustomer.value.cusId, data as UpdateCustomerRequest)
    } else {
      await customerService.create(data as CreateCustomerRequest)
    }

    dialogVisible.value = false

    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Customer saved successfully.',
      life: 3000,
    })

    await loadCustomer()
  } catch (error) {
    console.error(error)

    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Unable to save customer.',
      life: 3000,
    })
  }
}

function confirmDelete(customer: Customer) {
  confirm.require({
    message: `Are you sure you want to delete "${customer.cusName}" (${customer.cusId})?`,
    header: 'Confirm Delete',
    icon: 'pi pi-exclamation-triangle',

    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      outlined: true,
    },

    acceptProps: {
      label: 'Delete',
      severity: 'danger',
    },

    accept: async () => {
      try {
        await customerService.delete(customer.cusId)

        toast.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Customer deleted successfully.',
          life: 3000,
        })

        await loadCustomer()
      } catch (error) {
        console.error(error)

        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Unable to delete customer.',
          life: 3000,
        })
      }
    },
  })
}

onMounted(loadCustomer)
</script>

<template>
  <div class="customer-page">
    <Toast />
    <ConfirmDialog />

    <div class="page-header">
      <div>
        <h1>Customer Management</h1>
        <p>Manage your customer information.</p>
      </div>

      <Button label="Add Customer" icon="pi pi-plus" @click="openCreateDialog" />
    </div>

    <CustomerTable
      :customer="customerList"
      :loading="loading"
      @edit="openEditDialog"
      @delete="confirmDelete"
    />

    <CustomerFormDialog
      v-model:visible="dialogVisible"
      :customer="selectedCustomer"
      @save="saveCustomer"
    />
  </div>
</template>

<style scoped>
.customer-page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.page-header h1 {
  margin: 0;
  font-size: 1.5rem;
}

.page-header p {
  margin: 0.5rem 0 0;
  color: var(--p-text-muted-color);
}
</style>
