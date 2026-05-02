<script setup>
defineProps({
  request: {
    type: Object,
    required: true,
  },
  mode: {
    type: String,
    default: 'student',
  },
  comment: {
    type: String,
    default: '',
  },
  busy: {
    type: Boolean,
    default: false,
  },
})

defineEmits(['accept', 'reject', 'cancel', 'update:comment'])
</script>

<template>
  <article class="request-row">
    <div class="request-title">
      <div>
        <h3>{{ request.proposedTitle }}</h3>
        <p class="muted">
          <span v-if="mode === 'mentor'">{{ request.studentName }} · {{ request.studentEmail }}</span>
          <span v-else>{{ request.mentorName }} · {{ request.mentorEmail || 'No mentor email' }}</span>
        </p>
      </div>
      <span class="status" :class="request.status.toLowerCase()">{{ request.status }}</span>
    </div>

    <p>{{ request.description }}</p>
    <p v-if="request.optionalMessage" class="message">{{ request.optionalMessage }}</p>
    <p v-if="request.mentorResponse" class="mentor-comment">{{ request.mentorResponse }}</p>

    <div class="request-meta">
      <span>Created {{ new Date(request.createdAt).toLocaleDateString() }}</span>
      <span>Updated {{ new Date(request.updatedAt).toLocaleDateString() }}</span>
    </div>

    <div v-if="mode === 'mentor' && request.status === 'Pending'" class="decision-box">
      <input
        :value="comment"
        placeholder="Optional response or reason"
        @input="$emit('update:comment', $event.target.value)"
      />
      <button class="button success" type="button" :disabled="busy" @click="$emit('accept')">Accept</button>
      <button class="button danger" type="button" :disabled="busy" @click="$emit('reject')">Reject</button>
    </div>

    <div v-if="mode === 'student' && request.status === 'Pending'" class="form-actions">
      <button class="button secondary" type="button" :disabled="busy" @click="$emit('cancel')">Cancel request</button>
    </div>
  </article>
</template>
