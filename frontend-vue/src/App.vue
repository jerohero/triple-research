<script setup lang="ts">
  import { RouterView } from 'vue-router'
  import NavHeader from './components/NavHeader.vue'
  import NavSidebar from '@/components/NavSidebar.vue'
</script>

<template>
  <NavHeader/>
  <NavSidebar/>
  <div
      class="text-text min-h-screen pl-[var(--sidebar-width)] pt-[var(--header-height)]"
  >
    <RouterView/>
  </div>
</template>

<style scoped>
</style>
