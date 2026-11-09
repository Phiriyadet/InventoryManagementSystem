<script setup lang="ts">
import { ref } from 'vue'

import OrderTable from '../components/orders/OrderTable.vue'
import OrderHeaderForm from '../components/orders/OrderHeaderForm.vue'
import OrderDetailForm from '../components/orders/OrderDetailForm.vue'
import OrderDetailTable from '../components/orders/OrderDetailTable.vue'

import { orderService } from '../services/orderService'
import { customerService } from '../services/customerService'

import type {
  OrderHeader,
  OrderDetail,
  CreateHOrderRequest,
  CreateDOrderRequest,
  UpdateDOrderRequest,
} from '../types/order'

type Screen = 'list' | 'create-header' | 'create-detail' | 'details'

type SelectedOrder = OrderHeader & {
  cusName: string
}

const currentScreen = ref<Screen>('list')

const selectedOrder = ref<SelectedOrder | null>(null)
const editingDetail = ref<OrderDetail | null>(null)
const orderDetails = ref<OrderDetail[]>([])

const orderTableRef = ref<InstanceType<typeof OrderTable> | null>(null)
const detailFormRef = ref<InstanceType<typeof OrderDetailForm> | null>(null)

// -------------------------
// Order Header
// -------------------------

function showCreateForm() {
  selectedOrder.value = null
  editingDetail.value = null
  currentScreen.value = 'create-header'
}

async function handleCreateOrder(data: CreateHOrderRequest) {
  try {
    // สร้าง Header ก่อน
    const createdOrder = await orderService.create(data)

    // หากดึงชื่อลูกค้าไม่สำเร็จ ก็ยังคง Header ที่สร้างแล้วไว้
    let cusName = createdOrder.cusId

    try {
      const customer = await customerService.getById(createdOrder.cusId)
      cusName = customer.cusName
    } catch (error) {
      console.error('Failed to load customer:', error)
    }

    selectedOrder.value = {
      ...createdOrder,
      cusName,
    }

    // โหลดรายการ Detail ของ Order ที่เพิ่งสร้าง
    await loadOrderDetails()

    editingDetail.value = null
    currentScreen.value = 'create-detail'
  } catch (error) {
    console.error('Failed to create order:', error)
    alert('ไม่สามารถสร้าง Order ได้')
  }
}

// -------------------------
// Order Detail
// -------------------------

async function loadOrderDetails() {
  if (!selectedOrder.value) return

  try {
    orderDetails.value = await orderService.getDetails(selectedOrder.value.orderNo)
  } catch (error) {
    console.error('Failed to load order details:', error)
    alert('ไม่สามารถโหลดรายการสินค้าได้')
  }
}

async function handleAddDetail(data: CreateDOrderRequest) {
  try {
    await orderService.addDetail(data)

    // โหลดข้อมูลใหม่หลังเพิ่มสำเร็จ
    await loadOrderDetails()

    // ล้างฟอร์มเพื่อเพิ่มรายการถัดไป
    detailFormRef.value?.resetForm()

    alert('เพิ่มรายการสินค้าเรียบร้อยแล้ว')
  } catch (error) {
    console.error('Failed to add order detail:', error)
    alert('ไม่สามารถเพิ่มรายการสินค้าได้ กรุณาตรวจสอบข้อมูลอีกครั้ง')
  }
}

function showEditDetailForm(detail: OrderDetail) {
  editingDetail.value = detail
  currentScreen.value = 'create-detail'
}

async function handleUpdateDetail(data: CreateDOrderRequest) {
  if (!editingDetail.value || !selectedOrder.value) return

  const oldGoodsId = editingDetail.value.goodsId

  const request: UpdateDOrderRequest = {
    goodsId: data.goodsId,
    ordDate: data.ordDate,
    finDate: data.finDate,
    amount: data.amount,
  }

  try {
    await orderService.updateDetail(selectedOrder.value.orderNo, oldGoodsId, request)

    await loadOrderDetails()

    editingDetail.value = null
    currentScreen.value = 'details'

    alert('แก้ไขรายการสินค้าเรียบร้อยแล้ว')
  } catch (error) {
    console.error('Failed to update order detail:', error)
    alert('ไม่สามารถแก้ไขรายการสินค้าได้ กรุณาตรวจสอบข้อมูลอีกครั้ง')
  }
}

