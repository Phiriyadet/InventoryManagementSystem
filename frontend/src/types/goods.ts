export interface Goods {
  goodsId: string
  goodsName: string
  costUnit: number
}

export interface CreateGoodsRequest {
  goodsId: string
  goodsName: string
  costUnit: number
}

export interface UpdateGoodsRequest {
  goodsName: string
  costUnit: number
}
