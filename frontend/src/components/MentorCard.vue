<script setup>
defineProps({
  mentor: {
    type: Object,
    required: true,
  },
})

defineEmits(['apply'])
</script>

<template>
  <article class="mentor-card">
    <div class="mentor-main">
      <div>
        <h3>{{ [mentor.title, mentor.fullName].filter(Boolean).join(' ') }}</h3>
        <p class="muted">{{ mentor.email || 'Email not available on source page' }}</p>
      </div>
      <span class="slot-badge" :class="{ full: mentor.availableSlots === 0 }">
        {{ mentor.availableSlots }} / {{ mentor.maxStudents }} free
      </span>
    </div>

    <p class="lab">{{ mentor.laboratory || 'Laboratory not imported yet' }}</p>

    <div class="chips" aria-label="Research areas">
      <span v-if="!mentor.researchAreas.length" class="chip">Research areas not listed</span>
      <span v-for="area in mentor.researchAreas" :key="area" class="chip">{{ area }}</span>
    </div>

    <div class="card-actions">
      <a v-if="mentor.profileUrl" class="muted" :href="mentor.profileUrl" target="_blank" rel="noreferrer">
        FRI profile
      </a>
      <span v-else class="muted">{{ mentor.currentAcceptedStudents }} accepted students</span>
      <button
        class="button"
        type="button"
        :disabled="mentor.availableSlots === 0"
        @click="$emit('apply', mentor)"
      >
        Apply for mentorship
      </button>
    </div>
  </article>
</template>
