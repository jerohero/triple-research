<script setup lang="ts">
  import { ref } from 'vue'
  import { useToast } from 'vue-toastification'
  import ConfirmationModal from '@/components/ConfirmationModal.vue'

  const props = defineProps<{
    models: any,
    rowItem?: any, // Use for editing
    deleteModel?(model: any): any,
  }>()

  const toast = useToast()
  const emit = defineEmits(['delete', 'openUpload'])

  const modelIdToDelete = ref<number>()
  const data = ref()

  const getDisplayValue = (input: any) => {
    if (!input)
      return ''

    return props.rowItem?.edit?.options?.display(input)
  }

  const deleteModel = (model: any) => {
    modelIdToDelete.value = model.Id;
  }

  const onDeleteCancel = () => {
    modelIdToDelete.value = 0
  }

  const onDeleteConfirm = async () => {
    emit('delete', modelIdToDelete.value)
    modelIdToDelete.value = 0
  }

  const onOpenUploadModel = () => {
    emit('openUpload')
  }
</script>

<template>
  <div>
    <div class="flex-wrap">
      <div
          v-for="model in models"
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
