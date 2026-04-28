# Mentor Reservation System

Mentor Reservation System is a full-stack prototype for a university seminar project in the course **Analysis and Improvement of Business Processes**.

The project improves the informal process students currently use before official bachelor thesis registration in STUDIS. It does **not** replace STUDIS. Instead, it supports the phase where students search for a mentor, check availability, submit a structured request, and wait for the mentor's decision.

## Project Topic

**Mentor Selection System for Bachelor's Thesis Supervision**

Team members:

- Dino Džaferagić
- Tibor Koderman
- Jerneja Krajcar

## Business Process Context

### Current AS-IS Process

Students usually choose a thesis mentor informally:

1. The student identifies an area of interest.
2. The student manually checks mentors, laboratories, or faculty pages.
3. The student contacts a mentor by email, in person, or through another informal channel.
4. The student waits for a response.
5. If the mentor agrees, the student officially registers the thesis topic and mentor in STUDIS.
6. The mentor confirms the registration in STUDIS.

Main problems:

- students do not clearly see mentor availability,
- supervision capacity is not transparent,
- mentor research areas are scattered across different sources,
- students do not know whether a request was seen,
- mentors receive requests through multiple informal channels,
- requests are hard to track.

### Improved TO-BE Process

The prototype supports a clearer pre-STUDIS process:

1. The student opens the application.
2. The student browses mentors, research areas, and available slots.
3. The student submits a structured mentorship request.
4. The mentor sees incoming requests in one dashboard.
5. The mentor accepts or rejects the request and can write a short comment.
6. The student tracks request status.
7. If accepted, the student can continue with official STUDIS registration.

## Features

### Student Features

- Select a seeded student user for prototype login.
- Browse all mentors.
- See mentor name, email, laboratory, research areas, maximum capacity, accepted students, and available slots.
- Search mentors by name, laboratory, or research area.
- Submit a mentorship request with:
  - proposed thesis title,
  - thesis idea or description,
  - optional message to the mentor.
- View submitted requests and their statuses:
  - Pending,
  - Accepted,
  - Rejected,
  - Cancelled.
- See mentor comments after acceptance or rejection.
- Cancel pending requests.

### Mentor Features

- Select a seeded mentor user for prototype login.
- View mentor capacity summary:
  - accepted students,
  - available slots,
  - pending requests.
- View all incoming mentorship requests.
- See student name, email, proposed thesis title, description, optional message, status, and creation date.
- Accept pending requests.
- Reject pending requests.
- Add an optional comment or reason when accepting or rejecting.
- Capacity rule prevents accepting students beyond the mentor limit.

### Prototype Login

The application uses simple seeded users instead of real university authentication. This keeps the prototype easy to demonstrate and avoids integration with university login systems.

## Business Rules

- Each mentor can supervise a maximum of **5 students**.
- Accepted requests reduce the mentor's available slots.
- Students cannot submit a new active request to the same mentor if they already have a pending or accepted request with that mentor.
- Mentors can only accept or reject pending requests.
- Mentors cannot accept a request if their capacity is full.
- A student cannot have more than one accepted mentorship request.
- Cancelled and rejected requests stay visible for tracking/history.

## Technology Stack

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- Npgsql Entity Framework Core provider
- PostgreSQL
- REST API

### Frontend

- Vue 3
- Vite
- Vue Router
- Fetch API for backend communication
- Local storage for prototype session state

### Local Development

- Docker Compose
- PostgreSQL container

## Project Structure

```text
mentor-reservation-system/
  backend/
    Controllers/      REST API controllers
    Data/             EF Core DbContext and seed data
    DTOs/             API request/response models
    Migrations/       EF Core database migrations
    Models/           Database entities and enums
    Services/         Business logic and validation
    Program.cs        API startup configuration

  frontend/
    src/
      api/            API client and prototype session helpers
      components/     Reusable Vue components
      router/         Vue Router configuration
      views/          Student, mentor, and login pages
      App.vue         Main layout
      style.css       Application styling

  docker-compose.yml  Local PostgreSQL setup
  README.md
```

## Requirements

Install these before running the project:

- .NET SDK 10 or newer
- Node.js and npm
- Docker Desktop

## How to Run

Open a terminal in the project root:

```powershell
cd C:\Users\dino8\Desktop\Sola\MAG\1_letnik\ARP2\mentor-reservation-system
```

