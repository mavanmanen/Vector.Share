import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/Home.vue'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            component: Home
        },
        {
            path: '/:identifier',
            redirect: to => {
                const { params } = to
                window.location.href = `${process.env.VUE_APP_API_URL}/${params.identifier}`
                return ''
            }
        }
    ]
})

export default router