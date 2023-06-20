<script setup lang="ts">
  const props = defineProps<{
    colKey: string, // "Key" is a reserved prop
    value?: any
  }>()

  const emit = defineEmits(['change'])

  const onInput = (input: any) => {
    emit('change', {
      key: props.colKey,
      value: input.target.value
    })
  }
</script>

<template>
  <input
      type="text"
      :value="props.value"
      v-on:input="onInput"
      :name="props.colKey"
      :id="props.colKey"
      required
      class="block w-full h-full px-3 py-2 rounded-md bg-foreground border border-text text-base
             focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
  >
</template>