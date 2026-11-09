import { describe, expect, it } from 'vitest'
import { mount } from '@vue/test-utils'
import GoodsFormDialog from '../components/goods/GoodsFormDialog.vue'
import { primeVueStubs } from './primevue-stubs'

function mountForm() {
  return mount(GoodsFormDialog, {
    props: { visible: true, goods: null },
    global: { stubs: primeVueStubs },
  })
}

describe('GoodsFormDialog - goods validation', () => {
  it('does not save when goods ID is not exactly 10 characters', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('G001')
    await wrapper.find('#goodsName').setValue('Product')
    await wrapper.find('#costUnit').setValue('10.50')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('รหัสสินค้าต้องมีความยาว 10 ตัวอักษรพอดี')
    expect(wrapper.emitted('save')).toBeUndefined()
  })

  it('does not save when goods name is empty', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('GOODS00001')
    await wrapper.find('#costUnit').setValue('10.50')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('กรุณาระบุชื่อสินค้า')
    expect(wrapper.emitted('save')).toBeUndefined()
  })

  it('rejects zero price because the assignment requires price greater than zero', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('GOODS00001')
    await wrapper.find('#goodsName').setValue('Product')
    await wrapper.find('#costUnit').setValue('0')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('ราคาต้องมากกว่า 0')
    expect(wrapper.emitted('save')).toBeUndefined()
  })

  it('accepts the smallest positive unit price', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('GOODS00001')
    await wrapper.find('#goodsName').setValue('Product')
    await wrapper.find('#costUnit').setValue('0.01')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('save')?.[0]).toEqual([
      { goodsId: 'GOODS00001', goodsName: 'Product', costUnit: 0.01 },
    ])
  })

  it('rejects unit price above 999,999.99', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('GOODS00001')
    await wrapper.find('#goodsName').setValue('Product')
    await wrapper.find('#costUnit').setValue('1000000')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('ราคาต้องไม่เกิน 999,999.99')
    expect(wrapper.emitted('save')).toBeUndefined()
  })

  it('saves a valid goods record at the maximum allowed unit price', async () => {
    const wrapper = mountForm()

    await wrapper.find('#goodsId').setValue('GOODS00001')
    await wrapper.find('#goodsName').setValue('Product')
    await wrapper.find('#costUnit').setValue('999999.99')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('save')?.[0]).toEqual([
      { goodsId: 'GOODS00001', goodsName: 'Product', costUnit: 999999.99 },
    ])
  })
})
