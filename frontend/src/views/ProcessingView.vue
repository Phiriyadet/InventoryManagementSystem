<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import DatePicker from 'primevue/datepicker'
import { orderProcessingService } from '@/services/orderProcessingService'
import { dateToIsoDateString } from '@/utils/date'

const router = useRouter()

const startDate = ref<Date | null>(null)
const endDate = ref<Date | null>(null)

const result = ref<number>(0)
const hasProcessed = ref(false)

const loading = ref(false)
const errorMessage = ref('')

const handleProcess = async () => {
  errorMessage.value = ''
  hasProcessed.value = false

  if (!startDate.value || !endDate.value) {
    errorMessage.value = 'กรุณาระบุวันที่ให้ครบถ้วน'
    return
  }

  if (startDate.value > endDate.value) {
    errorMessage.value = 'วันที่เริ่มต้นต้องไม่มากกว่าวันที่สิ้นสุด'
    return
  }

  if (!confirm('ยืนยันการประมวลผลข้อมูลสั่งซื้อสินค้า?')) {
    return
  }

  loading.value = true

  try {
    result.value = await orderProcessingService.processOrders(
      dateToIsoDateString(startDate.value),
      dateToIsoDateString(endDate.value),
    )
    hasProcessed.value = true
  } catch (error) {
    console.error('Fetch report failed:', error)
    errorMessage.value = 'ไม่สามารถเรียกรายงานได้ กรุณาลองใหม่อีกครั้ง'
  } finally {
    loading.value = false
  }
}

const handleCancel = () => {
  router.back()
}
</script>

<template>
  <div class="page">
    <h2>การประมวลผลข้อมูลสั่งซื้อสินค้า</h2>
    <p>ประมวลผลข้อมูลสั่งซื้อสินค้าไปเก็บไว้ใน Master File</p>

    <div class="processing-card">
      <div class="form-group">
        <label for="startDate">ระหว่างวันที่ส่งสินค้า</label>
        <DatePicker
          id="startDate"
          v-model="startDate"
          dateFormat="dd/mm/yy"
          placeholder="วว/ดด/ปปปป"
          showIcon
          fluid
        />
      </div>

      <div class="form-group">
        <label for="endDate">ถึงวันที่</label>
        <DatePicker
          id="endDate"
          v-model="endDate"
          dateFormat="dd/mm/yy"
          placeholder="วว/ดด/ปปปป"
          showIcon
          fluid
        />
      </div>

      <p v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </p>
      <p v-if="result > 0" class="success-message">ประมวลผลสำเร็จ {{ result }} รายการ</p>

      <p v-else-if="result === 0 && hasProcessed" class="success-message">
        ไม่มีรายการใหม่ที่ต้องประมวลผล
      </p>

      <div class="form-actions">
        <button class="btn-primary" :disabled="loading" @click="handleProcess">
          {{ loading ? 'กำลังประมวลผล...' : 'ตกลง' }}
        </button>

        <button class="btn-secondary" :disabled="loading" @click="handleCancel">ยกเลิก</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.processing-card {
  max-width: 650px;
  margin-top: 24px;
  padding: 28px;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 20px;
}

.form-group label {
  font-size: 14px;
  font-weight: 500;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 28px;
}

.form-actions button {
  padding: 10px 24px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.btn-primary {
  background: #2563eb;
  color: white;
}

.btn-secondary {
  background: #e2e8f0;
  color: #334155;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  color: #dc2626;
  font-size: 13px;
}
</style>
