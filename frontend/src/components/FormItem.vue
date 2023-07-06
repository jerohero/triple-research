<script setup lang="ts">
  defineProps<{
    label: string,
    name: string,
    type: string,
    autocomplete?: string,
    value: string
  }>()

  const emit = defineEmits(['update:value'])

  const onInput = (input: any) => {
    emit('update:value', input.target.value)
  }
</script>

<template>
  <div>
    <label :for="name" class="block text-base font-medium text-gray-700">
      {{ label }}
    </label>
    <div class="mt-1">
      <input :id="name" :name="name" :type="type" :autocomplete="autocomplete" required
             v-on:input="onInput"
             class="block w-full appearance-none rounded-md border border-gray-300 px-3 py-2 text-background
                    shadow-sm focus:outline-none text-sm"
      />
    </div>
  </div>
</template>