async function handleDetailSubmit(data: CreateDOrderRequest) {
  if (editingDetail.value) {
    await handleUpdateDetail(data)
  } else {
    await handleAddDetail(data)
  }
}

function handleDetailCancel() {
  editingDetail.value = null

  // กลับไปหน้ารายละเอียด โดยไม่ลบ Header
  currentScreen.value = 'details'
}

async function handleDeleteDetail(goodsId: string) {
  if (!selectedOrder.value) return

  const confirmed = window.confirm(`ต้องการลบรายการสินค้า ${goodsId} ใช่หรือไม่?`)

  if (!confirmed) return

  try {
    await orderService.deleteDetail(selectedOrder.value.orderNo, goodsId)

    await loadOrderDetails()

    alert('ลบรายการสินค้าเรียบร้อยแล้ว')
  } catch (error) {
    console.error('Failed to delete order detail:', error)
    alert('ไม่สามารถลบรายการสินค้าได้')
  }
}

// -------------------------
// Navigation
// -------------------------

async function showEditForm(orderNo: number) {
  try {
    const order = await orderService.getById(orderNo)

    let cusName = order.cusId

    try {
      const customer = await customerService.getById(order.cusId)
      cusName = customer.cusName
    } catch (error) {
      console.error('Failed to load customer:', error)
    }

    selectedOrder.value = {
      ...order,
      cusName,
    }

    editingDetail.value = null

    await loadOrderDetails()

    currentScreen.value = 'details'
  } catch (error) {
    console.error('Failed to load order:', error)
    alert('ไม่สามารถโหลดข้อมูล Order ได้')
  }
}

function showOrderList() {
  selectedOrder.value = null
  editingDetail.value = null
  currentScreen.value = 'list'

  orderTableRef.value?.loadOrders()
}

function goHome() {
  showOrderList()
}
</script>

<template>
  <div class="page">
    <!-- Order List -->
    <OrderTable
      v-if="currentScreen === 'list'"
      ref="orderTableRef"
      @create="showCreateForm"
      @edit="showEditForm"
    />

    <!-- Create Order Header -->
    <OrderHeaderForm
      v-else-if="currentScreen === 'create-header'"
      @submit="handleCreateOrder"
      @cancel="showOrderList"
    />

    <!-- Create / Edit Order Detail -->
    <template v-else-if="currentScreen === 'create-detail' && selectedOrder">
      <OrderDetailForm
        ref="detailFormRef"
        :order-no="selectedOrder.orderNo"
        :cus-id="selectedOrder.cusId"
        :cus-name="selectedOrder.cusName"
        :order-date="selectedOrder.orderDate"
        :mode="editingDetail ? 'edit' : 'create'"
        :detail="editingDetail"
        :existing-goods-ids="orderDetails.map((item) => item.goodsId)"
        @submit="handleDetailSubmit"
        @cancel="handleDetailCancel"
      />

      <!-- แสดงตารางเฉพาะ Create Mode -->
      <OrderDetailTable
        v-if="!editingDetail"
        :order-no="selectedOrder.orderNo"
        :cus-id="selectedOrder.cusId"
        :cus-name="selectedOrder.cusName"
        :order-date="selectedOrder.orderDate"
        :details="orderDetails"
        :show-header="false"
        :show-navigation="false"
        @edit="showEditDetailForm"
        @delete="handleDeleteDetail"
      />
    </template>

    <!-- Order Detail List / View -->
    <OrderDetailTable
      v-else-if="currentScreen === 'details' && selectedOrder"
      :order-no="selectedOrder.orderNo"
      :cus-id="selectedOrder.cusId"
      :cus-name="selectedOrder.cusName"
      :order-date="selectedOrder.orderDate"
      :details="orderDetails"
      @edit="showEditDetailForm"
      @delete="handleDeleteDetail"
      @back="showOrderList"
      @home="goHome"
    />
  </div>
</template>
