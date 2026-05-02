# Mentor Reservation System

Full-stack university seminar prototype for improving the pre-STUDIS mentor selection process for bachelor thesis supervision.

The application does not replace STUDIS. It supports the earlier phase where a student finds a mentor, checks available supervision slots, sends a structured request, and tracks the mentor's decision.

## Features

- Student registration with hashed password storage.
- Simple login plus demo user switching for presentations.
- Real mentor profile import from the public FRI mentor page.
- Robust local import from `https___www.fri.uni-lj.si_sl_mentorji.htm`, including browser `view-source` exports.
- Mentor list with search by name, title, email, laboratory, and research area.
- Structured mentorship request submission.
- Mentor dashboard with accept/reject actions and optional response.
- Capacity rule: each mentor has 5 slots by default.
- Available slots are calculated from accepted requests.
- Students may have multiple pending requests but only one accepted request.
- In-app notifications for request decisions.
- EmailOutbox table for development/admin inspection of email messages.

## Current Process Rule TODO

We still need to confirm with the professor whether accepting one request should automatically cancel the student's other pending requests. For now, the system does not cancel them automatically.

## Technology Stack

- Backend: C# ASP.NET Core Web API
- ORM: Entity Framework Core
- Database: PostgreSQL with Npgsql
- HTML parsing: HtmlAgilityPack
- Frontend: Vue 3, Vite, Vue Router
- Local database: Docker Compose

## Project Structure

```text
backend/
  Controllers/
  Data/
  DTOs/
  Migrations/
  Models/
  Services/

frontend/
  src/api/
  src/components/
  src/router/
  src/views/

docker-compose.yml
```

## Run Locally

Start Docker Desktop first.

From the project root:

```powershell
docker compose up -d postgres
```

The database is exposed on:

```text
localhost:5434
```

Start the backend:

```powershell
cd backend
dotnet run --urls http://localhost:5266
```

The backend applies EF Core migrations automatically on startup.

Start the frontend in a second terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open:

```text
http://127.0.0.1:5173
```

## Fresh Database Reset

For the cleanest demo after schema changes:

```powershell
docker compose down -v
docker compose up -d postgres
cd backend
dotnet run --urls http://localhost:5266
```

Then import mentors:

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5266/api/mentors/import
```

## Mentor Import

The importer first looks for this local file in the project root:

```text
https___www.fri.uni-lj.si_sl_mentorji.htm
```

If the file is missing, it fetches:

```text
https://www.fri.uni-lj.si/sl/mentorji
```

The local file can be either:

- normal raw HTML,
- browser `view-source` saved HTML.

Imported mentor data:

- title,
- first name,
- last name,
- profile URL,
- research areas when listed.

Fields such as email, office, phone, and laboratory may remain empty because the mentor list page does not reliably expose them.

## Seed Users

Seeded users use:

```text
Password123!
```

Example seeded accounts:

- `dino.dzaferagic@student.uni-lj.si`
- `tibor.koderman@student.uni-lj.si`
- `jerneja.krajcar@student.uni-lj.si`
- `maja.vidmar@student.uni-lj.si`
- `mentor.demo@fri.uni-lj.si`
- `admin@mentor-reservation.local`

## Main API Endpoints

```text
POST /api/auth/register-student
POST /api/auth/login
GET  /api/users
GET  /api/mentors
GET  /api/mentors/{id}
POST /api/mentors/import
GET  /api/import-runs
GET  /api/students/{id}/requests
GET  /api/mentors/{id}/requests
POST /api/requests
POST /api/requests/{id}/accept
POST /api/requests/{id}/reject
POST /api/requests/{id}/cancel
GET  /api/notifications/user/{userId}
POST /api/notifications/{id}/read
GET  /api/email-outbox
```

## Test Flow

1. Start PostgreSQL, backend, and frontend.
2. Run `POST /api/mentors/import`.
3. Open the frontend.
4. Register a new student or login with a seeded student.
5. Search imported mentors.
6. Submit a mentorship request.
7. Switch to the demo mentor user.
8. Select the mentor context in the mentor dashboard.
9. Accept or reject the request.
10. Switch back to the student and check request status and notifications.
11. Inspect `GET /api/email-outbox` to see generated email messages.

## Build Commands

Backend:

```powershell
cd backend
dotnet build
```

Frontend:

```powershell
cd frontend
npm run build
```

## Prototype Limitations

- No real university login integration.
- No JWT or production authentication flow.
- No SMTP sending; email messages are stored in `EmailOutbox`.
- Mentor account invitation/linking is prepared in the schema but not fully implemented as an admin workflow.
- Imported mentor contact details depend on what is available on the public FRI mentor page.
