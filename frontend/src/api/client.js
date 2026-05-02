const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5266/api'

async function request(path, options = {}) {
  const response = await fetch(`${API_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    ...options,
  })

  if (!response.ok) {
    let message = `Request failed with status ${response.status}`

    try {
      const error = await response.json()
      message = error.message ?? message
    } catch {
      // Keep the generic message if the server returns an empty body.
    }

    throw new Error(message)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

export const api = {
  registerStudent(payload) {
    return request('/auth/register-student', {
      method: 'POST',
      body: JSON.stringify(payload),
    })
  },

  login(payload) {
    return request('/auth/login', {
      method: 'POST',
      body: JSON.stringify(payload),
    })
  },

  getUsers(role) {
    const query = role ? `?role=${encodeURIComponent(role)}` : ''
    return request(`/users${query}`)
  },

  getMentors(search) {
    const query = search ? `?search=${encodeURIComponent(search)}` : ''
    return request(`/mentors${query}`)
  },

  getStudentRequests(studentId) {
    return request(`/students/${studentId}/requests`)
  },

  getMentorRequests(mentorId) {
    return request(`/mentors/${mentorId}/requests`)
  },

  importMentors() {
    return request('/mentors/import', {
      method: 'POST',
      body: JSON.stringify({}),
    })
  },

  getImportRuns() {
    return request('/import-runs')
  },

  createRequest(payload) {
    return request('/requests', {
      method: 'POST',
      body: JSON.stringify(payload),
    })
  },

  acceptRequest(id, comment) {
    return request(`/requests/${id}/accept`, {
      method: 'POST',
      body: JSON.stringify({ response: comment }),
    })
  },

  rejectRequest(id, comment) {
    return request(`/requests/${id}/reject`, {
      method: 'POST',
      body: JSON.stringify({ response: comment }),
    })
  },

  cancelRequest(id) {
    return request(`/requests/${id}/cancel`, {
      method: 'POST',
      body: JSON.stringify({}),
    })
  },

  getNotifications(userId) {
    return request(`/notifications/user/${userId}`)
  },

  markNotificationRead(id) {
    return request(`/notifications/${id}/read`, {
      method: 'POST',
      body: JSON.stringify({}),
    })
  },

  getEmailOutbox() {
    return request('/email-outbox')
  },
}
