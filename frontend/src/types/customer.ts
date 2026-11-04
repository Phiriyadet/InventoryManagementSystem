export interface Customer {
  cusId: string
  cusName: string
}

export interface CreateCustomerRequest {
  cusId: string
  cusName: string
}

export interface UpdateCustomerRequest {
  cusName: string
}
