import { describe, expect, it } from 'vitest'
import { mount } from '@vue/test-utils'
import CustomerFormDialog from '../components/customers/CustomerFormDialog.vue'
import { primeVueStubs } from './primevue-stubs'

function mountForm() {
  return mount(CustomerFormDialog, {
    props: { visible: true, customer: null },
    global: { stubs: primeVueStubs },
  })
}

describe('CustomerFormDialog - customer validation', () => {
  it('does not save when customer ID is not exactly 5 characters or name is empty', async () => {
    const wrapper = mountForm()

    await wrapper.find('#customerId').setValue('1234')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('รหัสลูกค้าต้องมีความยาว 5 ตัวอักษรพอดี')
    expect(wrapper.text()).toContain('กรุณาระบุชื่อลูกค้า')
    expect(wrapper.emitted('save')).toBeUndefined()
  })

  it('saves a customer with a 5-character ID and non-empty name', async () => {
    const wrapper = mountForm()

    await wrapper.find('#customerId').setValue('C0001')
    await wrapper.find('#customerName').setValue('KKF Customer')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('save')?.[0]).toEqual([{ cusId: 'C0001', cusName: 'KKF Customer' }])
  })
})
