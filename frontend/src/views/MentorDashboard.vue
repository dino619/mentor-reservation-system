<script setup>
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../api/client'
import { getCurrentUser } from '../api/session'
import NotificationsPanel from '../components/NotificationsPanel.vue'
import RequestCard from '../components/RequestCard.vue'

const router = useRouter()
const currentUser = ref(getCurrentUser())
const mentors = ref([])
const mentor = ref(null)
const selectedMentorId = ref(null)
const requests = ref([])
const notifications = ref([])
const comments = reactive({})
const loading = ref(true)
const busyRequestId = ref(null)
const notificationBusy = ref(null)
const importing = ref(false)
const importResult = ref(null)
const error = ref('')

const pendingCount = computed(() => requests.value.filter((request) => request.status === 'Pending').length)

onMounted(async () => {
  if (!currentUser.value || currentUser.value.role !== 'Mentor') {
    router.push('/login')
    return
  }

  await loadData()
})

watch(selectedMentorId, async () => {
  if (selectedMentorId.value) {
    mentor.value = mentors.value.find((item) => item.id === selectedMentorId.value)
    await loadRequests()
  }
})

async function loadData() {
  loading.value = true
  error.value = ''

  try {
    mentors.value = await api.getMentors()
    mentor.value = mentors.value.find((item) => item.userId === currentUser.value.id) ?? mentors.value[0] ?? null
    selectedMentorId.value = mentor.value?.id ?? null
    notifications.value = await api.getNotifications(currentUser.value.id)

    if (mentor.value) {
      await loadRequests()
    }
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function loadRequests() {
  if (!selectedMentorId.value) {
    requests.value = []
    return
  }

  requests.value = await api.getMentorRequests(selectedMentorId.value)
}

async function decide(request, decision) {
  busyRequestId.value = request.id
  error.value = ''

  try {
    if (decision === 'accept') {
      await api.acceptRequest(request.id, comments[request.id] ?? '')
    } else {
      await api.rejectRequest(request.id, comments[request.id] ?? '')
    }

    comments[request.id] = ''
    await loadData()
  } catch (err) {
    error.value = err.message
  } finally {
    busyRequestId.value = null
  }
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

async function importMentors() {
  importing.value = true
  error.value = ''
  importResult.value = null

  try {
    importResult.value = await api.importMentors()
    await loadData()
  } catch (err) {
    error.value = err.message
  } finally {
    importing.value = false
  }
}
</script>

<template>
  <section class="page">
    <div class="page-title">
      <p class="eyebrow">Mentor dashboard</p>
      <h1>Manage incoming requests</h1>
      <p class="lead">Review thesis proposals in one place and respond before official STUDIS registration.</p>
    </div>

    <p v-if="error" class="alert error">{{ error }}</p>
    <p v-if="loading" class="muted">Loading mentor data...</p>

    <template v-else>
      <section class="panel import-panel">
        <div class="section-heading">
          <div>
            <h2>FRI mentor import</h2>
            <p class="muted">Imports real public mentor profiles from the local saved page or the FRI URL.</p>
          </div>
          <button class="button secondary" type="button" :disabled="importing" @click="importMentors">
            {{ importing ? 'Importing...' : 'Import mentors' }}
          </button>
        </div>
        <p v-if="importResult" class="muted">
          Imported {{ importResult.importedCount }}, updated {{ importResult.updatedCount }},
          skipped {{ importResult.skippedCount }}.
        </p>
      </section>

      <p v-if="!mentors.length" class="alert error">
        No mentor profiles are available yet. Run the mentor import first.
      </p>

      <template v-else-if="mentor">
        <section class="metrics">
          <div class="metric">
            <span>Accepted students</span>
            <strong>{{ mentor.currentAcceptedStudents }} / {{ mentor.maxStudents }}</strong>
          </div>
          <div class="metric">
            <span>Available slots</span>
            <strong>{{ mentor.availableSlots }}</strong>
          </div>
          <div class="metric">
            <span>Pending requests</span>
            <strong>{{ pendingCount }}</strong>
          </div>
        </section>

        <div class="dashboard-grid">
          <section class="panel">
            <div class="section-heading">
              <div>
                <h2>Requests for {{ [mentor.title, mentor.fullName].filter(Boolean).join(' ') }}</h2>
                <p class="muted">{{ mentor.laboratory || 'Demo mode can manage any imported mentor profile.' }}</p>
              </div>
              <span class="slot-badge" :class="{ full: mentor.availableSlots === 0 }">
                {{ mentor.availableSlots }} slots free
              </span>
            </div>

            <label class="mentor-picker">
              Mentor context
              <select v-model.number="selectedMentorId">
                <option v-for="item in mentors" :key="item.id" :value="item.id">
                  {{ [item.title, item.fullName].filter(Boolean).join(' ') }}
                </option>
              </select>
            </label>

            <div class="stack">
              <p v-if="!requests.length" class="muted">No requests yet.</p>
              <RequestCard
                v-for="request in requests"
                :key="request.id"
                v-model:comment="comments[request.id]"
                :request="request"
                :busy="busyRequestId === request.id"
                mode="mentor"
                @accept="decide(request, 'accept')"
                @reject="decide(request, 'reject')"
              />
            </div>
          </section>

          <aside class="side-column">
            <NotificationsPanel
              :notifications="notifications"
              :busy-id="notificationBusy"
              @mark-read="markNotificationRead"
            />
          </aside>
        </div>
      </template>
    </template>
  </section>
</template>
