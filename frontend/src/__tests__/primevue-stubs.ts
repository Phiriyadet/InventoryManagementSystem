import { defineComponent } from 'vue'

const InputTextStub = defineComponent({
  name: 'InputText',
  inheritAttrs: true,
  props: {
    modelValue: { type: [String, Number], default: '' },
    id: { type: String, default: undefined },
    disabled: { type: Boolean, default: false },
    maxlength: { type: [String, Number], default: undefined },
  },
  emits: ['update:modelValue', 'input', 'blur'],
  template: `
    <input
      :id="id"
      type="text"
      :value="modelValue"
      :disabled="disabled"
      :maxlength="maxlength"
      @input="$emit('update:modelValue', $event.target.value); $emit('input', $event)"
      @blur="$emit('blur', $event)"
    />
  `,
})

const InputNumberStub = defineComponent({
  name: 'InputNumber',
  props: {
    modelValue: { type: Number, default: null },
    id: { type: String, default: undefined },
    disabled: { type: Boolean, default: false },
  },
  emits: ['update:modelValue'],
  template: `
    <input
      :id="id"
      type="number"
      :value="modelValue ?? ''"
      :disabled="disabled"
      @input="$emit('update:modelValue', $event.target.value === '' ? null : Number($event.target.value))"
    />
  `,
})

const DatePickerStub = defineComponent({
  name: 'DatePicker',
  props: {
    modelValue: { type: Date, default: null },
    id: { type: String, default: undefined },
    disabled: { type: Boolean, default: false },
  },
  emits: ['update:modelValue'],
  computed: {
    dateValue(): string {
      if (!this.modelValue) return ''
      const year = this.modelValue.getFullYear()
      const month = String(this.modelValue.getMonth() + 1).padStart(2, '0')
      const day = String(this.modelValue.getDate()).padStart(2, '0')
      return `${year}-${month}-${day}`
    },
  },
  methods: {
    updateDate(event: Event) {
      const value = (event.target as HTMLInputElement).value
      if (!value) {
        this.$emit('update:modelValue', null)
        return
      }

      const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value)
      if (!match) return

      const yearPart = match[1]
      const monthPart = match[2]
      const dayPart = match[3]

      if (!yearPart || !monthPart || !dayPart) return

      const year = Number(yearPart)
      const month = Number(monthPart)
      const day = Number(dayPart)
      this.$emit('update:modelValue', new Date(year, month - 1, day))
    },
  },
  template: `
    <input
      :id="id"
      type="date"
      :value="dateValue"
      :disabled="disabled"
      @input="updateDate"
    />
  `,
})

const ButtonStub = defineComponent({
  name: 'Button',
  props: {
    label: { type: String, default: '' },
    type: { type: String, default: 'button' },
    disabled: { type: Boolean, default: false },
  },
  template: '<button :type="type" :disabled="disabled"><slot />{{ label }}</button>',
})

const SlotStub = defineComponent({
  template: '<div><slot /></div>',
})

export const primeVueStubs = {
  Dialog: defineComponent({
    name: 'Dialog',
    props: { visible: { type: Boolean, default: true } },
    template: '<div v-if="visible"><slot /></div>',
  }),
  InputText: InputTextStub,
  InputNumber: InputNumberStub,
  DatePicker: DatePickerStub,
  Button: ButtonStub,
  Toast: SlotStub,
  ConfirmDialog: SlotStub,
  Message: SlotStub,
  DataTable: SlotStub,
  Column: SlotStub,
  IconField: SlotStub,
  InputIcon: SlotStub,
}
