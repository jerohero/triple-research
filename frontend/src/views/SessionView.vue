<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import { useRoute } from 'vue-router'
import SessionSubscriber from '@/components/SessionSubscriber.vue'
import StopSession from "@/components/StopSession.vue";
import axios from "@/shared/axios";
import {onMounted, ref} from "vue";

const router = useRoute()

const title = 'Session'

const session = ref<any>()

onMounted(() => {
  fetchSession()
})

const fetchSession = async () => {
  session.value = (await axios().get(`session/${ router.params.id.toString() }`)).data
}
</script>

<template>
  <div class="mx-10 my-7 min-h-full">
    <EntityTitle
        :title="title"
        :sub="session?.Pod"
    />
    <div class="mt-3">
      <StopSession
          v-if="session && session.Status === 'Running'"
          :session-id="router.params.id.toString()"
      />
    </div>
    <SessionSubscriber
        :id="router.params.id.toString()"
    />
  </div>
</template>

<style>
</style>