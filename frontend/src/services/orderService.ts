import api from './api'
import type {
  OrderSummary,
  OrderHeader,
  OrderDetail,
  CreateHOrderRequest,
  CreateDOrderRequest,
  UpdateDOrderRequest,
} from '../types/order'

const endpoint = '/orders'

export const orderService = {
  // Order Header
  async getAll(): Promise<OrderSummary[]> {
    const response = await api.get<OrderSummary[]>(`${endpoint}/`)
    return response.data
  },

  async getById(orderNo: number): Promise<OrderHeader> {
    const response = await api.get<OrderHeader>(`${endpoint}/${orderNo}`)
    return response.data
  },

  async create(data: CreateHOrderRequest): Promise<OrderHeader> {
    const response = await api.post<OrderHeader>(`${endpoint}/`, data)
    return response.data
  },

  async delete(orderNo: number): Promise<void> {
    await api.delete(`${endpoint}/${orderNo}`)
  },

  // Order Detail
  async getDetails(orderNo: number): Promise<OrderDetail[]> {
    const response = await api.get<OrderDetail[]>(`${endpoint}/${orderNo}/details`)
    return response.data
  },

  async getDetailById(orderNo: number, goodsId: string): Promise<OrderDetail> {
    const response = await api.get<OrderDetail>(
      `${endpoint}/${orderNo}/details/${encodeURIComponent(goodsId)}`,
    )
    return response.data
  },

  async addDetail(data: CreateDOrderRequest): Promise<OrderDetail> {
    const response = await api.post<OrderDetail>(`${endpoint}/details`, data)
    return response.data
  },

  async updateDetail(
    orderNo: number,
    goodsId: string,
    data: UpdateDOrderRequest,
  ): Promise<OrderDetail> {
    const response = await api.put<OrderDetail>(
      `${endpoint}/${orderNo}/details/${encodeURIComponent(goodsId)}`,
      data,
    )
    return response.data
  },

  async deleteDetail(orderNo: number, goodsId: string): Promise<void> {
    await api.delete(`${endpoint}/${orderNo}/details/${encodeURIComponent(goodsId)}`)
  },
}
