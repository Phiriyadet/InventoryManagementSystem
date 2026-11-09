<script setup lang="ts">
import { ref } from 'vue'
import DatePicker from 'primevue/datepicker'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Message from 'primevue/message'

import { reportService } from '@/services/reportService'
import type { OverdueOrderReport } from '@/types/report'
import { formatNumber } from '@/utils/number'
import { dateToDisplay, dateToIsoDateString, isoDateStringToDisplay } from '@/utils/date'

const startDate = ref<Date | null>(null)
const endDate = ref<Date | null>(null)

const report = ref<OverdueOrderReport | null>(null)

const loading = ref(false)
const errorMessage = ref('')

async function fetchReport() {
  errorMessage.value = ''
  report.value = null

  if (!startDate.value || !endDate.value) {
    errorMessage.value = 'กรุณาเลือกวันที่เริ่มต้นและวันที่สิ้นสุด'
    return
  }

  if (startDate.value > endDate.value) {
    errorMessage.value = 'วันที่เริ่มต้นต้องไม่มากกว่าวันที่สิ้นสุด'
    return
  }

  loading.value = true

  try {
    report.value = await reportService.getReport(
      dateToIsoDateString(startDate.value),
      dateToIsoDateString(endDate.value),
    )
  } catch (error) {
    console.error('Fetch report failed:', error)
    errorMessage.value = 'ไม่สามารถเรียกรายงานได้ กรุณาลองใหม่อีกครั้ง'
  } finally {
    loading.value = false
  }
}

function printReport() {
  window.print()
}
</script>

<template>
  <div class="report-page">
    <div class="report-header">
      <div>
        <h2>รายงานแสดงข้อมูลที่ครบกำหนดส่งสินค้าแล้วยังไม่ได้ส่งสินค้า</h2>
        <p>แสดงรายละเอียดคำสั่งซื้อและวันที่กำหนดส่งตามแผน</p>
      </div>

      <Button
        label="พิมพ์รายงาน"
        icon="pi pi-print"
        severity="secondary"
        outlined
        :disabled="!report"
        class="no-print"
        @click="printReport"
      />
    </div>

    <div class="report-filter no-print">
      <div class="filter-field">
        <label for="startDate">ตั้งแต่วันที่</label>
        <DatePicker
          id="startDate"
          v-model="startDate"
          dateFormat="dd/mm/yy"
          placeholder="เลือกวันที่เริ่มต้น"
          showIcon
          :maxDate="endDate ?? undefined"
          :manualInput="false"
        />
      </div>

      <div class="filter-field">
        <label for="endDate">ถึงวันที่</label>
        <DatePicker
          id="endDate"
          v-model="endDate"
          dateFormat="dd/mm/yy"
          placeholder="เลือกวันที่สิ้นสุด"
          showIcon
          :minDate="startDate ?? undefined"
          :manualInput="false"
        />
      </div>

      <Button label="แสดง" icon="pi pi-search" :loading="loading" @click="fetchReport" />
    </div>

    <Message v-if="errorMessage" severity="error" :closable="false" class="no-print">
      {{ errorMessage }}
    </Message>

    <div v-if="report" class="print-header">
      <h3>รายงานกำหนดส่งสินค้า</h3>
      <p>ตั้งแต่วันที่ {{ dateToDisplay(startDate!) }} ถึงวันที่ {{ dateToDisplay(endDate!) }}</p>
    </div>

    <div v-if="report" class="report-table">
      <DataTable
        :value="report.items"
        :loading="loading"
        stripedRows
        showGridlines
        size="small"
        tableStyle="min-width: 1100px"
      >
        <template #empty> ไม่พบข้อมูลในช่วงวันที่ที่เลือก </template>

        <Column
          field="sequence"
          header="ลำดับ"
          headerStyle="width: 65px"
          bodyStyle="text-align: center"
        />

        <Column header="รายละเอียดลูกค้า" headerStyle="min-width: 180px">
          <template #body="{ data }"> {{ data.cusId }} : {{ data.cusName }} </template>

          <template #footer> รวมทั้งหมด </template>
        </Column>

        <Column header="รายละเอียดสินค้า" headerStyle="min-width: 180px">
          <template #body="{ data }"> {{ data.goodsId }} : {{ data.goodsName }} </template>
        </Column>

        <Column header="วันที่สั่ง" headerStyle="min-width: 110px">
          <template #body="{ data }">
            {{ isoDateStringToDisplay(data.docDate) }}
          </template>
        </Column>

        <Column field="orderNo" header="เลขที่สั่ง" />

        <Column header="วันกำหนดส่ง" headerStyle="min-width: 110px">
          <template #body="{ data }">
            {{ isoDateStringToDisplay(data.ordDate) }}
          </template>
        </Column>

        <Column header="จำนวน" bodyStyle="text-align: right" footerStyle="text-align: right">
          <template #body="{ data }">
            {{ formatNumber(data.amount) }}
          </template>

          <template #footer>
            {{ formatNumber(report.totalAmount) }}
          </template>
        </Column>

        <Column header="ราคา/หน่วย" bodyStyle="text-align: right">
          <template #body="{ data }">
            {{ formatNumber(data.unitCost) }}
          </template>
        </Column>

        <Column header="ราคารวม" bodyStyle="text-align: right" footerStyle="text-align: right">
          <template #body="{ data }">
            {{ formatNumber(data.totalPrice) }}
          </template>

          <template #footer>
            {{ formatNumber(report.totalPrice) }}
          </template>
        </Column>
      </DataTable>
    </div>

    <div v-else class="report-empty no-print">
      <i class="pi pi-file"></i>
      <p>เลือกช่วงวันที่แล้วกดแสดง เพื่อดูรายงาน</p>
    </div>
  </div>
