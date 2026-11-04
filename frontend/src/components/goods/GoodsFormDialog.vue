<script setup lang="ts">
import { reactive, watch, computed } from 'vue'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'

import type { Goods, CreateGoodsRequest, UpdateGoodsRequest } from '../../types/goods'

const props = defineProps<{
  visible: boolean
  goods: Goods | null
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  save: [data: CreateGoodsRequest | UpdateGoodsRequest]
}>()

const isEdit = computed(() => props.goods !== null)

const form = reactive({
  goodsId: '',
  goodsName: '',
  costUnit: 0,
})

watch(
  () => [props.visible, props.goods] as const,
  ([visible, goods]) => {
    if (!visible) return

    form.goodsId = goods?.goodsId ?? ''
    form.goodsName = goods?.goodsName ?? ''
    form.costUnit = goods?.costUnit ?? 0
  },
  { immediate: true },
)

function submit() {
  if (!form.goodsName.trim() || form.costUnit < 0) return

  if (!isEdit.value && !form.goodsId.trim()) return

  if (isEdit.value) {
    emit('save', {
      goodsName: form.goodsName.trim(),
      costUnit: form.costUnit,
    })
  } else {
    emit('save', {
      goodsId: form.goodsId.trim(),
      goodsName: form.goodsName.trim(),
      costUnit: form.costUnit,
    })
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? 'Edit Goods' : 'Add Goods'"
    modal
    :style="{ width: '450px' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="goods-form" @submit.prevent="submit">
      <div class="form-field">
        <label for="goodsId">Goods ID</label>

        <InputText
          id="goodsId"
          v-model="form.goodsId"
          :disabled="isEdit"
          maxlength="10"
          placeholder="Enter goods ID"
          fluid
        />
      </div>

      <div class="form-field">
        <label for="goodsName">Goods Name</label>

        <InputText
          id="goodsName"
          v-model="form.goodsName"
          maxlength="30"
          placeholder="Enter goods name"
          fluid
          required
        />
      </div>

      <div class="form-field">
        <label for="costUnit">Cost Unit</label>

        <InputNumber
          id="costUnit"
          v-model="form.costUnit"
          :min="0"
          :max="999999.99"
          :minFractionDigits="2"
          :maxFractionDigits="2"
          mode="decimal"
          fluid
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
.goods-form {
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
