<script setup lang="ts">
  import EntityTitle from '@/components/EntityTitle.vue'
  import EntityContent from '@/components/EntityContent.vue'
  import { useUserStore } from '@/stores/user'
  import type { Column } from '@/shared/interfaces'

  interface UserColumns {
    id: Column,
    email: Column,
    firstName: Column,
    lastName: Column,
    userType: Column,
    organization: Column
  }

  const userStore = useUserStore()

  const title = 'Users'

  const columns = [
    'Email', 'First name', 'Last name', 'Role', 'Organization'
  ]
  const route = '/user'

  const columnInputs = {
    email: {
      type: 'input-text'
    },
    firstName: {
      type: 'input-text'
    },
    lastName: {
      type: 'input-text'
    },
    userType: {
      type: 'search-single',
      options: {
        id: (userType: string) => userType,
        value: [
          'SuperAdmin',
          'Admin',
          'Instructor',
          'Student'
        ],
        display: (userType: string) => userType,
        queryable: (userType: string) => userType
      }
    },
    organization: {
      type: 'search-single',
      disabled: !userStore.isSuperAdmin,
      options: {
        id: (organization: any) => organization.id,
        fetchUrl: '/organization',
        display: (organization: any) => organization.name,
        queryable: (organization: any) => organization.name
      }
    }
  }

  const createSettings = {
    email: {
      key: 'email',
      label: 'Email',
      ...columnInputs.email
    },
    firstName: {
      key: 'firstName',
      label: 'First name',
      ...columnInputs.firstName
    },
    lastName: {
      key: 'lastName',
      label: 'Last name',
      ...columnInputs.lastName
    },
    organization: {
      key: 'organization',
      label: 'Organization',
      staticValue: !userStore.isSuperAdmin ? { id: userStore.user.organization.id } : null,
      ...columnInputs.organization
    },
  }

  const getUpdateObject = (updated: any) => {
    const { email, firstName, lastName, userType, organization } = updated

    return {
      email: email.value,
      firstName: firstName.value,
      lastName: lastName.value,
      userType: userType.value,
      organization: {
        id: organization.value.id
      }
    }
  }

  const getRowObject = (user: any): UserColumns => {
    return {
      id: {
        key: 'id',
        display: (id: string) => id,
        value: user.id,
        queryable: true,
        editable: false,
      },
      email: {
        key: 'email',
        display: (email: string) => email,
        value: user.email,
        editable: true,
        queryable: true,
        edit: columnInputs.email
      },
      firstName: {
        key: 'firstName',
        display: (firstName: string) => firstName,
        value: user.firstName,
        editable: true,
        queryable: true,
        edit: columnInputs.firstName
      },
      lastName: {
        key: 'lastName',
        display: (lastName: string) => lastName,
        value: user.lastName,
        editable: true,
        queryable: true,
        edit: columnInputs.lastName
      },
      userType: {
        key: 'userType',
        display: (userType: string) => userType,
        value: user.userType,
        queryable: true,
        editable: true,
        edit: columnInputs.userType
      },
      organization: {
        key: 'organization',
        display: (organization: any) => organization.name,
        value: user.organization,
        queryable: true,
        editable: true,
        edit: columnInputs.organization
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
        :getRowObject="getRowObject"
        :getUpdateObject="getUpdateObject"
        :create-settings="createSettings"
    />
  </div>
</template>

<style>
</style>