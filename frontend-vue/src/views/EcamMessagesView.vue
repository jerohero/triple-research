<script setup lang="ts">
  import EntityTitle from '@/components/EntityTitle.vue'
  import EntityContent from '@/components/EntityContent.vue'
  import type { Column } from '@/shared/interfaces';

  interface EcamMessageColumns {
    id: Column,
    name: Column,
    isAccepted: Column,
  }

  const title = 'ECAM Messages'

  const columns = [
    'Name', 'Accepted',
  ]
  const route = '/ecam-message'

  const columnInputs = {
    name: {
      type: 'input-text'
    },
    isAccepted: {
      type: 'search-single',
      options: {
        id: (isAccepted: any) => isAccepted.display,
        value: [
          { display: 'True', id: true },
          { display: 'False', id: false }
        ],
        display: (isAccepted: any) => isAccepted.display,
        queryable: (isAccepted: any) => isAccepted.display
      }
    },
  }

  const createSettings = {
    name: {
      key: 'name',
      label: 'Name',
      ...columnInputs.name
    },
    isAccepted: {
      key: 'isAccepted',
      label: 'Accepted',
      ...columnInputs.isAccepted
    }
  }

  const getUpdateObject = (updated: any) => {
    const { name, isAccepted } = updated

    return {
      name: name.value,
      isAccepted: isAccepted.value.id
    }
  }

  const getRowObject = (ecamMessage: any): EcamMessageColumns => {
    return {
      id: {
        key: 'id',
        display: (id: string) => id,
        value: ecamMessage.id,
        editable: false,
        queryable: true,
      },
      name: {
        key: 'name',
        display: (name: string) => name,
        value: ecamMessage.name,
        editable: true,
        queryable: true,
        edit: columnInputs.name
      },
      isAccepted: {
        key: 'isAccepted',
        display: (isAccepted: any) => isAccepted ? 'True' : 'False',
        value: ecamMessage.isAccepted,
        editable: true,
        queryable: true,
        edit: columnInputs.isAccepted
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