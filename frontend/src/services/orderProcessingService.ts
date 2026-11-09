import api from './api'

const endpoint = '/order-processing'

interface ProcessOrdersResponse {
  processedCount: number
}

export const orderProcessingService = {
  async processOrders(startDate: string, endDate: string): Promise<number> {
    const response = await api.post<ProcessOrdersResponse>(`${endpoint}/process`, null, {
      params: {
        startDate,
        endDate,
      },
    })

    return response.data.processedCount
  },
}
