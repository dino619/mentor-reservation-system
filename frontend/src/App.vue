<script setup>
import { computed, ref, watch } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'
import { getCurrentUser, clearCurrentUser } from './api/session'

const route = useRoute()
const router = useRouter()
const currentUser = ref(getCurrentUser())

watch(
  () => route.fullPath,
  () => {
    currentUser.value = getCurrentUser()
  }
)

const homeRoute = computed(() => {
  if (!currentUser.value) return '/login'
  return currentUser.value.role === 'Mentor' ? '/mentor' : '/student'
})

function logout() {
  clearCurrentUser()
  currentUser.value = null
  router.push('/login')
}
</script>

<template>
  <div class="app-shell">
    <header class="topbar">
      <RouterLink class="brand" :to="homeRoute">
        <span class="brand-mark">MR</span>
        <span>
          <strong>Mentor Reservation</strong>
          <small>Before STUDIS thesis registration</small>
        </span>
      </RouterLink>

      <nav class="topnav" aria-label="Main navigation">
        <RouterLink to="/student">Student</RouterLink>
        <RouterLink to="/mentor">Mentor</RouterLink>
      </nav>

      <div class="session">
        <span v-if="currentUser">{{ currentUser.fullName }}</span>
        <RouterLink v-if="!currentUser" class="button secondary" to="/login">Select user</RouterLink>
        <button v-else class="button secondary" type="button" @click="logout">Switch user</button>
      </div>
    </header>

    <main>
      <RouterView />
    </main>
  </div>
</template>
