import { createRouter, createWebHistory } from 'vue-router'
import RoleSelection from '../views/RoleSelection.vue'
import StudentDashboard from '../views/StudentDashboard.vue'
import MentorDashboard from '../views/MentorDashboard.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/login' },
    { path: '/login', component: RoleSelection },
    { path: '/student', component: StudentDashboard },
    { path: '/mentor', component: MentorDashboard },
  ],
})

export default router
