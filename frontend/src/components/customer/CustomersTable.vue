<script setup lang="ts">
import { ref } from 'vue'

import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'

import type { Customer } from '../../types/customer'

defineProps<{
  customer: Customer[]
  loading: boolean
}>()

const emit = defineEmits<{
  edit: [customer: Customer]
  delete: [customer: Customer]
}>()

const searchText = ref('')
</script>

<template>
  <div class="customer-table">
    <!-- Toolbar -->
    <div class="table-toolbar">
      <IconField>
        <InputIcon>
          <i class="pi pi-search" />
        </InputIcon>

        <InputText v-model="searchText" placeholder="Search customer..." />
      </IconField>
    </div>

    <!-- Table -->
    <DataTable
      :value="customer"
      :loading="loading"
      dataKey="cusId"
      paginator
      :rows="10"
      :rowsPerPageOptions="[5, 10, 20, 50]"
      :globalFilterFields="['cusId', 'cusName']"
      :filters="{
        global: {
          value: searchText,
          matchMode: 'contains',
        },
      }"
      sortMode="single"
      stripedRows
      showGridlines
      responsiveLayout="scroll"
      emptyMessage="No customer found."
    >
      <Column field="cusId" header="ID" sortable style="min-width: 120px" />

      <Column field="cusName" header="Name" sortable style="min-width: 200px" />
      <Column header="Actions" :exportable="false" style="min-width: 140px">
        <template #body="{ data }">
          <div class="action-buttons">
            <Button
              icon="pi pi-pencil"
              severity="info"
              text
              rounded
              aria-label="Edit"
              @click="emit('edit', data)"
            />

            <Button
              icon="pi pi-trash"
              severity="danger"
              text
              rounded
              aria-label="Delete"
              @click="emit('delete', data)"
            />
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<style scoped>
.table-toolbar {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 1rem;
}

.action-buttons {
  display: flex;
  justify-content: center;
  gap: 0.25rem;
}
</style>
