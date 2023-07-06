import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Ref } from 'vue'

interface State {
  query: Ref<string>
}

export const useTableStore = defineStore('table', (): State => {
  const query = ref<string>('')

  return {
    query
  }
})
