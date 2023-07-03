<script setup lang="ts">
  import { onMounted, ref } from 'vue'
  import ModelUpload from '@/components/ModelUpload.vue'
  import ColumnModelList from '@/components/ColumnModelList.vue'
  import axios from '@/shared/axios'

  const props = defineProps<{
    projectId: number,
    rowItem?: any
  }>()

  const isUploadingModel = ref(false)
  const models = ref<any[]>([])

  const emit = defineEmits(['update', 'delete'])

  onMounted(async () => {
    await fetch()
  })

  const fetch = async () => {
    const fetchUrl = props.rowItem.edit.options.fetchUrl(props.projectId)

    if (fetchUrl) {
      const res = await axios()
          .get(fetchUrl)
      models.value = res.data

      return
    }

    models.value = props.rowItem?.edit?.options?.value
  }

  const onDelete = async (modelId: number) => {
    const res = await axios().delete(`trained-model/${ modelId }`)

    if (res.status === 200) {
      const index = models.value.indexOf((model: any) => model.id == modelId)
      models.value.splice(index, 1)
    }
  }

  const onOpenUploadModel = () => {
    isUploadingModel.value = true
  }

  const onCloseUploadModel = () => {
    isUploadingModel.value = false
  }

  const onUploadModel = async () => {
    await fetch()
  }
</script>

<template>
  <div>
    <ColumnModelList
        :row-item="rowItem"
        :models="models"
        @delete="onDelete"
        @open-upload="onOpenUploadModel"
    />
    <ModelUpload
        :col-key="rowItem.key"
        :value="rowItem.value"
        :open="isUploadingModel"
        @close="onCloseUploadModel"
        @create="onUploadModel"
        :project-id="projectId"
    />
  </div>
</template>

<style scoped>

</style>
