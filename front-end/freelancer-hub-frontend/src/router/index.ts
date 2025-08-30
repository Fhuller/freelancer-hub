import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Login from '../views/Login.vue'
import Dashboard from '../views/Dashboard.vue'
import Clients from '../views/Clients.vue'
import Finance from '../views/Finance.vue'
import ResetPassword from '../views/ResetPassword.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/dashboard' },
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/reset-password', name: 'ResetPassword', component: ResetPassword },
    { path: '/dashboard', name: 'Dashboard', component: Dashboard, meta: { requiresAuth: true } },
    { path: '/clients', name: 'Clients', component: Clients, meta: { requiresAuth: true } },
    { path: '/finance', name: 'Finance', component: Finance, meta: { requiresAuth: true } },
    { path: '/:pathMatch(.*)*', redirect: '/dashboard' }
  ]
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.name === 'ResetPassword') {
    return next()
  }
  
  try {
    await authStore.checkAuth()
  } catch (err) {
    console.error(err)
  }
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/login')
  }
  
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return next('/dashboard')
  }
  
  next()
})

export default router