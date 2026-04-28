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
        <h3>{{ mentor.fullName }}</h3>
        <p class="muted">{{ mentor.email }}</p>
      </div>
      <span class="slot-badge" :class="{ full: mentor.availableSlots === 0 }">
        {{ mentor.availableSlots }} / {{ mentor.maxStudents }} free
      </span>
    </div>

    <p class="lab">{{ mentor.laboratory }}</p>

    <div class="chips" aria-label="Research areas">
      <span v-for="area in mentor.researchAreas" :key="area" class="chip">{{ area }}</span>
    </div>

    <div class="card-actions">
      <span class="muted">{{ mentor.currentAcceptedStudents }} accepted students</span>
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
