import { createRouter, createWebHistory } from 'vue-router'

import DashboardView from '../views/DashboardView.vue'
import GoodsView from '../views/GoodsView.vue'
import CustomersView from '../views/CustomersView.vue'
import OrdersView from '../views/OrdersView.vue'
import ReportsView from '../views/ReportsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),

  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { title: 'Dashboard' },
    },
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
  ],
})

export default router
