import { createApp } from 'vue'
import App from './App.vue'
import mitt from 'mitt'
import router from './router'

export const eventBus = mitt()

createApp(App).use(router).mount('#app')
