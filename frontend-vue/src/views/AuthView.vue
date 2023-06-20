<script setup lang="ts">
  import { ref } from 'vue'
  import CustomButton from '@/components/CustomButton.vue'
  import FormItem from '@/components/FormItem.vue'
  import { useUserStore } from '@/stores/user'
  import {useToast} from 'vue-toastification'
  import router from '@/router'

  const email = ref<string>('')
  const password = ref<string>('')

  const userStore = useUserStore()
  const toast = useToast()

  const login = () => {
    userStore.login(email.value, password.value).then(() => {
      if (userStore.user.userType === 'Student' || userStore.user.userType === 'Instructor') {
        toast.error("Not authorized for admin access")
        return
      }

      router.push({ path: '/users' })
    })
  }
</script>

<template>
  <div class="flex h-screen flex-col justify-center pb-24 px-6 lg:px-8 text-text">
    <img src="@/assets/img/vref-white.png" alt="VRef Solutions" class="mx-auto h-16 w-auto" />
    <div class="mt-8 mx-auto w-full max-w-md">
      <div class="bg-foreground bg- py-8 px-4 shadow rounded-lg px-10">
        <form @submit.prevent="login" class="space-y-6">
          <FormItem v-model:value="email"
                    label="Email address" name="email"
                    type="email" autocomplete="email"
          />
          <FormItem v-model:value="password"
                    label="Password" name="password"
                    type="password" autocomplete="current-password"
          />
          <div class="pt-3">
            <CustomButton text="Sign in" full-width is-primary />
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