</template>

<style scoped>
.report-page {
  width: 100%;
}

.report-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.report-header h2 {
  margin: 0 0 var(--spacing-sm);
  font-size: 20px;
}

.report-header p {
  margin: 0;
  color: var(--color-muted);
  font-size: 14px;
}

.report-filter {
  display: flex;
  align-items: end;
  gap: var(--spacing-md);
  padding: var(--spacing-lg);
  margin-bottom: var(--spacing-lg);
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
}

.filter-field {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
  min-width: 220px;
}

.filter-field label {
  font-size: 13px;
  font-weight: 500;
}

.filter-field :deep(.p-datepicker) {
  width: 100%;
}

.report-table {
  background: var(--color-surface);
  border-radius: var(--radius-md);
  overflow-x: auto;
}

.report-empty {
  padding: 60px 20px;
  text-align: center;
  color: var(--color-muted);
  background: var(--color-surface);
  border: 1px dashed var(--color-border);
  border-radius: var(--radius-lg);
}

.report-empty i {
  font-size: 32px;
}

.print-header {
  display: none;
}

@media (max-width: 768px) {
  .report-filter {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-field {
    width: 100%;
  }
}

@media print {
  @page {
    size: A4 portrait;
    margin: 10mm;
  }

  :global(body) {
    background: #fff !important;
  }

  :global(.sidebar),
  :global(.app-header),
  .no-print {
    display: none !important;
  }

  .report-header {
    display: none;
  }

  .report-page {
    padding: 0;
  }

  .print-header {
    display: block;
    text-align: center;
    margin-bottom: 16px;
  }

  .report-table {
    overflow: visible !important;
  }

  .report-table :deep(.p-datatable-table-container) {
    overflow: visible !important;
  }

  .report-table :deep(table) {
    width: 100% !important;
    min-width: 0 !important;
    font-size: 9pt;
  }

  .report-table :deep(th),
  .report-table :deep(td) {
    padding: 5px !important;
    white-space: normal !important;
  }

  .report-table :deep(tr) {
    break-inside: avoid;
  }
}
</style>
