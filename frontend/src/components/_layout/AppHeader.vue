<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const pageTitle = computed(() => {
  return route.meta.title ?? 'แบบทดสอบ Programmer'
})
</script>

<template>
  <header class="app-header">
    <div>
      <h1>{{ pageTitle }}</h1>
      <span>Inventory Management System</span>
    </div>

    <div class="header-user">
      <div class="user-avatar">
        <i class="pi pi-user"></i>
      </div>

      <div class="user-info">
        <strong>Administrator</strong>
        <small>System User</small>
      </div>
    </div>
  </header>
</template>
