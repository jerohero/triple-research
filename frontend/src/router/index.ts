import { createRouter, createWebHistory } from 'vue-router'
import ProjectsView from '@/views/ProjectsView.vue'
import VisionSetView from '@/views/VisionSetsView.vue'
import SessionsView from '@/views/SessionsView.vue'
import SessionView from '@/views/SessionView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      alias: '/projects',
      name: 'projects',
      component: ProjectsView
    },
    {
      path: '/projects/:id',
      name: 'vision-sets',
      component: VisionSetView
    },
    {
      path: '/vision-sets/:id',
      name: 'sessions',
      component: SessionsView
    },
    {
      path: '/sessions/:id',
      name: 'session',
      component: SessionView
    },
  ]
})

export default router
