import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import OrderDetailForm from '../components/orders/OrderDetailForm.vue'
import { goodsService } from '../services/goodsService'
import { primeVueStubs } from './primevue-stubs'

vi.mock('../services/goodsService', () => ({
  goodsService: { getById: vi.fn() },
}))

const getById = vi.mocked(goodsService.getById)

function mountForm(existingGoodsIds: string[] = []) {
  return mount(OrderDetailForm, {
    props: {
      orderNo: 1001,
      cusId: 'C0001',
      cusName: 'KKF Customer',
      orderDate: '2026-11-09',
      existingGoodsIds,
    },
    global: { stubs: primeVueStubs },
  })
}

function getFieldInput(wrapper: ReturnType<typeof mountForm>, label: string) {
  const field = wrapper
    .findAll('.form-field')
    .find((item) => item.find('label').text().includes(label))

  if (!field) throw new Error(`ไม่พบช่องกรอกข้อมูล: ${label}`)

  return field.find('input')
}

async function selectValidGoods(wrapper: ReturnType<typeof mountForm>) {
  const goodsIdInput = wrapper.find('input[placeholder="กรอกรหัสสินค้า"]')

  await goodsIdInput.setValue('GOODS00001')
  await goodsIdInput.trigger('blur')
  await flushPromises()
}

describe('OrderDetailForm - order detail validation and calculation', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getById.mockResolvedValue({
      goodsId: 'GOODS00001',
      goodsName: 'Product',
      costUnit: 12.5,
    })
  })

  it('validates goods ID against goods master and calculates total price', async () => {
    const wrapper = mountForm()
    await selectValidGoods(wrapper)

    await wrapper.find('.amount-input').setValue('2')

    expect(getById).toHaveBeenCalledWith('GOODS00001')

    const goodsNameInput = getFieldInput(wrapper, 'ชื่อสินค้า')
    expect((goodsNameInput.element as HTMLInputElement).value).toBe('Product')

    expect(wrapper.text()).toContain('25.00')
  })

  it('submits order detail with null actual delivery date when it has not been delivered', async () => {
    const wrapper = mountForm()
    await selectValidGoods(wrapper)

    await getFieldInput(wrapper, 'วันที่กำหนดส่ง').setValue('2026-11-10')
    await wrapper.find('.amount-input').setValue('2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('เพิ่มรายการ'))!
      .trigger('click')

    expect(wrapper.emitted('submit')?.[0]).toEqual([
      {
        orderNo: 1001,
        goodsId: 'GOODS00001',
        ordDate: '2026-11-10',
        finDate: null,
        amount: 2,
      },
    ])
  })

  it.each([0, 10_000_000])('does not allow invalid order amount: %s', async (amount) => {
    const wrapper = mountForm()
    await selectValidGoods(wrapper)
    await wrapper.find('.amount-input').setValue(String(amount))

    const addButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('เพิ่มรายการ'))
    expect(addButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.emitted('submit')).toBeUndefined()
  })

  it('rejects a planned delivery date earlier than order date', async () => {
    const wrapper = mountForm()
    await selectValidGoods(wrapper)

    await getFieldInput(wrapper, 'วันที่กำหนดส่ง').setValue('2026-11-08')
    await wrapper.find('.amount-input').setValue('1')

    expect(wrapper.text()).toContain('วันที่กำหนดส่งต้องไม่ก่อนวันที่สั่ง Order')
  })

  it('does not allow adding a duplicate goods item to the same order', async () => {
    const wrapper = mountForm(['GOODS00001'])
    const goodsIdInput = getFieldInput(wrapper, 'รหัสสินค้า')

    await goodsIdInput.setValue('GOODS00001')
    await goodsIdInput.trigger('blur')
    await flushPromises()

    expect(wrapper.text()).toContain('สินค้านี้มีอยู่ใน Order แล้ว ไม่สามารถเพิ่มซ้ำได้')
    expect(getById).not.toHaveBeenCalled()
  })
})
