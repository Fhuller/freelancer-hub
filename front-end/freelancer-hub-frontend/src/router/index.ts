import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Login from '../views/Login.vue'
import Dashboard from '../views/Dashboard.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/dashboard' },
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/dashboard', name: 'Dashboard', component: Dashboard, meta: { requiresAuth: true } },
    { path: '/:pathMatch(.*)*', redirect: '/dashboard' } // fallback para evitar tela de erro
  ]
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  try {
    await authStore.checkAuth() // sempre checa antes de decidir
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
