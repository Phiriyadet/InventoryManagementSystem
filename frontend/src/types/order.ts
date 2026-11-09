export interface OrderSummary {
  cusId: string
  cusName: string
  orderNo: number
  count: number
  amount: number
}

export interface OrderHeader {
  orderNo: number
  cusId: string
  orderDate: string
}

export interface OrderDetail {
  orderNo: number
  goodsId: string
  ordDate: string
  finDate: string
  amount: number
  costUnit: number
  totPrc: number
}

export interface CreateHOrderRequest {
  cusId: string
  orderDate: string
}

export interface CreateDOrderRequest {
  orderNo: number
  goodsId: string
  ordDate: string
  finDate: string | null
  amount: number
}

export interface UpdateDOrderRequest {
  goodsId: string
  ordDate: string
  finDate: string | null
  amount: number
}
