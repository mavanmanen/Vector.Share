import { createApp } from 'vue'
import App from './App.vue'
import mitt from 'mitt'

export const eventBus = mitt()

createApp(App).mount('#app')
