<script setup lang="ts">
  import NavSidebarItem from '@/components/NavSidebarItem.vue'
  import { ref, watch } from 'vue'
  import { useRoute } from 'vue-router'
  import { useUserStore } from '@/stores/user'
  import type { INavSidebarItem } from '@/shared/interfaces'

  const route = useRoute()
  const userStore = useUserStore();

  const activeRoute = ref<string>('')

  const routes: INavSidebarItem[] = [
    { name: 'Users', href: '/users', icon: 'person-outline' },
    { name: 'Trainings', href: '/trainings', icon: 'airplane-outline' }
  ]
  const foldables: INavSidebarItem[] = []

  if (userStore.isSuperAdmin) {
    routes.push({ name: 'Organizations', href: '/organizations', icon: 'people-outline' })
    foldables.push(
        { name: 'AI', href: '/ai', icon: 'hardware-chip-outline', subItems: [
            { name: 'Event Types', href: '/event-types' },
            { name: 'ECAM Messages', href: '/ecam-messages' }
          ]
        }
    )
  }

  const signOutItem: INavSidebarItem =
      { name: 'Sign Out', href: '/', icon: 'log-out-outline', cb: () => { signOut() }}

  const signOut = () => {
    userStore.logout()
  }

  watch(route, (from, to) => {
    activeRoute.value = to.path
  })
</script>

<template>
  <div class="bg-secondary fixed pt-[var(--header-height)] flex flex-col justify-between
              w-[var(--sidebar-width)] min-w-[15rem] h-full"
  >
    <div class="mt-16">
      <RouterLink
          v-for="route in routes"
          v-bind:key="route.href"
          :to="route.href"
      >
        <NavSidebarItem :item="route" :activeRoute="activeRoute"/>
      </RouterLink>
      <NavSidebarItem
          v-for="foldable in foldables"
          v-bind:key="foldable.href"
          :item="foldable"
          :active-route="activeRoute"
          :is-foldable="true"
      />
    </div>
    <div class="mb-5">
      <NavSidebarItem :item="signOutItem"/>
    </div>
  </div>
</template>

<style scoped>

</style>
