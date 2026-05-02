<script setup>
import { reactive, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { api } from '../api/client'
import { setCurrentUser } from '../api/session'

const router = useRouter()
const loading = ref(false)
const error = ref('')

const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  enrollmentNumber: '',
  studyProgram: 'UNI',
  yearOfStudy: '',
})

async function submit() {
  loading.value = true
  error.value = ''

  try {
    const user = await api.registerStudent({
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
      password: form.password,
      enrollmentNumber: form.enrollmentNumber,
      studyProgram: form.studyProgram,
      yearOfStudy: form.yearOfStudy ? Number(form.yearOfStudy) : null,
    })

    setCurrentUser(user)
    router.push('/student')
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="page narrow">
    <div class="page-title">
      <p class="eyebrow">Student registration</p>
      <h1>Create a student account</h1>
      <p class="lead">Register as a student to submit mentorship requests and track responses.</p>
    </div>

    <section class="panel">
      <form class="stack" @submit.prevent="submit">
        <div class="two-column">
          <label>
            First name
            <input v-model.trim="form.firstName" required maxlength="80" />
          </label>

          <label>
            Last name
            <input v-model.trim="form.lastName" required maxlength="100" />
          </label>
        </div>

        <label>
          Email
          <input v-model.trim="form.email" type="email" required maxlength="160" />
        </label>

        <label>
          Password
          <input v-model="form.password" type="password" required minlength="6" maxlength="120" />
        </label>

        <div class="two-column">
          <label>
            Enrollment number
            <input v-model.trim="form.enrollmentNumber" required maxlength="40" />
          </label>

          <label>
            Study program
            <select v-model="form.studyProgram" required>
              <option value="VSS">VSS</option>
              <option value="UNI">UNI</option>
              <option value="MAG">MAG</option>
            </select>
          </label>
        </div>

        <label>
          Year of study
          <input v-model="form.yearOfStudy" type="number" min="1" max="5" />
        </label>

        <p v-if="error" class="alert error">{{ error }}</p>

        <div class="form-actions">
          <RouterLink class="button secondary" to="/login">Back to login</RouterLink>
          <button class="button" type="submit" :disabled="loading">
            {{ loading ? 'Creating account...' : 'Create account' }}
          </button>
        </div>
      </form>
    </section>
  </section>
</template>
