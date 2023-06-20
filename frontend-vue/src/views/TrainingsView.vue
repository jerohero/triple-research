<script setup lang="ts">
  import EntityTitle from '@/components/EntityTitle.vue'
  import EntityContent from '@/components/EntityContent.vue'
  import dayjs from 'dayjs'
  import type { Column } from '@/shared/interfaces'

  interface TrainingColumns {
    id: Column,
    status: Column,
    instructor: Column,
    students: Column,
    date: Column
  }

  const title = 'Trainings'

  const columns = [
    'Status', 'Instructor', 'Students', 'Date'
  ]
  const route = '/training'

  const columnInputs = {
    students: {
      type: 'search-multiple',
      options: {
        id: (student: any) => student.id,
        fetchUrl: '/user',
        display: (student: any) => Array.isArray(student)
            ? student?.map((student: any) => `${ student?.firstName } ${ student?.lastName }`).join(', ')
            : `${ student?.firstName } ${ student?.lastName }`,
        queryable: (student: any) => `${ student?.firstName } ${ student?.lastName }`,
        displaySub: (student: any) => student.email
      }
    }
  }

  const getUpdateObject = (updated: any) => {
    const { students } = updated

    return {
      students: students.value.map((student: any) => student.id)
    }
  }

  const getRowObject = (training: any): TrainingColumns => {
    const trainingDate = dayjs(training.creationDateTime)

    return {
      id: {
        key: 'id',
        display: (id: string) => id,
        value: training.id,
        editable: true,
        queryable: false
      },
      status: {
        key: 'status',
        display: (status: string) => status,
        value: training.status,
        editable: false,
        queryable: true
      },
      instructor: {
        key: 'instructor',
        display: (instructor: any) => `${ instructor?.firstName } ${ instructor?.lastName }`,
        value: training.instructor,
        editable: false,
        queryable: true,
      },
      students: {
        key: 'students',
        display: (student: any) => Array.isArray(student)
            ? student?.map((student: any) => `${ student?.firstName } ${ student?.lastName }`).join(', ')
            : `${ student?.firstName } ${ student?.lastName }`,
        value: training.students,
        editable: true,
        queryable: true,
        edit: {
          disabled: training.status === 'Processing' || training.status === 'Finished',
          ...columnInputs.students
        }
        ,
      },
      date: {
        key: 'date',
        display: (creationDateTime: string) => dayjs(creationDateTime).format('DD-MM-YYYY HH:mm'),
        value: trainingDate,
        editable: false,
        queryable: true
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
    />
  </div>
</template>

<style>
</style>