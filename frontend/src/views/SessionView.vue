<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import { useRoute } from 'vue-router'
import SessionSubscriber from '@/components/SessionSubscriber.vue'

const router = useRoute()

const title = 'Session'
</script>

<template>
  <div class="mx-10 my-7 min-h-full">
    <EntityTitle
        :title="title"
        :sub="router.path"
    />
    <SessionSubscriber :id="router.params.id.toString()"/>
  </div>
</template>

<style>
</style>