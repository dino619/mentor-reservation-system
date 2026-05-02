<script setup>
defineProps({
  notifications: {
    type: Array,
    required: true,
  },
  busyId: {
    type: Number,
    default: null,
  },
})

defineEmits(['mark-read'])
</script>

<template>
  <section class="panel">
    <div class="section-heading">
      <div>
        <h2>Notifications</h2>
        <p class="muted">{{ notifications.filter((item) => !item.isRead).length }} unread</p>
      </div>
    </div>

    <div class="stack">
      <p v-if="!notifications.length" class="muted">No notifications yet.</p>
      <article
        v-for="notification in notifications"
        :key="notification.id"
        class="notification-row"
        :class="{ unread: !notification.isRead }"
      >
        <div>
          <h3>{{ notification.title }}</h3>
          <p>{{ notification.message }}</p>
          <p class="muted">{{ new Date(notification.createdAt).toLocaleString() }}</p>
        </div>
        <button
          v-if="!notification.isRead"
          class="button secondary"
          type="button"
          :disabled="busyId === notification.id"
          @click="$emit('mark-read', notification)"
        >
          Mark read
        </button>
      </article>
    </div>
  </section>
</template>
