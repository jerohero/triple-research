<script setup lang="ts">
  import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
  import ColumnCombobox from '@/components/ColumnCombobox.vue'
  import CustomButton from '@/components/CustomButton.vue'
  import ColumnInput from '@/components/ColumnInput.vue'

  defineProps<{
    open: boolean,
    createSettings: any
  }>()

  const emit = defineEmits(['close', 'create'])

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
                    <div class="items-start border-t border-line pt-5">
                      <div
                          v-for="field in Object.keys(createSettings)"
                          v-bind:key="field"
                          class="my-2"
                      >
                        <div v-if="!createSettings[field].staticValue">
                          <label for="last-name" class="block text-sm font-medium text-gray-700 mt-px pt-2 mb-1">
                            {{ createSettings[field].label }}
                          </label>
                          <div class="mt-1 sm:col-span-2 sm:mt-0">
                            <ColumnInput
                                v-if="createSettings[field].type === 'input-text'"
                                :col-key="createSettings[field].key"
                                @change="onChange"
                            />
                            <ColumnCombobox
                                v-if="createSettings[field].type === 'search-single'"
                                :create-settings="createSettings[field]"
                                @change="onChange"
                            />
                          </div>
                        </div>
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