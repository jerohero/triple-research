<script setup lang="ts">
import EntityTitle from '@/components/EntityTitle.vue'
import EntityContent from '@/components/EntityContent.vue'
import type { Column } from '@/shared/interfaces'
import { useRoute } from 'vue-router'
import dayjs from 'dayjs'
import {onMounted, ref} from "vue";
import axios from "@/shared/axios";
import type {AxiosResponse} from "axios";
import SessionSubscriber from "@/components/SessionSubscriber.vue";

const router = useRoute()

const title = 'Session'
</script>

<template>
  <div class="mx-10 my-7 min-h-full">
    <EntityTitle :title="title"/>
    <SessionSubscriber :id="router.params.id.toString()"/>
  </div>
</template>

<style>
</style>