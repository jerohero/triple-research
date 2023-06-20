<script setup lang="ts">
  import {useUserStore} from '@/stores/user'

  defineProps<{
    title: string
  }>()

  const userStore = useUserStore()

  const organization = userStore.isSuperAdmin ? 'Super Admin' : userStore.user.organization?.name
</script>

<template>
  <div class="flex">
    <h2 class="text-text text-4xl font-bold mr-3">
      {{ title }}
    </h2>
    <div class="my-auto">
      <span class="text-text text-xs px-1.5 py-1 rounded inline" :class="userStore.isSuperAdmin ? 'bg-primary' : 'bg-secondary'">
        {{ organization }}
      </span>
    </div>
  </div>
</template>

<style scoped>

</style>
