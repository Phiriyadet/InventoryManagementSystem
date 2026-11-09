import { createApp } from 'vue'
import { ConfirmationService, ToastService } from 'primevue'
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import 'primeicons/primeicons.css'

import App from './App.vue'
import router from './router'

import './assets/styles/index.css'

const app = createApp(App)

app.use(router)

app.use(PrimeVue, {
  theme: {
    preset: Aura,
  },
})

app.use(ConfirmationService)
app.use(ToastService)

app.mount('#app')
