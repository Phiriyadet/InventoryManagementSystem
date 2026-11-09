import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import OrderHeaderForm from '../components/orders/OrderHeaderForm.vue'
import { customerService } from '../services/customerService'
import { primeVueStubs } from './primevue-stubs'

vi.mock('../services/customerService', () => ({
  customerService: { getById: vi.fn() },
}))

const getById = vi.mocked(customerService.getById)

describe('OrderHeaderForm - customer validation before creating an order', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getById.mockResolvedValue({ cusId: 'C0001', cusName: 'KKF Customer' })
  })

  it('does not query customer when ID is not exactly 5 characters', async () => {
    const wrapper = mount(OrderHeaderForm, {
      global: { stubs: primeVueStubs },
    })

    await wrapper.find('#cusId').setValue('1234')
    await wrapper.find('#cusId').trigger('blur')
    await flushPromises()

    expect(wrapper.text()).toContain('รหัสลูกค้าต้องมีความยาว 5 ตัวอักษรพอดี')
    expect(getById).not.toHaveBeenCalled()
  })

  it('loads customer name and submits header for a valid customer', async () => {
    const wrapper = mount(OrderHeaderForm, {
      global: { stubs: primeVueStubs },
    })

    await wrapper.find('#cusId').setValue('C0001')
    await wrapper.find('#cusId').trigger('blur')
    await flushPromises()

    expect(getById).toHaveBeenCalledWith('C0001')
    expect((wrapper.find('#cusName').element as HTMLInputElement).value).toBe('KKF Customer')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('เพิ่มข้อมูล'))!
      .trigger('click')

    expect(wrapper.emitted('submit')?.[0]).toEqual([
      { cusId: 'C0001', orderDate: expect.any(String) },
    ])
  })
})
