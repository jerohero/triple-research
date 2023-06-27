<script setup lang="ts">
  import { ref } from 'vue'

  const props = defineProps<{
    colKey: string, // "Key" is a reserved prop
    value?: any
  }>()

  const emit = defineEmits(['change'])
  const newSources = ref<string[]>(JSON.parse(JSON.stringify(props.value)))
  const inputValue = ref<string>()

  const removeSource = (sourceToRemove: string) => {
    newSources.value = newSources.value
        .filter(source => source !== sourceToRemove)

    emit('change', {
      key: props.colKey,
      value: newSources.value
    })
  }

  const addSource = (e: any) => {
    e.preventDefault()

    if (!inputValue.value) {
      return
    }

    newSources.value.push(inputValue.value)
    inputValue.value = ''

    emit('change', {
      key: props.colKey,
      value: newSources.value
    })
  }
</script>

<template>
  <div>
    <div
        v-for="source in newSources"
        :key="source + Math.random()"
        class="flex space-x-2"
    >
      <p class="text-xs">
        {{ source }}
      </p>
      <button>
        <ion-icon name="close-outline"
                  class="h-4 w-4 text-text hover:text-delete"
                  aria-hidden="true"
                  @click="removeSource(source)"
        />
      </button>
    </div>
    <form class="flex w-full space-x-2" :onsubmit="addSource">
      <input
          type="text"
          v-on:input="inputValue = $event.target.value"
          :name="props.colKey"
          :id="props.colKey"
          :value="inputValue"
          required
          class="w-full h-full px-3 py-2 rounded-md bg-foreground border border-text text-base
             focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
      >
      <button type="submit"
              class="inline-flex w-full justify-center rounded-md px-4 border
          py-2 text-base font-medium text-white focus:outline-none sm:w-auto sm:text-sm bg-primary hover:bg-primaryDark"
      >
        Add
      </button>
    </form>
  </div>
</template>