<script setup lang="ts">
  defineProps<{
    text: string,
    fullWidth?: boolean,
    onClick?: () => void,
    isPrimary?: boolean
  }>()
</script>

<template>
  <button type="submit"
          @click="onClick"
          class="inline-flex w-full justify-center rounded-md px-4 border
          py-2 text-base font-medium text-white focus:outline-none sm:w-auto sm:text-sm"
          :class="`${ fullWidth && 'w-full' } ${ isPrimary ? 'bg-primary hover:bg-primaryDark' : 'bg-backgroundDark hover:bg-background' }`"
  >
    {{ text }}
  </button>
</template>