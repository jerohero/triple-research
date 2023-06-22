<script setup lang="ts">
  import { ref } from 'vue'
  import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
  import CustomButton from '@/components/CustomButton.vue'

  const props = defineProps<{
    onConfirm: () => void,
    onCancel: () => void,
    title: string,
    content: string,
    type: string
    confirmText?: string,
  }>()

  const open = ref(true)

  let icon = ''
  let iconColor = ''

  const cancel = () => {
    props.onCancel()
  }

  const confirm = () => {
    props.onConfirm()
  }

  if (props.type == 'alert') {
    icon = 'alert-circle-outline'
    iconColor = 'text-delete'
  }
</script>

<template>
  <TransitionRoot as="template" :show="open">
    <Dialog as="div" class="relative z-10" @close="cancel">
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
                    @click="cancel"
                    type="button"
                    class="rounded-md text-textGrey hover:text-textDark focus:outline-none"
                >
                  <span class="sr-only">
                    Close
                  </span>
                  <ion-icon name="close-outline" class="h-6 w-6" aria-hidden="true"/>
                </button>
              </div>
              <div class="flex items-start">
                <div
                    class="flex flex-shrink-0 items-center justify-center rounded-full bg-backgroundDark mx-0 h-10 w-10"
                >
                  <ion-icon :name="icon" class="h-6 w-6" :class="iconColor" aria-hidden="true" />
                </div>
                <div class="mt-3 mt-0 ml-4 text-left">
                  <DialogTitle as="h3" class="text-lg font-medium leading-6 text-text">
                    {{ title }}
                  </DialogTitle>
                  <div class="mt-2">
                    <p class="text-sm text-textDark">
                      {{ content }}
                    </p>
                  </div>
                </div>
              </div>
              <div class="mt-4 flex flex-row-reverse gap-4">
                <CustomButton
                    :text="confirmText || 'Confirm'"
                    :on-click="confirm"
                    is-primary
                />
                <CustomButton
                    text="Cancel"
                    :on-click="cancel"
                />
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>
