import { createRouter, createWebHistory } from 'vue-router'

import GoodsView from '../views/GoodsView.vue'
import CustomersView from '../views/CustomersView.vue'
import OrdersView from '../views/OrdersView.vue'
import ReportsView from '../views/ReportsView.vue'
import ProcessingView from '@/views/ProcessingView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),

  routes: [
    {
      path: '/goods',
      name: 'goods',
      component: GoodsView,
      meta: { title: 'Goods Management' },
    },
    {
      path: '/customers',
      name: 'customers',
      component: CustomersView,
      meta: { title: 'Customer Management' },
    },
    {
      path: '/orders',
      name: 'orders',
      component: OrdersView,
      meta: { title: 'Order Management' },
    },
    {
      path: '/reports',
      name: 'reports',
      component: ReportsView,
      meta: { title: 'Reports' },
    },
    {
      path: '/processing',
      name: 'processing',
      component: ProcessingView,
      meta: { title: 'Processing' },
    },
  ],
})

export default router
