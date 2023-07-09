<script setup lang="ts">
  defineProps<{
    title: string,
    sub?: string
  }>()
</script>

<template>
  <div class="flex">
    <h2 class="text-text text-4xl font-bold mr-3">
      {{ title }}
    </h2>
    <div v-if="sub" class="my-auto">
      <span class="text-text text-xs px-1.5 py-1 rounded inline bg-primary">
        {{ sub }}
      </span>
    </div>
  </div>
</template>

<style scoped>

</style>
