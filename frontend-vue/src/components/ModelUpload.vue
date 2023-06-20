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

  const selectedFile = ref()
  const uploadProgress = ref()

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

  const onFileChange = (e: any) => selectedFile.value = (e.target.files ? e.target.files[0] : null)

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

  const uploadChunk: any = async (chunk: any, start: number, end: number, retries = 3) => {
    try {
      await axios().post(`project/${props.projectId}/trained-model`, chunk, {
        headers: {
          "Content-Type": "application/octet-stream",
          "x-chunk-metadata": JSON.stringify({ name: selectedFile.value.name, size: selectedFile.value.size })
        }
      })
      const nextChunkStart = start + chunk.byteLength
      uploadProgress.value = (Math.min(Math.floor((nextChunkStart / selectedFile.value.size) * 100), 100))
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
                  Add new
                </DialogTitle>
                <div class="space-y-5 pt-3 w-full">
                  <form class="flex flex-col justify-between" onsubmit="return false">
                    <div>

                      <input type="file" onChange={onFileChange} />
                      <button onClick={onFileUpload}>Upload!</button>
                      <div>Upload Progress: {uploadProgress}%</div>

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