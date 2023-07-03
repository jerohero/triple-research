<script setup lang="ts">
  import IconButton from '@/components/IconButton.vue'
  import { ref } from 'vue'
  import ConfirmationModal from '@/components/ConfirmationModal.vue'
  import ColumnInput from '@/components/ColumnInput.vue'
  import ColumnCombobox from '@/components/ColumnCombobox.vue'
  import ModelUpload from '@/components/ModelUpload.vue'
  import router from '@/router'
  import ColumnModelList from '@/components/ColumnModelList.vue'
  import ColumnInputList from '@/components/ColumnInputList.vue'
  import axios from "@/shared/axios";
  import ColumnModelEdit from "@/components/ColumnModelEdit.vue";

  const props = defineProps<{
    rowData: any,
    gotoPath(rowData: any): any,
    actions?: string[],
  }>()

  const isEditing = ref<boolean>()
  const isDeleting = ref<boolean>()
  let editedValue = JSON.parse(JSON.stringify(props.rowData))
  const isUploadingModel = ref(false)

  const emit = defineEmits(['update', 'delete'])

  const getRowDataWithoutId = () => {
    let { Id, ...withoutId } = props.rowData

    return withoutId
  }

  const onEdit = () => {
    isEditing.value = true
  }

  const onSave = () => {
    isEditing.value = false

    if (JSON.stringify(props.rowData) !== JSON.stringify(editedValue)) {
      emit('update', editedValue)
    }
  }

  const onGoto = () => {
    router.push({ path: props.gotoPath(props.rowData) })
  }

  const onCancel = () => {
    isEditing.value = false
    editedValue = JSON.parse(JSON.stringify(props.rowData))
  }

  const onDelete = () => {
    isDeleting.value = true
  }

  const onDeleteCancel = () => {
    isDeleting.value = false
  }

  const onDeleteConfirm = () => {
    isDeleting.value = false

    emit('delete', props.rowData)
  }

  const onChange = (emitted: { key: string, value: any }) => {
    editedValue[emitted.key].value = emitted.value
  }

  const onOpenUploadModel = () => {
    isUploadingModel.value = true
  }

  const onCloseUploadModel = () => {
    isUploadingModel.value = false
  }

  const onUploadModel = async () => {
    console.log('Upload model')
  }

  const isActionEnabled = (action: string) => {
    const index = props.actions?.indexOf(action)

    return index != undefined && index > -1
  }
</script>

<template>
  <tr class="border-b border-b-line text-sm">
    <th scope="row" class="py-5 px-6 whitespace-nowrap">
      {{ rowData.Id.display(rowData.Id.value) }}
    </th>
    <td v-for="rowItem in getRowDataWithoutId()" v-bind:key="rowItem" class="px-6">
      <span v-if="!isEditing || !rowItem.editable || rowItem.edit?.disabled" class="py-5">
        {{ rowItem.display(rowItem.value) }}
      </span>
      <!-- Do if multiple items -->
      <div v-else>
        <ColumnCombobox
            v-if="rowItem.edit?.type === 'search-multiple'"
            :row-item="rowItem"
            multiple
            :min-items="2"
            :max-items="2"
            @change="onChange"
        />
        <ColumnCombobox
            v-if="rowItem.edit?.type === 'search-single'"
            :rowItem="rowItem"
            @change="onChange"
        />
        <ColumnInput
            v-if="rowItem.edit?.type === 'input-text'"
            :col-key="rowItem.key"
            :value="rowItem.value"
            @change="onChange"
        />
        <ColumnInputList
            v-if="rowItem.edit?.type === 'input-text-list'"
            :col-key="rowItem.key"
            :value="rowItem.value"
            @change="onChange"
        />
        <div v-if="rowItem.edit?.type === 'model-upload'">
          <ColumnModelEdit :row-item="rowItem" :project-id="rowData.Id.value" />
        </div>
      </div>
    </td>
    <td class="px-6 text-xl select-none relative">
      <div v-if="isEditing" class="flex gap-1 justify-end">
        <IconButton :on-click="onSave" is-save class="text-3xl" />
        <IconButton :on-click="onCancel" is-cancel class="text-3xl" />
      </div>
      <div v-else class="flex gap-3 justify-end">
        <IconButton v-if="isActionEnabled('edit')" :on-click="onEdit" is-edit class="text-2xl" />
        <IconButton v-if="isActionEnabled('delete')" :on-click="onDelete" is-delete class="text-2xl" />
        <IconButton v-if="isActionEnabled('goto')" :on-click="onGoto" is-goto class="text-3xl" />
      </div>
    </td>
  </tr>
  <ConfirmationModal
      v-if="isDeleting"
      :onCancel="onDeleteCancel"
      :onConfirm="onDeleteConfirm"
      title="Delete row"
      type="alert"
      content="Are you sure you want to delete this row? This action cannot be undone."
      confirm-text="Delete"
  />
</template>

<style scoped>

</style>
