<script setup lang="ts">
  import EntityTitle from '@/components/EntityTitle.vue'
  import EntityContent from '@/components/EntityContent.vue'
  import type { Column } from '@/shared/interfaces'

  interface OrganizationColumns {
    id: Column,
    name: Column,
  }

  const title = 'Organizations'

  const columns = [
    'Name'
  ]
  const route = '/organization'

  const columnInputs = {
    name: {
      type: 'input-text'
    }
  }

  const createSettings = {
    name: {
      key: 'name',
      label: 'Name',
      ...columnInputs.name
    }
  }

  const getUpdateObject = (updated: any) => {
    const { name } = updated

    return {
      name: name.value
    }
  }

  const getRowObject = (training: any): OrganizationColumns => {
    return {
      id: {
        key: 'id',
        display: (id: string) => id,
        value: training.id,
        editable: true,
        queryable: true,
      },
      name: {
        key: 'name',
        display: (name: string) => name,
        value: training.name,
        editable: true,
        queryable: true,
        edit: columnInputs.name
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