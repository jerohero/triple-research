<script setup lang="ts">
  import { RouterView } from 'vue-router'
  import NavHeader from './components/NavHeader.vue'
  import NavSidebar from '@/components/NavSidebar.vue'
  import { useUserStore } from '@/stores/user'

  const userRoute = useUserStore()
</script>

<template>
  <div v-if="!userRoute.inAuth">
    <NavHeader/>
    <NavSidebar/>
  </div>
  <div
      class="text-text min-h-screen"
      :class="!userRoute.inAuth && 'pl-[var(--sidebar-width)] pt-[var(--header-height)]'"
  >
    <RouterView/>
  </div>
</template>

<style scoped>
</style>
