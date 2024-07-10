// import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import 'element-plus/theme-chalk/dark/css-vars.css'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
//@ts-ignore
import vi from 'element-plus/dist/locale/vi.mjs'
const app = createApp(App)

app.use(router)
app.use(ElementPlus, {
    locale: vi,
  })
app.mount('#app')