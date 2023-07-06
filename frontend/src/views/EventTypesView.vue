<script setup lang="ts">
  import EntityTitle from '@/components/EntityTitle.vue'
  import EntityContent from '@/components/EntityContent.vue'
  import type { Column } from '@/shared/interfaces';

  interface EventTypeColumns {
    id: Column,
    name: Column,
    message: Column,
    symbol: Column
  }

  const title = 'Event Types'

  const columns = [
    'Name', 'Message', 'Symbol'
  ]
  const route = '/event-type'

  const columnInputs = {
    name: {
      type: 'input-text'
    },
    message: {
      type: 'input-text'
    },
    symbol: {
      type: 'input-text'
    }
  }

  const createSettings = {
    name: {
      key: 'name',
      label: 'Name',
      ...columnInputs.name
    },
    message: {
      key: 'message',
      label: 'Message',
      ...columnInputs.message
    },
    symbol: {
      key: 'symbol',
      label: 'Symbol',
      ...columnInputs.symbol
    }
  }

  const getUpdateObject = (updated: any) => {
    const { name, message, symbol } = updated

    return {
      name: name.value,
      message: message.value,
      symbol: symbol.value
    }
  }

  const getRowObject = (eventType: any): EventTypeColumns => {
    return {
      id: {
        key: 'id',
        display: (id: string) => id,
        value: eventType.id,
        editable: false,
        queryable: true
      },
      name: {
        key: 'name',
        display: (name: string) => name,
        value: eventType.name,
        editable: true,
        queryable: true,
        edit: columnInputs.name
      },
      message: {
        key: 'message',
        display: (message: string) => message,
        value: eventType.message,
        editable: true,
        queryable: true,
        edit: columnInputs.message
      },
      symbol: {
        key: 'symbol',
        display: (symbol: string) => symbol,
        value: eventType.symbol,
        editable: true,
        queryable: true,
        edit: columnInputs.symbol
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