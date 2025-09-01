import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import LandingPage from '../views/LandingPage.vue'
import About from '../views/About.vue'
import Portfolio from '../views/Portfolio.vue'

import Login from '../views/Login.vue'
import Dashboard from '../views/Dashboard.vue'
import Clients from '../views/Clients.vue'
import Client from '../views/Client.vue'
import Finance from '../views/Finance.vue'
import ResetPassword from '../views/ResetPassword.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'LandingPage', component: LandingPage },
    { path: '/about', name: 'About', component: About },
    { path: '/portfolio', name: 'Portfolio', component: Portfolio },
    { path: '/login', name: 'Login', component: Login, meta: { requiresGuest: true } },
    { path: '/reset-password', name: 'ResetPassword', component: ResetPassword },
    { path: '/app/dashboard', name: 'Dashboard', component: Dashboard, meta: { requiresAuth: true } },
    { path: '/app/clients', name: 'Clients', component: Clients, meta: { requiresAuth: true } },
    { path: '/app/clients/:id', name: 'Client', component: Client, meta: { requiresAuth: true }, props: true },
    { path: '/app/finance', name: 'Finance', component: Finance, meta: { requiresAuth: true } },
    { path: '/:pathMatch(.*)*', redirect: '/' }
  ]
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  try {
    await authStore.checkAuth()
  } catch (err) {
    console.error(err)
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next({ name: 'Login' })
  }

  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return next({ name: 'Dashboard' })
  }

  next()
})

export default router