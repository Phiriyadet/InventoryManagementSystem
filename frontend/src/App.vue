<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppSidebar from './components/layout/AppSidebar.vue'
import AppHeader from './components/layout/AppHeader.vue'
</script>

<template>
  <div class="app-layout">
    <AppSidebar />

    <main class="main-container">
      <AppHeader />

      <div class="page-content">
        <RouterView />
      </div>
    </main>
  </div>
</template>
