<script setup lang="ts">
  import NavSidebarItem from '@/components/NavSidebarItem.vue'
  import { ref, watch } from 'vue'
  import { useRoute } from 'vue-router'
  import type { INavSidebarItem } from '@/shared/interfaces'

  const route = useRoute()

  const activeRoute = ref<string>('')

  const routes: INavSidebarItem[] = [
    { name: 'Projects', href: '/projects', icon: 'person-outline' },
  ]

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
    </div>
  </div>
</template>

<style scoped>

</style>
