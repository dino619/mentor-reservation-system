<script setup>
import { reactive, ref, watch } from 'vue'
import { api } from '../api/client'

const props = defineProps({
  mentor: {
    type: Object,
    required: true,
  },
  student: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['created', 'cancel'])

const form = reactive({
  proposedTitle: '',
  description: '',
  optionalMessage: '',
})

const loading = ref(false)
const error = ref('')

watch(
  () => props.mentor.id,
  () => {
    error.value = ''
  }
)

async function submit() {
  loading.value = true
  error.value = ''

  try {
    const created = await api.createRequest({
      studentId: props.student.id,
      mentorId: props.mentor.id,
      proposedTitle: form.proposedTitle,
      description: form.description,
      optionalMessage: form.optionalMessage || null,
    })

    form.proposedTitle = ''
    form.description = ''
    form.optionalMessage = ''
    emit('created', created)
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <section class="panel">
    <div class="section-heading">
      <div>
        <h2>Request mentorship</h2>
        <p class="muted">{{ mentor.fullName }}</p>
      </div>
      <button class="icon-button" type="button" title="Close request form" @click="$emit('cancel')">X</button>
    </div>

    <form class="stack" @submit.prevent="submit">
      <label>
        Proposed thesis title
        <input v-model.trim="form.proposedTitle" required minlength="5" maxlength="180" />
      </label>

      <label>
        Thesis idea or description
        <textarea v-model.trim="form.description" required minlength="20" maxlength="3000" rows="5"></textarea>
      </label>

      <label>
        Optional message to mentor
        <textarea v-model.trim="form.optionalMessage" maxlength="1500" rows="3"></textarea>
      </label>

      <p v-if="error" class="alert error">{{ error }}</p>

      <div class="form-actions">
        <button class="button secondary" type="button" @click="$emit('cancel')">Cancel</button>
        <button class="button" type="submit" :disabled="loading || mentor.availableSlots === 0">
          {{ loading ? 'Sending...' : 'Send request' }}
        </button>
      </div>
    </form>
  </section>
</template>
