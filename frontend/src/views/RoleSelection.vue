<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../api/client'
import { setCurrentUser } from '../api/session'

const router = useRouter()
const users = ref([])
const role = ref('Student')
const selectedUserId = ref(null)
const loading = ref(true)
const error = ref('')

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
      <h1>Select a seeded user</h1>
      <p class="lead">Use one of the seeded student or mentor identities to test the request workflow.</p>
    </div>

    <div class="panel">
      <div class="segmented">
        <button type="button" :class="{ active: role === 'Student' }" @click="setRole('Student')">Student</button>
        <button type="button" :class="{ active: role === 'Mentor' }" @click="setRole('Mentor')">Mentor</button>
      </div>

      <p v-if="loading" class="muted">Loading users...</p>
      <p v-else-if="error" class="alert error">{{ error }}</p>

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
    </div>
  </section>
</template>
