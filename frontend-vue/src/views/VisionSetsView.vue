<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import EntityContent from '@/components/EntityContent.vue'
import type { Column } from '@/shared/interfaces'
import { useRoute } from 'vue-router'

interface VisionSetColumns {
  Id: Column,
  Name: Column,
  Sources: Column,
  ContainerImage: Column,
  TrainedModel: Column
}

const router = useRoute()

const title = 'Vision Sets'

const columns = [
  'Name',
  'Sources',
  'Docker image',
  'Model'
]
const route = `/project/${ router.params.id }/vision-set`
const updateRoute = '/vision-set'
const gotoPath = (rowData: VisionSetColumns) => `/vision-sets/${ rowData.Id.value }`

const actions = [
  'edit', 'delete', 'goto'
]

const columnInputs = {
  Name: {
    type: 'input-text'
  },
  Sources: {
    type: 'input-text-list'
  },
  ContainerImage: {
    type: 'input-text'
  },
  TrainedModel: {
    type: 'search-single',
    options: {
      id: (model: any) => model.Id,
      fetchUrl: `/project/${ router.params.id }/trained-model`,
      display: (model: any) => model.Name,
      queryable: (model: any) => model?.Name
    }
  },
}

const createSettings = {
  Name: {
    key: 'Name',
    label: 'Name',
    ...columnInputs.Name
  },
  Sources: {
    key: 'Sources',
    label: 'Sources',
    ...columnInputs.Sources
  },
  ContainerImage: {
    key: 'ContainerImage',
    label: 'Docker image',
    ...columnInputs.ContainerImage
  },
  TrainedModel: {
    key: 'TrainedModel',
    label: 'Model',
    ...columnInputs.TrainedModel
  },
}

const getUpdateObject = (updated: any) => {
  const { Id, Name, Sources, ContainerImage, TrainedModel } = updated

  return {
    id: Id.value,
    name: Name.value,
    sources: Sources.value,
    containerImage: ContainerImage.value,
    trainedModelId: TrainedModel.value.Id
  }
}

const getCreateObject = (created: any) => {
  return {
    projectId: router.params.id,
    name: created.Name,
    sources: created.Sources,
    containerImage: created.ContainerImage,
    trainedModelId: created.TrainedModel.Id
  }
}

const getRowObject = (project: any): VisionSetColumns => {
  return {
    Id: {
      key: 'Id',
      display: (id: string) => id,
      value: project.Id,
      editable: true,
      queryable: true,
    },
    Name: {
      key: 'Name',
      display: (name: string) => name,
      value: project.Name,
      editable: true,
      queryable: true,
      edit: columnInputs.Name
    },
    Sources: {
      key: 'Sources',
      display: (sources: string) => `${ sources.length.toString() } source${ sources.length !== 1 ? 's' : '' }`,
      value: project.Sources,
      editable: true,
      queryable: true,
      edit: columnInputs.Sources
    },
    ContainerImage: {
      key: 'ContainerImage',
      display: (containerImage: string) => containerImage,
      value: project.ContainerImage,
      editable: true,
      queryable: true,
      edit: columnInputs.ContainerImage
    },
    TrainedModel: {
      key: 'TrainedModel',
      display: (trainedModel: any) => `${ trainedModel?.Name }`,
      value: project.TrainedModel,
      editable: true,
      queryable: true,
      edit: columnInputs.TrainedModel
    },
  }
}
</script>

<template>
  <div class="mx-10 my-7 min-h-full">
    <EntityTitle
        :title="title"
    />
    <EntityContent
        :columns="columns"
        :route="route"
        :goto-path="gotoPath"
        :update-route="updateRoute"
        :get-row-object="getRowObject"
        :get-update-object="getUpdateObject"
        :get-create-object="getCreateObject"
        :create-settings="createSettings"
        :actions="actions"
    />
  </div>
</template>

<style>
</style>