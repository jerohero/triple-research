<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import EntityContent from '@/components/EntityContent.vue'
import type { Column } from '@/shared/interfaces'

interface ProjectColumns {
  Id: Column,
  Name: Column,
  Models: Column
}

const title = 'Projects'

const columns = [
  'Name',
  'Models'
]
const route = '/project'

const columnInputs = {
  Name: {
    type: 'input-text'
  },
  Models: {
    type: 'model-upload',
    options: {
      id: (model: any) => model.Id,
      fetchUrl: (project: any) => `/project/${ project.Id }/trained-model`,
      display: (models: any) =>
          Array.isArray(models)
            ? models?.map((model: any) => `${model?.Name}`).join(', ')
            : `${models?.Name}`,
      queryable: (model: any) => `${ model?.Name }`
    }
  }
}

const createSettings = {
  Name: {
    key: 'Name',
    label: 'Name',
    ...columnInputs.Name
  }
}

const getUpdateObject = (updated: any) => {
  const { Id, Name } = updated

  return {
    id: Id.value,
    name: Name.value
  }
}

const getRowObject = (project: any): ProjectColumns => {
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
    Models: {
      key: 'Models',
      display: (models: any) => Array.isArray(models)
        ? models?.map((model: any) => `${model?.Name}`).join(', ')
        : `${models?.Name}`,
      value: project.TrainedModels,
      editable: true,
      queryable: true,
      edit: columnInputs.Models
    }
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
        :get-row-object="getRowObject"
        :get-update-object="getUpdateObject"
        :create-settings="createSettings"
    />
  </div>
</template>

<style>
</style>