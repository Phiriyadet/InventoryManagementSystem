import api from './api'
import type { Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../types/customer'

const endpoint = '/customers'

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const response = await api.get<Customer[]>(`${endpoint}/`)
    return response.data
  },

  async getById(id: string): Promise<Customer> {
    const response = await api.get<Customer>(`${endpoint}/${id}`)
    return response.data
  },

  async create(data: CreateCustomerRequest): Promise<Customer> {
    const response = await api.post<Customer>(`${endpoint}/`, data)
    return response.data
  },

  async update(id: string, data: UpdateCustomerRequest): Promise<Customer> {
    const response = await api.put<Customer>(`${endpoint}/${id}`, data)
    return response.data
  },

  async delete(id: string): Promise<void> {
    await api.delete(`${endpoint}/${id}`)
  },
}
