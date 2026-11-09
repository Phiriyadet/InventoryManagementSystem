<script setup lang="ts">
import { ref, onMounted } from 'vue'

import Button from 'primevue/button'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'

import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'

import GoodsTable from '../components/goods/GoodsTable.vue'
import GoodsFormDialog from '../components/goods/GoodsFormDialog.vue'

import { goodsService } from '../services/goodsService'

import type { Goods, CreateGoodsRequest, UpdateGoodsRequest } from '../types/goods'

const confirm = useConfirm()
const toast = useToast()

const goodsList = ref<Goods[]>([])
const loading = ref(false)

const dialogVisible = ref(false)
const selectedGoods = ref<Goods | null>(null)

async function loadGoods() {
  loading.value = true

  try {
    goodsList.value = await goodsService.getAll()
  } catch (error) {
    console.error(error)

    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'ไม่สามารถโหลดข้อมูลสินค้าได้',
      life: 3000,
    })
  } finally {
    loading.value = false
  }
}

function openCreateDialog() {
  selectedGoods.value = null
  dialogVisible.value = true
}

function openEditDialog(goods: Goods) {
  selectedGoods.value = { ...goods }
  dialogVisible.value = true
}

async function saveGoods(data: CreateGoodsRequest | UpdateGoodsRequest) {
  try {
    if (selectedGoods.value) {
      await goodsService.update(selectedGoods.value.goodsId, data as UpdateGoodsRequest)
    } else {
      await goodsService.create(data as CreateGoodsRequest)
    }

    dialogVisible.value = false

    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: 'บันทึกข้อมูลสินค้าสำเร็จแล้ว',
      life: 3000,
    })

    await loadGoods()
  } catch (error) {
    console.error(error)

    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'ไม่สามารถบันทึกข้อมูลสินค้าได้',
      life: 3000,
    })
  }
}

function confirmDelete(goods: Goods) {
  confirm.require({
    message: `คุณแน่ใจหรือไม่ ว่าต้องการลบข้อมูล  "${goods.goodsName}" (${goods.goodsId})?`,
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
        await goodsService.delete(goods.goodsId)

        toast.add({
          severity: 'success',
          summary: 'Success',
          detail: 'ลบข้อมูลสินค้าสำเร็จแล้ว',
          life: 3000,
        })

        await loadGoods()
      } catch (error) {
        console.error(error)

        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: 'ไม่สามารถลบข้อมูลสินค้าได้',
          life: 3000,
        })
      }
    },
  })
}

onMounted(loadGoods)
</script>

<template>
  <div class="goods-page">
    <Toast />
    <ConfirmDialog />

    <div class="page-header">
      <div>
        <h1>การบันทึก/แก้ไข ข้อมูลสินค้า</h1>
        <p>เพิ่ม ลบ แก้ไข ข้อมูลสินค้า</p>
      </div>

      <Button label="เพิ่มข้อมูลสินค้า" icon="pi pi-plus" @click="openCreateDialog" />
    </div>

    <GoodsTable
      :goods="goodsList"
      :loading="loading"
      @edit="openEditDialog"
      @delete="confirmDelete"
    />

    <GoodsFormDialog v-model:visible="dialogVisible" :goods="selectedGoods" @save="saveGoods" />
  </div>
</template>

<style scoped>
.goods-page {
  padding: 1.5rem;
}
</style>
