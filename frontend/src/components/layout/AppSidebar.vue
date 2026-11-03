<script setup lang="ts">
import { RouterLink } from 'vue-router'

const menuItems = [
  {
    label: 'Dashboard',
    icon: 'pi pi-home',
    to: '/',
  },
  {
    label: 'Goods',
    icon: 'pi pi-box',
    to: '/goods',
  },
  {
    label: 'Customers',
    icon: 'pi pi-users',
    to: '/customers',
  },
  {
    label: 'Orders',
    icon: 'pi pi-shopping-cart',
    to: '/orders',
  },
  {
    label: 'Reports',
    icon: 'pi pi-chart-bar',
    to: '/reports',
  },
]
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-brand">
      <div class="brand-icon">
        <i class="pi pi-warehouse"></i>
      </div>

      <div>
        <h2>KKF Inventory</h2>
        <span>Management System</span>
      </div>
    </div>

    <div class="sidebar-section">
      <span class="section-label">MAIN MENU</span>

      <nav class="sidebar-nav">
        <RouterLink
          v-for="item in menuItems"
          :key="item.to"
          :to="item.to"
          class="nav-item"
          exact-active-class="active"
        >
          <i :class="item.icon"></i>
          <span>{{ item.label }}</span>
        </RouterLink>
      </nav>
    </div>

    <div class="sidebar-footer">
      <span>KKF Inventory System</span>
      <small>Version 1.0.0</small>
    </div>
  </aside>
</template>
