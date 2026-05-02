<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../api/client'
import { getCurrentUser } from '../api/session'
import MentorCard from '../components/MentorCard.vue'
import NotificationsPanel from '../components/NotificationsPanel.vue'
import RequestCard from '../components/RequestCard.vue'
import RequestForm from '../components/RequestForm.vue'

const router = useRouter()
const currentUser = ref(getCurrentUser())
const mentors = ref([])
const requests = ref([])
const notifications = ref([])
const selectedMentor = ref(null)
const search = ref('')
const loading = ref(true)
const actionBusy = ref(null)
const notificationBusy = ref(null)
const error = ref('')

onMounted(async () => {
  if (!currentUser.value || currentUser.value.role !== 'Student') {
    router.push('/login')
    return
  }

  await loadData()
})

async function loadData() {
  loading.value = true
  error.value = ''

  try {
    const [mentorData, requestData] = await Promise.all([
      api.getMentors(search.value),
      api.getStudentRequests(currentUser.value.id),
    ])
    mentors.value = mentorData
    requests.value = requestData
    notifications.value = await api.getNotifications(currentUser.value.id)
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function cancelRequest(request) {
  actionBusy.value = request.id
  error.value = ''

  try {
    await api.cancelRequest(request.id)
    await loadData()
  } catch (err) {
    error.value = err.message
  } finally {
    actionBusy.value = null
  }
}

async function requestCreated() {
  selectedMentor.value = null
  await loadData()
}

async function markNotificationRead(notification) {
  notificationBusy.value = notification.id
  error.value = ''

  try {
    await api.markNotificationRead(notification.id)
    notifications.value = await api.getNotifications(currentUser.value.id)
  } catch (err) {
    error.value = err.message
  } finally {
    notificationBusy.value = null
  }
}
</script>

<template>
  <section class="page">
    <div class="page-title">
      <p class="eyebrow">Student dashboard</p>
      <h1>Find and reserve a mentor</h1>
      <p class="lead">Browse availability, filter by research fit, and submit a structured mentorship request.</p>
    </div>

    <p v-if="error" class="alert error">{{ error }}</p>

    <div class="dashboard-grid">
      <section class="panel mentor-list">
        <div class="section-heading">
          <div>
            <h2>Mentors</h2>
            <p class="muted">{{ mentors.length }} mentors shown</p>
          </div>
          <input v-model.trim="search" class="search" placeholder="Search name, lab, area" @input="loadData" />
        </div>

        <p v-if="loading" class="muted">Loading mentors...</p>
        <div v-else class="mentor-grid">
          <MentorCard
            v-for="mentor in mentors"
            :key="mentor.id"
            :mentor="mentor"
            @apply="selectedMentor = $event"
          />
        </div>
      </section>

      <aside class="side-column">
        <RequestForm
          v-if="selectedMentor"
          :mentor="selectedMentor"
          :student="currentUser"
          @created="requestCreated"
          @cancel="selectedMentor = null"
        />

        <NotificationsPanel
          :notifications="notifications"
          :busy-id="notificationBusy"
          @mark-read="markNotificationRead"
        />

        <section class="panel">
          <div class="section-heading">
            <div>
              <h2>My requests</h2>
              <p class="muted">Pending, accepted, rejected, and cancelled requests</p>
            </div>
          </div>

          <div class="stack">
            <p v-if="!requests.length" class="muted">No submitted requests yet.</p>
            <RequestCard
              v-for="request in requests"
              :key="request.id"
              :request="request"
              :busy="actionBusy === request.id"
              mode="student"
              @cancel="cancelRequest(request)"
            />
          </div>
        </section>
      </aside>
    </div>
  </section>
</template>
