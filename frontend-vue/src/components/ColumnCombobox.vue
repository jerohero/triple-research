<script setup lang="ts">
import {computed, onMounted, ref, watch} from 'vue'
  import {
    Combobox,
    ComboboxButton,
    ComboboxInput,
    ComboboxOption,
    ComboboxOptions,
  } from '@headlessui/vue'
  import { useToast } from 'vue-toastification'
  import axios from '../shared/axios'

  const props = defineProps<{
    rowItem?: any, // Use for editing
    createSettings?: any, // Use for creating
    multiple?: boolean,
    maxItems?: number,
    minItems?: number
  }>()

  const settings = props.rowItem?.edit || props.createSettings

  const toast = useToast()
  const emit = defineEmits(['change'])

  const data = ref()
  const selected = ref()
  const query = ref('')
  const filtered = computed(() =>
      query.value === ''
          ? data.value
          : data.value.filter((dataItem: any) => {
            const queryable = settings.options?.queryable(dataItem)

            return queryable.toLowerCase().includes(query.value.toLowerCase())
          })
  )

  const isOverMax = computed(() => props.maxItems &&  selected.value ? selected.value.length > props.maxItems : false)
  const isUnderMin = computed(() => props.minItems && selected.value ? selected.value.length < props.minItems : false)

  onMounted(async () => {
    if (settings.options?.fetchUrl) {
      const res = await axios()
          .get(settings.options.fetchUrl)
      data.value = res.data
    } else if (settings.options?.value) {
      data.value = settings.options?.value
    }

    if (props.multiple) {
      initSelectedValuesMultiple()
      return
    }

    initSelectedValue()
  })

  const initSelectedValue = () => {
    if (!props.rowItem) {
      selected.value = data.value[0]
      emit('change', {
        key: props.rowItem?.key || props.createSettings?.key,
        value: data.value[0]
      })
      return
    }

    selected.value = data.value.find((item: any) =>
        (item?.Id || item) === (props.rowItem.value.Id || props.rowItem.value)
    )
  }

  const initSelectedValuesMultiple = () => {
    if (!props.rowItem) return

    selected.value = data.value.filter((item: any) =>
        props.rowItem.value.map((current: any) => current.Id).includes(item?.Id || item)
        || props.rowItem.value.map((current: any) => current).includes(item)
    )
  }

  watch(isUnderMin, (from, to) => {
    if (to) return // Is over min

    toast.warning(`This field has a minimum of ${ props.minItems } values!`)
  })

  watch(isOverMax, (from, to) => {
    if (to) return // Is under max

    toast.warning(`This field has a maximum of ${ props.minItems } values!`)
  })

  watch(selected, (from, to) => {
    if (!to) return // None selected

    emit('change', {
      key: props.rowItem?.key || props.createSettings?.key,
      value: selected.value
    })
  })

  const getDisplayValue = (input: any) => {
    if (!input)
      return ''

    return settings.options?.display(input)
  }
</script>

<template>
  <Combobox v-if="data" as="div" v-model="selected" :multiple="multiple">
    <div class="relative mt-1">
      <ComboboxInput
          @change="query = $event.target.value"
          :display-value="(value) => multiple ? query : getDisplayValue(value)"
          class="w-full bg-foreground border border-text text-sm rounded-md py-2 pl-3 pr-10 focus:border-indigo-500
          focus:outline-none focus:ring-1 focus:ring-indigo-500"
          :title="getDisplayValue(selected)"
      />
      <span
          v-if="multiple && !query"
          class="absolute left-0 py-2 pl-3 pr-10 text-textGrey select-none pointer-events-none whitespace-nowrap
                text-ellipsis w-full overflow-hidden"
      >
        {{ getDisplayValue(selected) }}
      </span>
      <ComboboxButton class="absolute inset-y-0 right-0 flex items-center rounded-r-md px-2 focus:outline-none">
        <ion-icon name="code-outline" class="h-4 w-4 text-gray-400 rotate-90" aria-hidden="true"/>
      </ComboboxButton>

      <ComboboxOptions
          v-if="filtered.length > 0"
          class="absolute bg-background z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white py-1 text-base
                shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm"
      >
        <ComboboxOption v-for="option in filtered"
                        v-bind:key="settings.options?.id(option)"
                        :value="option"
                        as="template"
                        v-slot="{ active, selected }"
        >
          <li class="relative cursor-pointer select-none py-2 pl-3 pr-9 hover:bg-foreground"
              :class="active ? 'bg-indigo-600 text-white' : 'text-gray-900'"
          >
            <div class="flex items-center justify-between">
              <span class="truncate" :class="selected && 'font-semibold'">
                {{ getDisplayValue(option) }}
              </span>
              <span v-if="settings.options?.displaySub" class="ml-2 mr-2 truncate text-[0.5rem] text-line">
                {{ settings.options?.displaySub(option) }}
              </span>
            </div>

            <span v-if="selected"
                  class="absolute inset-y-0 right-0 flex items-center pr-4"
                  :class="active ? 'text-white' : 'text-indigo-600'"
            >
              <ion-icon name="checkmark-outline" class="h-5 w-5" aria-hidden="true"/>
            </span>
          </li>
        </ComboboxOption>
      </ComboboxOptions>
    </div>
  </Combobox>
</template>
