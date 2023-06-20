<script setup lang="ts">
  const props = defineProps<{
    onClick: () => void,
    isEdit?: boolean,
    isDelete?: boolean,
    isSave?: boolean,
    isCancel?: boolean
  }>()

  let icon = '';
  let color = '';

  if (props.isEdit) {
    icon = 'pencil-outline'
    color = 'group-hover:text-edit'
  } else if (props.isDelete) {
    icon = 'trash-outline'
    color = 'group-hover:text-delete'
  } else if (props.isSave) {
    icon = 'checkmark-outline'
    color = 'group-hover:text-save'
  } else if (props.isCancel) {
    icon = 'close-outline'
    color = 'group-hover:text-delete'
  }
</script>

<template>
  <div @click="onClick" class="group cursor-pointer flex justify-center items-center">
    <ion-icon :name="icon"
              class="transition-colors pointer-events-none"
              :class="color"
              aria-hidden="true"
    />
  </div>
</template>
