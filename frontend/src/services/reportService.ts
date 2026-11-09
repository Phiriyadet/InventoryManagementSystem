import api from './api'
import type { OverdueOrderReport } from '../types/report'

const endpoint = '/reports'

export const reportService = {
  async getReport(startDate: string, endDate: string): Promise<OverdueOrderReport> {
    const response = await api.get<OverdueOrderReport>(`${endpoint}/overdue-orders`, {
      params: {
        startDate,
        endDate,
      },
    })

    return response.data
  },
}
