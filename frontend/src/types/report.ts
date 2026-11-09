export interface OverdueOrderReportItem {
  sequence: number
  cusId: string
  cusName: string
  goodsId: string
  goodsName: string
  docDate: string
  orderNo: number
  ordDate: string
  amount: number
  unitCost: number
  totalPrice: number
}

export interface OverdueOrderReport {
  items: OverdueOrderReportItem[]
  totalAmount: number
  totalPrice: number
}