### 1. Start PostgreSQL

```powershell
docker compose up -d postgres
```

The PostgreSQL container is exposed on:

```text
localhost:5434
```

The project uses port `5434` because port `5432` is often already used by another local PostgreSQL installation or container.

### 2. Start the Backend

Open a terminal:

```powershell
cd backend
dotnet run --urls http://localhost:5266
```

The backend will:

- connect to PostgreSQL,
- apply EF Core migrations automatically,
- seed demo users, mentors, and example requests if the database is empty,
- start the API on `http://localhost:5266`.

### 3. Start the Frontend

Open a second terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open the app in a browser:

```text
http://127.0.0.1:5173
```

## Demo Flow

### Student Demo

1. Open `http://127.0.0.1:5173`.
2. Select the **Student** role.
3. Choose a seeded student user.
4. Browse mentors and use the search field.
5. Click **Apply for mentorship**.
6. Fill in the request form and submit.
7. Check the request status in **My requests**.

### Mentor Demo

1. Click **Switch user**.
2. Select the **Mentor** role.
3. Choose the mentor who received the request.
4. Review incoming requests.
5. Write an optional response.
6. Accept or reject the request.
7. Switch back to the student and confirm the updated status.

## Seed Data

### Students

- Dino Džaferagić
- Tibor Koderman
- Jerneja Krajcar
- Maja Vidmar

### Mentors

- doc. dr. Ana Novak
- izr. prof. dr. Marko Horvat
- prof. dr. Petra Zupan
- asist. dr. Luka Kovač

### Example Mentor Areas

- Data mining
- Databases
- Business intelligence
- Machine learning
- Natural language processing
- Recommendation systems
- Computer networks
- Cybersecurity
- Distributed systems
- Software engineering
- Web applications
- Process improvement

## API Endpoints

Base URL:

```text
http://localhost:5266/api
```

Available endpoints:

- `GET /users`
- `GET /users?role=Student`
- `GET /users?role=Mentor`
- `GET /mentors`
- `GET /mentors?search=machine`
- `GET /mentors/{id}`
- `GET /mentors/{id}/requests`
- `GET /students/{id}/requests`
- `POST /requests`
- `POST /requests/{id}/accept`
- `POST /requests/{id}/reject`
- `POST /requests/{id}/cancel`

Example request body for `POST /api/requests`:

```json
{
  "studentId": 5,
  "mentorId": 1,
  "proposedTitle": "Dashboard for Transparent Mentor Availability",
  "description": "A prototype dashboard that helps students see mentor capacity and submit structured supervision requests.",
  "optionalMessage": "I would like to focus on a practical web prototype."
}
```

Example request body for accepting or rejecting:

```json
{
  "comment": "Accepted. Please prepare a short topic scope before our first meeting."
}
```

## Database Configuration

Connection string:

```text
Host=localhost;Port=5434;Database=mentor_reservation;Username=mentor_app;Password=mentor_app_password
```

It is configured in:

```text
backend/appsettings.json
backend/appsettings.Development.json
```

Docker Compose creates:

- database: `mentor_reservation`
- user: `mentor_app`
- password: `mentor_app_password`

## Useful Commands

Build backend:

```powershell
cd backend
dotnet build
```

Build frontend:

```powershell
cd frontend
npm run build
```

Stop PostgreSQL:

```powershell
docker compose down
```

Reset the database completely:

```powershell
docker compose down -v
docker compose up -d postgres
```

After resetting the database, start the backend again so migrations and seed data are applied.

## Troubleshooting

### Backend executable is locked

If `dotnet run` says `MentorReservation.Api.exe` is being used by another process, the backend is already running.

Stop the process shown in the error:

```powershell
taskkill /PID <PID_FROM_ERROR> /F
```

Then run the backend again.

### PostgreSQL port conflict

This project uses host port `5434`. If that port is also taken, change the port mapping in `docker-compose.yml` and update the backend connection string.

### Frontend cannot reach the backend

Check that the backend is running on:

```text
http://localhost:5266
```

The frontend API client uses that URL by default.

## Project Scope

This is a realistic university seminar prototype. It focuses on the business process improvement before official STUDIS registration. It intentionally avoids:

- real university authentication,
- official STUDIS integration,
- email notifications,
- complex admin functionality,
- production deployment configuration.

Those features could be added later, but they are outside the current prototype scope.
