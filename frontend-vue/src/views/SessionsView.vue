<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import EntityContent from '@/components/EntityContent.vue'
import type { Column } from '@/shared/interfaces'
import { useRoute } from 'vue-router'
import dayjs from 'dayjs'

interface SessionColumns {
  Id: Column,
  Source: Column,
  Pod: Column,
  Status: Column,
  CreatedAt: Column,
  StartedAt: Column,
  StoppedAt: Column,
}

const router = useRoute()

const title = 'Sessions'

const columns = [
  'Source',
  'Pod',
  'Status',
  'Created at',
  'Started at',
  'Stopped at',
]
const route = `/vision-set/${ router.params.id }/session`
const gotoPath = (rowData: SessionColumns) => `/sessions/${ rowData.Id.value }`

const actions = [
  'goto'
]

const getRowObject = (project: any): SessionColumns => {
  return {
    Id: {
      key: 'Id',
      display: (id: string) => id,
      value: project.Id,
      editable: false,
      queryable: true,
    },
    Source: {
      key: 'Source',
      display: (source: string) => source,
      value: project.Source,
      editable: false,
      queryable: true,
    },
    Pod: {
      key: 'Pod',
      display: (pod: string) => pod,
      value: project.Pod,
      editable: false,
      queryable: true,
    },
    Status: {
      key: 'Status',
      display: (status: string) => status,
      value: project.Status,
      editable: false,
      queryable: true,
    },
    CreatedAt: {
      key: 'CreatedAt',
      display: (createdAt: string) => formatDate(createdAt),
      value: project.CreatedAt,
      editable: true,
      queryable: true
    },
    StartedAt: {
      key: 'StartedAt',
      display: (startedAt: string) => formatDate(startedAt),
      value: project.StartedAt,
      editable: true,
      queryable: true
    },
    StoppedAt: {
      key: 'StoppedAt',
      display: (stoppedAt: string) => formatDate(stoppedAt),
      value: project.StoppedAt,
      editable: true,
      queryable: true
    }
  }
}

const formatDate = (date: string) => {
  if (dayjs(date).isBefore(dayjs('2000-1-1'))) {
    return ''
  }

  return dayjs(date).format('DD-MM-YYYY HH:mm:ss')
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
        :get-row-object="getRowObject"
        :actions="actions"
    />
  </div>
</template>

<style>
</style>