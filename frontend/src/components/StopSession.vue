<script setup lang="ts">
  import axios from "@/shared/axios";
  import {useToast} from "vue-toastification";

  const props = defineProps<{
    sessionId: string
  }>()

  const toast = useToast()

  const stopSession = async () => {
    try {
      await axios().post(`session/${ props.sessionId }/stop`)

      toast.success("Session has been stopped successfully!")
    } catch(e: any) {
      console.log(e)
      toast.error(e.response.message)
    }
  }
</script>

<template>
  <div class="w-fit flex bg-foreground items-center space-x-1 py-1 px-2 rounded cursor-pointer hover:bg-backgroundDark">
    <span @click="stopSession">
      Stop
    </span>
    <ion-icon name="stop-circle-outline" class="text-primary" aria-hidden="true"/>
  </div>
</template>

<style scoped>

</style>
