<script setup lang="ts">
  import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
  import {ref} from 'vue'
  import axios from '@/shared/axios'

  const props = defineProps<{
    open: boolean,
    colKey: string, // "Key" is a reserved prop
    projectId?: number,
    value?: any,
  }>()

  const emit = defineEmits(['change', 'close', 'create'])

  const selectedFile = ref<any>(null)
  const uploadProgress = ref<number>(0)

  let newObject: any = {}

  const onClose = () => {
    emit('close')
    newObject = {}
  }

  const onChange = (emitted: { key: string, value: any }) => {
    newObject[emitted.key] = emitted.value
  }

  const onSave = () => {
    emit('create', newObject)
  }

  const onFileChange = (e: any) => {
    selectedFile.value = e.target.files ? e.target.files[0] : null
  }

  const uploadChunk: any = async (chunk: any, start: number, end: number, retries = 3) => {
    try {
      await axios().post(`project/${props.projectId}/trained-model`, chunk, {
        headers: {
          "Content-Type": "application/octet-stream",
          "x-chunk-metadata": JSON.stringify({ name: selectedFile.value.name, size: selectedFile.value.size })
        }
      })
      const nextChunkStart = start + chunk.byteLength
      uploadProgress.value = Math.min(Math.floor((nextChunkStart / selectedFile.value.size) * 100), 100)
      return end
    } catch (error: any) {
      if (retries > 0 && error.response.status === 500) {
        console.error("File upload failed, retrying...", error)
        await new Promise(r => setTimeout(r, 1000))
        return uploadChunk(chunk, start, end, retries - 1)
      } else {
        throw error
      }
    }
  }

  const onFileUpload = async () => {
    if (!selectedFile.value) return

    const chunkSize = 1024 * 1024 * 5 // 5MB
    let start = 0

    while (start < selectedFile.value.size) {
      const end = Math.min(start + chunkSize, selectedFile.value.size)
      const chunk = await selectedFile.value.slice(start, end).arrayBuffer()
      start = await uploadChunk(chunk, start, end)
    }
  }
</script>

<template>
  <TransitionRoot as="template" :show="open">
    <Dialog as="div" class="relative z-10" @close="onClose">
      <TransitionChild
          as="template"
          enter="ease-out duration-300"
          enter-from="opacity-0"
          enter-to="opacity-100"
          leave="ease-in duration-200"
          leave-from="opacity-100"
          leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-background bg-opacity-20 transition-opacity" />
      </TransitionChild>

      <div class="fixed inset-0 z-10 overflow-y-auto bg-background bg-opacity-40">
        <div class="flex min-h-full justify-center text-center items-center p-0">
          <TransitionChild
              as="template"
              enter="ease-out duration-300"
              enter-from="opacity-0 translate-y-0 scale-95"
              enter-to="opacity-100 translate-y-0 scale-100"
              leave="ease-in duration-200"
              leave-from="opacity-100 translate-y-0 scale-100"
              leave-to="opacity-0 translate-y-4 translate-y-0 scale-95"
          >
            <DialogPanel
                class="relative bg-foreground transform overflow-hidden rounded-lg
                text-left shadow-xl transition-all my-8 w-full max-w-lg p-6"
            >
              <div class="absolute top-0 right-0 hidden pt-4 pr-4 sm:block">
                <button
                    @click="onClose"
                    type="button"
                    class="rounded-md text-textGrey hover:text-textDark focus:outline-none"
                >
                  <span class="sr-only">
                    Close
                  </span>
                  <ion-icon name="close-outline" class="h-6 w-6" aria-hidden="true"/>
                </button>
              </div>
              <div>
                <DialogTitle as="h3" class="text-lg font-medium leading-6 text-text">
                  Upload model
                </DialogTitle>
                <div class="space-y-5 pt-3 w-full">
                  <form class="flex flex-col justify-between" onsubmit="return false">
                    <div>

                      <div>
                        <div class="flex items-center justify-center w-full">
                          <label
                              v-if="!selectedFile?.value"
                              for="dropzone-file"
                              class="flex flex-col items-center justify-center w-full h-64 border-2 border-gray-300
                              border-dashed rounded-lg cursor-pointer bg-gray-50 dark:hover:bg-bray-800 dark:bg-gray-700
                                hover:bg-gray-100 dark:border-gray-600 dark:hover:border-gray-500 dark:hover:bg-gray-600"
                          >
                            <div class="flex flex-col items-center justify-center pt-5 pb-6">
                              <svg aria-hidden="true" class="w-10 h-10 mb-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"></path>
                              </svg>
                              <p class="mb-2 text-sm text-gray-500 dark:text-gray-400">
                                <span class="font-semibold">Click to upload</span> or drag and drop
                              </p>
<!--                              <p class="text-xs text-gray-500 dark:text-gray-400">Accepted:</p>-->
                            </div>
                            <input id="dropzone-file" type="file" class="hidden" @change="onFileChange" />
                          </label>
                          <div v-else>
                            
                          </div>
                        </div>

                        <button @click="onFileUpload">Upload!</button>
                        <div>Upload Progress: {{uploadProgress}}%</div>
                      </div>

                    </div>
                    <div class="mt-4 flex flex-row-reverse gap-4 mt-48">
                      <CustomButton
                          @click="onSave"
                          text="Add"
                          :on-click="onSave"
                          is-primary
                      />
                      <CustomButton
                          @click="onClose"
                          text="Cancel"
                      />
                    </div>
                  </form>
                </div>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>