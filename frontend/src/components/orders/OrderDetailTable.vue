<script setup lang="ts">
import { onMounted, ref } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'

import { goodsService } from '../../services/goodsService'
import type { OrderDetail } from '../../types/order'
import { formatNumber } from '@/utils/number'
import { isoDateStringToDisplay } from '@/utils/date'

withDefaults(
  defineProps<{
    orderNo: number
    cusId: string
    cusName: string
    orderDate: string
    details: OrderDetail[]
    showHeader?: boolean
    showNavigation?: boolean
  }>(),
  {
    showHeader: true,
    showNavigation: true,
  },
)

const emit = defineEmits<{
  edit: [detail: OrderDetail]
  delete: [goodsId: string]
  back: []
  home: []
}>()

const goodsNames = ref<Record<string, string>>({})

onMounted(async () => {
  try {
    const goods = await goodsService.getAll()

    goodsNames.value = Object.fromEntries(goods.map((item) => [item.goodsId, item.goodsName]))
  } catch (error) {
    console.error('Failed to load goods:', error)
  }
})
</script>

<template>
  <div class="order-detail-table">
    <div v-if="showHeader" class="order-detail-header">
      <h2>รายละเอียด Order</h2>

      <div class="order-info-grid">
        <div>
          <span>รหัสลูกค้า</span>
          <strong>{{ cusId }}</strong>
        </div>

        <div>
          <span>ชื่อลูกค้า</span>
          <strong>{{ cusName }}</strong>
        </div>

        <div>
          <span>วันที่สั่ง Order</span>
          <strong>{{ isoDateStringToDisplay(orderDate) }}</strong>
        </div>

        <div>
          <span>Order No.</span>
          <strong>{{ orderNo }}</strong>
        </div>
      </div>
    </div>

    <div class="detail-table-heading">
      <h3>รายการสินค้า</h3>
      <span>{{ details.length }} รายการ</span>
    </div>

    <DataTable
      :value="details"
      data-key="goodsId"
      striped-rows
      responsive-layout="scroll"
      empty-message="ยังไม่มีรายการสินค้า"
    >
      <Column field="goodsId" header="รหัสสินค้า" />

      <Column header="ชื่อสินค้า">
        <template #body="{ data }">
          {{ goodsNames[data.goodsId] ?? '-' }}
        </template>
      </Column>

      <Column header="วันที่กำหนดส่ง">
        <template #body="{ data }">
          {{ isoDateStringToDisplay(data.ordDate) }}
        </template>
      </Column>

      <Column header="วันที่ส่งสินค้าจริง">
        <template #body="{ data }">
          {{ isoDateStringToDisplay(data.finDate) }}
        </template>
      </Column>

      <Column header="จำนวน" body-class="text-right">
        <template #body="{ data }">
          {{ formatNumber(data.amount) }}
        </template>
      </Column>

      <Column header="ราคาต่อหน่วย" body-class="text-right">
        <template #body="{ data }">
          {{ formatNumber(data.costUnit) }}
        </template>
      </Column>

      <Column header="รวม" body-class="text-right">
        <template #body="{ data }">
          {{ formatNumber(data.totPrc) }}
        </template>
      </Column>

      <Column header="จัดการ" :exportable="false">
        <template #body="{ data }">
          <div class="action-buttons">
            <Button
              icon="pi pi-pencil"
              severity="info"
              text
              rounded
              aria-label="แก้ไข"
              @click="emit('edit', data)"
            />

            <Button
              icon="pi pi-trash"
              severity="danger"
              text
              rounded
              aria-label="ลบ"
              @click="emit('delete', data.goodsId)"
            />
          </div>
        </template>
      </Column>
    </DataTable>

    <div v-if="showNavigation" class="detail-navigation">
      <Button
        label="กลับหน้ารายการ Order"
        icon="pi pi-arrow-left"
        severity="secondary"
        outlined
        @click="emit('back')"
      />

      <Button
        label="หน้าหลัก"
        icon="pi pi-home"
        severity="secondary"
        outlined
        @click="emit('home')"
      />
    </div>
  </div>
</template>

<style scoped>
.order-detail-table {
  margin-top: 24px;
  padding: 24px;
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
}

.order-detail-header {
  margin-bottom: 24px;
}

.order-detail-header h2 {
  margin: 0 0 16px;
  font-size: 20px;
}

.order-info-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 16px;
  padding: 16px;
  background: var(--color-background);
  border-radius: var(--radius-md);
}

.order-info-grid div {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.order-info-grid span {
  color: var(--color-muted);
  font-size: 12px;
}

.order-info-grid strong {
  font-size: 14px;
}

.detail-table-heading {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.detail-table-heading h3 {
  margin: 0;
  font-size: 16px;
}

.detail-table-heading span {
  color: var(--color-muted);
  font-size: 13px;
}

.detail-navigation {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

@media (max-width: 640px) {
  .order-info-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .order-detail-table {
    padding: 16px;
  }

  .detail-navigation {
    flex-direction: column;
  }
}
</style>
