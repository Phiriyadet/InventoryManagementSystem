<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'

const confirm = useConfirm()

const isCollapsed = ref(false)

const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value
}

const handleLogout = () => {
  confirm.require({
    message: 'คุณต้องการออกจากเว็บไซต์ใช่หรือไม่?',
    header: 'ยืนยันการออกจากเว็บไซต์',
    icon: 'pi pi-exclamation-triangle',
    rejectLabel: 'ยกเลิก',
    acceptLabel: 'ออกจากเว็บไซต์',
    rejectClass: 'p-button-secondary',
    acceptClass: 'p-button-danger',
    accept: () => {
      window.close()
    },
  })
}

const menuGroups = [
  {
    label: '1. ฐานข้อมูลอ้างอิง',
    icon: 'pi pi-database',
    items: [
      {
        label: '1.1 บันทึก/แก้ไข ข้อมูลลูกค้า',
        icon: 'pi pi-users',
        to: '/customers',
      },
      {
        label: '1.2 บันทึก/แก้ไข ข้อมูลสินค้า',
        icon: 'pi pi-box',
        to: '/goods',
      },
    ],
  },
  {
    label: '2. การทำงานประจำวัน',
    icon: 'pi pi-calendar',
    items: [
      {
        label: '2.1 บันทึก/แก้ไข การสั่งซื้อสินค้า',
        icon: 'pi pi-shopping-cart',
        to: '/orders',
      },
      {
        label: '2.2 การประมวลผลข้อมูลการสั่งสินค้า',
        icon: 'pi pi-cog',
        to: '/processing',
      },
    ],
  },
  {
    label: '3. รายงาน',
    icon: 'pi pi-chart-bar',
    items: [
      {
        label: '3.1 รายงานกำหนดส่งสินค้า',
        icon: 'pi pi-file',
        to: '/reports',
      },
    ],
  },
]
</script>

<template>
  <ConfirmDialog />

  <aside :class="['sidebar', { collapsed: isCollapsed }]">
    <!-- Brand -->
    <div class="sidebar-brand">
      <div class="brand-icon">
        <i class="pi pi-warehouse"></i>
      </div>

      <div v-if="!isCollapsed" class="brand-text">
        <h2>KKF Inventory</h2>
        <span>Management System</span>
      </div>

      <button
        type="button"
        class="sidebar-toggle"
        :title="isCollapsed ? 'ขยายเมนู' : 'ยุบเมนู'"
        @click="toggleSidebar"
      >
        <i :class="isCollapsed ? 'pi pi-angle-right' : 'pi pi-angle-left'"></i>
      </button>
    </div>

    <!-- Navigation -->
    <div class="sidebar-section">
      <span v-if="!isCollapsed" class="section-label"> ระบบโปรแกรมบริหารจัดการสินค้าคงคลัง </span>

      <nav class="sidebar-nav">
        <div v-for="group in menuGroups" :key="group.label" class="nav-group">
          <div v-if="!isCollapsed" class="nav-group-title">
            <i :class="group.icon"></i>
            <span>{{ group.label }}</span>
          </div>

          <RouterLink
            v-for="item in group.items"
            :key="item.to"
            :to="item.to"
            class="nav-item"
            active-class="active"
            :title="isCollapsed ? item.label : undefined"
          >
            <i :class="item.icon"></i>

            <span v-if="!isCollapsed">
              {{ item.label }}
            </span>
          </RouterLink>
        </div>
      </nav>
    </div>

    <!-- Footer -->
    <div class="sidebar-footer">
      <!-- Logout -->
      <button
        type="button"
        class="nav-item logout-button"
        title="ออกจากเว็บไซต์"
        @click="handleLogout"
      >
        <i class="pi pi-sign-out"></i>

        <span v-if="!isCollapsed"> 4. ออกจากระบบ </span>
      </button>

      <!-- Version -->
      <div v-if="!isCollapsed" class="footer-info">
        <span>KKF Inventory System</span>
        <small>Version 1.0.0</small>
      </div>
    </div>
  </aside>
</template>

<style scoped>
/* Sidebar */
.sidebar {
  width: 250px;
  min-width: 250px;
  background: var(--sidebar-background);
  color: #fff;
  display: flex;
  flex-direction: column;
  padding: 24px 16px;
  transition:
    width 0.25s ease,
    min-width 0.25s ease,
    padding 0.25s ease;
}

.sidebar.collapsed {
  width: 76px;
  min-width: 76px;
  padding: 24px 10px;
}

/* Brand */
.sidebar-brand {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 8px 32px;
}

.brand-icon {
  width: 42px;
  min-width: 42px;
  height: 42px;
  border-radius: var(--radius-lg);
  background: #3b82f6;
  display: grid;
  place-items: center;
  font-size: 20px;
}

.brand-text {
  flex: 1;
  min-width: 0;
  white-space: nowrap;
  overflow: hidden;
}

.brand-text h2 {
  font-size: 16px;
  margin: 0;
  font-weight: 600;
}

.brand-text span {
  font-size: 11px;
  color: var(--sidebar-text);
}

.sidebar-toggle {
  border: none;
  background: transparent;
  color: var(--sidebar-text);
  cursor: pointer;
  border-radius: var(--radius-sm);
  padding: 6px;
  font-size: 16px;
  display: grid;
  place-items: center;
}

.sidebar-toggle:hover {
  background: var(--sidebar-hover);
  color: #fff;
}

/* Collapsed Brand */
.sidebar.collapsed .sidebar-brand {
  flex-direction: column;
  padding: 0 0 32px;
  gap: 12px;
}

/* Section */
.section-label {
  display: block;
  padding: 0 12px 12px;
  font-size: 11px;
  color: #94a3b8;
  letter-spacing: 1px;
  white-space: nowrap;
}

/* Navigation */
.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.nav-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-bottom: 10px;
}

.nav-group-title {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  font-size: 13px;
  color: #94a3b8;
  white-space: nowrap;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-radius: var(--radius-md);
  color: var(--sidebar-text);
  text-decoration: none;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  transition: background 0.2s;
  cursor: pointer;
}

.nav-item i {
  min-width: 18px;
  text-align: center;
}

.nav-item:hover {
  background: var(--sidebar-hover);
  color: #fff;
}

.nav-item.active {
  background: var(--color-primary);
  color: #fff;
}

/* Collapsed Navigation */
.sidebar.collapsed .nav-item {
  justify-content: center;
  padding: 12px 0;
}

.sidebar.collapsed .nav-item i {
  font-size: 18px;
}

/* Footer */
.sidebar-footer {
  margin-top: auto;
  padding: 20px 8px 0;
  border-top: 1px solid #334155;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.footer-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 12px;
  color: var(--sidebar-text);
}

.footer-info small {
  color: #94a3b8;
}

/* Logout */
.logout-button {
  width: 100%;
  border: none;
  background: transparent;
  text-align: left;
  font-family: inherit;
}

.logout-button:hover {
  background: var(--color-danger-hover);
  color: #fff;
}

/* Collapsed Footer */
.sidebar.collapsed .sidebar-footer {
  padding-left: 0;
  padding-right: 0;
}

.sidebar.collapsed .logout-button {
  justify-content: center;
}
</style>
