<script setup>
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../api/client'
import { getCurrentUser } from '../api/session'
import RequestCard from '../components/RequestCard.vue'

const router = useRouter()
const currentUser = ref(getCurrentUser())
const mentor = ref(null)
const requests = ref([])
const comments = reactive({})
const loading = ref(true)
const busyRequestId = ref(null)
const error = ref('')

const pendingCount = computed(() => requests.value.filter((request) => request.status === 'Pending').length)

onMounted(async () => {
  if (!currentUser.value || currentUser.value.role !== 'Mentor') {
    router.push('/login')
    return
  }

  await loadData()
})

async function loadData() {
  loading.value = true
  error.value = ''

  try {
    const mentors = await api.getMentors()
    mentor.value = mentors.find((item) => item.userId === currentUser.value.id)

    if (!mentor.value) {
      throw new Error('No mentor profile found for this user.')
    }

    requests.value = await api.getMentorRequests(mentor.value.id)
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
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

      <section class="panel">
        <div class="section-heading">
          <div>
            <h2>Requests for {{ mentor.fullName }}</h2>
            <p class="muted">{{ mentor.laboratory }}</p>
          </div>
          <span class="slot-badge" :class="{ full: mentor.availableSlots === 0 }">
            {{ mentor.availableSlots }} slots free
          </span>
        </div>

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
    </template>
  </section>
</template>
