<script setup lang="ts">
  import { RouterView } from 'vue-router'
  import NavHeader from './components/NavHeader.vue'
</script>

<template>
  <NavHeader/>
  <div
      class="text-text min-h-screen pt-[var(--header-height)]"
  >
    <RouterView/>
  </div>
</template>

<style scoped>
</style>
