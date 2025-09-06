import './assets/base.css'
import 'flag-icons/css/flag-icons.min.css';
import 'vue-toast-notification/dist/theme-default.css';
import ToastPlugin from 'vue-toast-notification';
import App from './App.vue'
import router from './router'
import i18n from './i18n'
import { createApp } from 'vue'
import { createPinia } from 'pinia'

const app = createApp(App)

app.use(createPinia())
app.use(i18n)
app.use(router)
app.use(ToastPlugin);

app.mount('#app')