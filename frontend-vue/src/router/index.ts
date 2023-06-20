import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'
import TrainingsView from '@/views/TrainingsView.vue'
import AuthView from '@/views/AuthView.vue'
import OrganizationsView from '@/views/OrganizationsView.vue'
import UsersView from '@/views/UsersView.vue'
import EventTypesView from '@/views/EventTypesView.vue'
import EcamMessagesView from '@/views/EcamMessagesView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      alias: '/auth',
      name: 'auth',
      component: AuthView
    },
    {
      path: '/trainings',
      name: 'trainings',
      component: TrainingsView,
      meta: { requiresAuth: true }
    },
    {
      path: '/organizations',
      name: 'organizations',
      component: OrganizationsView,
      meta: {
        requiresAuth: true,
        requiresSuperAdmin: true
      }
    },
    {
      path: '/users',
      name: 'users',
      component: UsersView,
      meta: { requiresAuth: true }
    },
    {
      path: '/event-types',
      name: 'event-types',
      component: EventTypesView,
      meta: { requiresAuth: true }
    },
    {
      path: '/ecam-messages',
      name: 'ecam-messages',
      component: EcamMessagesView,
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to) => {
  const userStore = useUserStore()

  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    return {
      name: 'auth'
    }
  }

  if (to.meta.requiresSuperAdmin && !userStore.isSuperAdmin) {
    return false
  }

  return true
})

export default router
