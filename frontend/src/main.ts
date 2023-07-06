import { createApp } from 'vue'
import { createPinia } from 'pinia'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'

import App from './App.vue'
import router from './router'

import './assets/main.css'

const app = createApp(App)

const toastOptions = {
    timeout: 2000
}

app.use(createPinia())
app.use(Toast, toastOptions)
app.use(router)

app.mount('#app')
