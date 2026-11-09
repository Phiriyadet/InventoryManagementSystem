<script setup lang="ts">
import { onMounted, ref } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Message from 'primevue/message'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'

import { orderService } from '../../services/orderService'
import type { OrderSummary } from '../../types/order'
import { formatNumber } from '@/utils/number'

const emit = defineEmits<{
  create: []
  edit: [orderNo: number]
}>()

const confirm = useConfirm()

const orders = ref<OrderSummary[]>([])
const loading = ref(false)
const errorMessage = ref('')

async function loadOrders() {
  loading.value = true
  errorMessage.value = ''

  try {
    orders.value = await orderService.getAll()
  } catch (error) {
    console.error('Failed to load orders:', error)
    errorMessage.value = 'ไม่สามารถโหลดข้อมูลการสั่งซื้อได้'
  } finally {
    loading.value = false
  }
}

function editOrder(order: OrderSummary) {
  emit('edit', order.orderNo)
}

function deleteOrder(orderNo: number) {
  confirm.require({
    message: `คุณต้องการลบ Order No. ${orderNo} ใช่หรือไม่?`,
    header: 'ยืนยันการลบข้อมูล',
    icon: 'pi pi-exclamation-triangle',
    rejectLabel: 'ยกเลิก',
    acceptLabel: 'ลบข้อมูล',
    rejectProps: {
      label: 'ยกเลิก',
      severity: 'secondary',
      outlined: true,
    },
    acceptProps: {
      label: 'ลบข้อมูล',
      severity: 'danger',
    },
    accept: async () => {
      try {
        await orderService.delete(orderNo)
        await loadOrders()
      } catch (error) {
        console.error('Failed to delete order:', error)
        errorMessage.value = 'ไม่สามารถลบข้อมูลการสั่งซื้อได้'
      }
    },
  })
}

onMounted(loadOrders)

defineExpose({
  loadOrders,
})
</script>

<template>
  <div class="order-list">
    <ConfirmDialog />

    <div class="page-header">
      <h2 class="page-heading">แสดงข้อมูลการสั่งซื้อสินค้า</h2>
    </div>

    <div class="table-toolbar">
      <Button label="เพิ่มข้อมูลการสั่งซื้อสินค้า" icon="pi pi-plus" @click="emit('create')" />
    </div>

    <Message v-if="errorMessage" severity="error" closable @close="errorMessage = ''">
      {{ errorMessage }}
    </Message>

    <div class="table-container">
      <DataTable
        :value="orders"
        :loading="loading"
        dataKey="orderNo"
        paginator
        :rows="10"
        :rowsPerPageOptions="[5, 10, 20, 50]"
        stripedRows
        showGridlines
        sortMode="single"
        responsiveLayout="scroll"
        emptyMessage="ไม่พบข้อมูลใบสั่งซื้อ"
      >
        <Column field="orderNo" header="เลขที่ใบสั่งซื้อ" sortable style="min-width: 150px" />

        <Column field="cusId" header="รหัสลูกค้า" sortable style="min-width: 120px" />

        <Column field="cusName" header="ชื่อลูกค้า" sortable style="min-width: 200px" />

        <Column field="count" header="จำนวนรายการ" sortable style="min-width: 130px">
          <template #body="{ data }">
            {{ formatNumber(data.count) }}
          </template>
        </Column>

        <Column field="amount" header="ยอดรวม" sortable style="min-width: 150px">
          <template #body="{ data }">
            {{ formatNumber(data.amount) }}
          </template>
        </Column>

        <Column header="จัดการ" :exportable="false" style="min-width: 120px">
          <template #body="{ data }">
            <div class="action-buttons">
              <Button
                icon="pi pi-pencil"
                severity="info"
                text
                rounded
                aria-label="แก้ไข"
                @click="editOrder(data)"
              />

              <Button
                icon="pi pi-trash"
                severity="danger"
                text
                rounded
                aria-label="ลบ"
                @click="deleteOrder(data.orderNo)"
              />
            </div>
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>
