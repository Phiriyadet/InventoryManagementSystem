<script setup lang="ts">
import { ref } from 'vue'

import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import IconField from 'primevue/iconfield'
import InputIcon from 'primevue/inputicon'

import type { Goods } from '../../types/goods'

defineProps<{
  goods: Goods[]
  loading: boolean
}>()

const emit = defineEmits<{
  edit: [goods: Goods]
  delete: [goods: Goods]
}>()

const searchText = ref('')
</script>

<template>
  <div class="goods-table">
    <div class="table-toolbar">
      <IconField>
        <InputIcon>
          <i class="pi pi-search" />
        </InputIcon>

        <InputText v-model="searchText" placeholder="Search goods..." />
      </IconField>
    </div>

    <DataTable
      :value="goods"
      :loading="loading"
      dataKey="goodsId"
      paginator
      :rows="10"
      :rowsPerPageOptions="[5, 10, 20, 50]"
      :globalFilterFields="['goodsId', 'goodsName', 'costUnit']"
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
      emptyMessage="No goods found."
    >
      <Column field="goodsId" header="รหัสสินค้า" sortable style="min-width: 120px" />

      <Column field="goodsName" header="รายละเอียด" sortable style="min-width: 200px" />

      <Column field="costUnit" header="ราคา/หน่วย" sortable style="min-width: 140px">
        <template #body="{ data }">
          {{ Number(data.costUnit).toFixed(2) }}
        </template>
      </Column>

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
