<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { api } from '../api/client'
import { setCurrentUser } from '../api/session'

const router = useRouter()
const users = ref([])
const role = ref('Student')
const selectedUserId = ref(null)
const mode = ref('login')
const loading = ref(true)
const submitting = ref(false)
const error = ref('')

const loginForm = reactive({
  email: '',
  password: '',
})

const filteredUsers = computed(() => users.value.filter((user) => user.role === role.value))

onMounted(loadUsers)

async function loadUsers() {
  loading.value = true
  error.value = ''

  try {
    users.value = await api.getUsers()
    selectedUserId.value = filteredUsers.value[0]?.id ?? null
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

function setRole(nextRole) {
  role.value = nextRole
  selectedUserId.value = filteredUsers.value[0]?.id ?? null
}

async function login() {
  submitting.value = true
  error.value = ''

  try {
    const user = await api.login(loginForm)
    setCurrentUser(user)
    router.push(user.role === 'Mentor' ? '/mentor' : '/student')
  } catch (err) {
    error.value = err.message
  } finally {
    submitting.value = false
  }
}

function continueAsUser() {
  const user = users.value.find((item) => item.id === selectedUserId.value)
  if (!user) return

  setCurrentUser(user)
  router.push(user.role === 'Mentor' ? '/mentor' : '/student')
}
</script>

<template>
  <section class="page narrow">
    <div class="page-title">
      <p class="eyebrow">Prototype login</p>
      <h1>Enter the mentor reservation system</h1>
      <p class="lead">Use a registered student account or keep using seeded demo users for presentation testing.</p>
    </div>

    <div class="panel">
      <div class="segmented">
        <button type="button" :class="{ active: mode === 'login' }" @click="mode = 'login'">Login</button>
        <button type="button" :class="{ active: mode === 'demo' }" @click="mode = 'demo'">Demo users</button>
      </div>

      <p v-if="error" class="alert error">{{ error }}</p>

      <form v-if="mode === 'login'" class="stack" @submit.prevent="login">
        <label>
          Email
          <input v-model.trim="loginForm.email" type="email" required />
        </label>

        <label>
          Password
          <input v-model="loginForm.password" type="password" required />
        </label>

        <div class="form-actions">
          <RouterLink class="button secondary" to="/register">Register student</RouterLink>
          <button class="button" type="submit" :disabled="submitting">
            {{ submitting ? 'Logging in...' : 'Login' }}
          </button>
        </div>

        <p class="muted">Seeded users use password <strong>Password123!</strong>.</p>
      </form>

      <template v-else>
        <div class="segmented">
          <button type="button" :class="{ active: role === 'Student' }" @click="setRole('Student')">Student</button>
          <button type="button" :class="{ active: role === 'Mentor' }" @click="setRole('Mentor')">Mentor</button>
        </div>

        <p v-if="loading" class="muted">Loading users...</p>

        <div v-else class="stack">
          <label>
            User
            <select v-model.number="selectedUserId">
              <option v-for="user in filteredUsers" :key="user.id" :value="user.id">
                {{ user.fullName }} · {{ user.email }}
              </option>
            </select>
          </label>

          <button class="button" type="button" :disabled="!selectedUserId" @click="continueAsUser">
            Continue
          </button>
        </div>
      </template>
    </div>
  </section>
</template>
