import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import ProcessingView from '../views/ProcessingView.vue'
import { orderProcessingService } from '../services/orderProcessingService'
import { primeVueStubs } from './primevue-stubs'

vi.mock('../services/orderProcessingService', () => ({
  orderProcessingService: { processOrders: vi.fn() },
}))

vi.mock('vue-router', () => ({
  useRouter: () => ({ back: vi.fn() }),
}))

const processOrders = vi.mocked(orderProcessingService.processOrders)

describe('ProcessingView - processing completed orders', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.spyOn(window, 'confirm').mockReturnValue(true)
    processOrders.mockResolvedValue(2)
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('does not process when date range is incomplete', async () => {
    const wrapper = mount(ProcessingView, {
      global: { stubs: primeVueStubs },
    })

    await wrapper.find('.btn-primary').trigger('click')

    expect(wrapper.text()).toContain('กรุณาระบุวันที่ให้ครบถ้วน')
    expect(processOrders).not.toHaveBeenCalled()
  })

  it('does not process when start date is after end date', async () => {
    const wrapper = mount(ProcessingView, {
      global: { stubs: primeVueStubs },
    })

    const dates = wrapper.findAll('input[type="date"]')
    await dates[0]!.setValue('2026-11-10')
    await dates[1]!.setValue('2026-11-09')
    await wrapper.find('.btn-primary').trigger('click')

    expect(wrapper.text()).toContain('วันที่เริ่มต้นต้องไม่มากกว่าวันที่สิ้นสุด')
    expect(processOrders).not.toHaveBeenCalled()
  })

  it('does not process when user cancels confirmation', async () => {
    vi.mocked(window.confirm).mockReturnValue(false)
    const wrapper = mount(ProcessingView, {
      global: { stubs: primeVueStubs },
    })

    const dates = wrapper.findAll('input[type="date"]')
    await dates[0]!.setValue('2026-11-01')
    await dates[1]!.setValue('2026-11-09')
    await wrapper.find('.btn-primary').trigger('click')

    expect(window.confirm).toHaveBeenCalledOnce()
    expect(processOrders).not.toHaveBeenCalled()
  })

  it('sends selected date range for processing and displays processed count', async () => {
    const wrapper = mount(ProcessingView, {
      global: { stubs: primeVueStubs },
    })

    const dates = wrapper.findAll('input[type="date"]')
    await dates[0]!.setValue('2026-11-01')
    await dates[1]!.setValue('2026-11-09')
    await wrapper.find('.btn-primary').trigger('click')
    await flushPromises()

    expect(processOrders).toHaveBeenCalledWith('2026-11-01', '2026-11-09')
    expect(wrapper.text()).toContain('ประมวลผลสำเร็จ 2 รายการ')
  })
})
