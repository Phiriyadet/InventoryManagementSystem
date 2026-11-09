import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import OverdueOrderReport from '../components/report/OverdueOrderReport.vue'
import { reportService } from '../services/reportService'
import { primeVueStubs } from './primevue-stubs'

vi.mock('../services/reportService', () => ({
  reportService: { getReport: vi.fn() },
}))

const getReport = vi.mocked(reportService.getReport)

describe('OverdueOrderReport - report date range', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    getReport.mockResolvedValue({ items: [], totalAmount: 0, totalPrice: 0 })
  })

  it('does not request report when either date is missing', async () => {
    const wrapper = mount(OverdueOrderReport, {
      global: { stubs: primeVueStubs },
    })

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('แสดง'))!
      .trigger('click')

    expect(wrapper.text()).toContain('กรุณาเลือกวันที่เริ่มต้นและวันที่สิ้นสุด')
    expect(getReport).not.toHaveBeenCalled()
  })

  it('does not request report when start date is after end date', async () => {
    const wrapper = mount(OverdueOrderReport, {
      global: { stubs: primeVueStubs },
    })

    const dates = wrapper.findAll('input[type="date"]')
    await dates[0]!.setValue('2026-11-10')
    await dates[1]!.setValue('2026-11-09')
    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('แสดง'))!
      .trigger('click')

    expect(wrapper.text()).toContain('วันที่เริ่มต้นต้องไม่มากกว่าวันที่สิ้นสุด')
    expect(getReport).not.toHaveBeenCalled()
  })

  it('requests overdue report with the selected planned delivery date range', async () => {
    const wrapper = mount(OverdueOrderReport, {
      global: { stubs: primeVueStubs },
    })

    const dates = wrapper.findAll('input[type="date"]')
    await dates[0]!.setValue('2026-11-01')
    await dates[1]!.setValue('2026-11-09')
    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('แสดง'))!
      .trigger('click')
    await flushPromises()

    expect(getReport).toHaveBeenCalledWith('2026-11-01', '2026-11-09')
  })
})
