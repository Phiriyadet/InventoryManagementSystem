import api from './api'
import type { Goods, CreateGoodsRequest, UpdateGoodsRequest } from '../types/goods'

const endpoint = '/goods'

export const goodsService = {
  async getAll(): Promise<Goods[]> {
    const response = await api.get<Goods[]>(`${endpoint}/`)
    return response.data
  },

  async getById(id: string): Promise<Goods> {
    const response = await api.get<Goods>(`${endpoint}/${id}`)
    return response.data
  },

  async create(data: CreateGoodsRequest): Promise<Goods> {
    const response = await api.post<Goods>(`${endpoint}/`, data)
    return response.data
  },

  async update(id: string, data: UpdateGoodsRequest): Promise<Goods> {
    const response = await api.put<Goods>(`${endpoint}/${id}`, data)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`${endpoint}/${id}`)
  },
}
