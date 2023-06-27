<script setup lang="ts">
  import {onMounted, ref, watch} from 'vue'
  import { useToast } from 'vue-toastification'
  import axios from '../shared/axios'
  import ConfirmationModal from '@/components/ConfirmationModal.vue'

  const props = defineProps<{
    rowItem?: any, // Use for editing
    createSettings?: any, // Use for creating
    multiple?: boolean,
    maxItems?: number,
    minItems?: number
  }>()

  const settings = props.rowItem?.edit || props.createSettings

  const toast = useToast()
  const emit = defineEmits(['change', 'openUpload', 'delete'])

  const modelIdToDelete = ref<number>()
  const data = ref()
  const selected = ref()

  onMounted(async () => {
    if (settings.options?.fetchUrl) {
      const res = await axios()
          .get(settings.options.fetchUrl)
      data.value = res.data
    } else if (settings.options?.value) {
      data.value = settings.options?.value
    }
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

  const getDisplayValue = (input: any) => {
    if (!input)
      return ''

    return settings.options?.display(input)
  }

  const deleteModel = (model: any) => {
    modelIdToDelete.value = model.Id;
  }

  const onDeleteCancel = () => {
    modelIdToDelete.value = 0
  }

  const onDeleteConfirm = () => {
    modelIdToDelete.value = 0

    emit('delete', modelIdToDelete)
  }

  const onOpenUploadModel = () => {
    emit('openUpload')
  }
</script>

<template>
  <div>
    <div class="flex-wrap">
      <div
          v-for="model in rowItem.value"
          :key="model.Id"
          class="bg-secondary rounded-2xl py-2 px-3 text-xs w-fit inline-block mr-2 my-1"
      >
        <div class="flex space-x-2">
          <p>
            {{ getDisplayValue(model) }}
          </p>
          <button @click="deleteModel(model)">
            <ion-icon name="close-outline"
                      class="h-4 w-4 text-text hover:text-delete"
                      aria-hidden="true"
            />
          </button>
        </div>
      </div>
      <button @click="onOpenUploadModel" class="h-full bg-primary rounded-2xl py-2 px-3 text-xs w-fit inline-block mr-2 my-1">
        <div class="flex">
          <p class="mr-1">
            Add
          </p>
          <ion-icon name="add-outline"
                    class="h-4 w-4 text-text"
                    aria-hidden="true"
          />
        </div>
      </button>
    </div>
    <ConfirmationModal
        v-if="modelIdToDelete > 0"
        :onCancel="onDeleteCancel"
        :onConfirm="onDeleteConfirm"
        title="Delete model"
        type="alert"
        content="Are you sure you want to delete this model? This action cannot be undone."
        confirm-text="Delete"
    />
  </div>
</template>
