# Architecture Report – Lab 3: gRPC & Microservices

**Project:** PRN232 Learning Management System (LMS)  
**Author:** SE1815  
**Date:** July 2026

---

## 1. Service Decomposition

The monolithic LMS from Lab 2 was refactored into four independent deployable units plus a shared contracts library:

| Service | Responsibility | Database | Port (Docker) |
|---------|---------------|----------|---------------|
| **Identity Service** | Authentication, JWT generation, refresh tokens | `IdentityDb` | 8081 |
| **Student Service** | Student CRUD, student information | `StudentDb` | 8082 (HTTP), 8083 (gRPC) |
| **Course Service** | Course management, enrollment | `CourseDb` | 8084 |
| **API Gateway** | Single entry point, JWT validation, routing | — | 8080 |

**Shared library:** `PRN232.LMS.Contracts` – common DTOs, JWT settings, gRPC proto definitions, and exception types.

Each service follows Clean Architecture layers:
- **Controllers** – HTTP endpoints
- **Services** – business logic
- **Repositories** – data access (EF Core)
- **Entities** – domain models
- **Middleware** – logging, exception handling

Direct cross-database access is prohibited. Course Service never reads `StudentDb`; it calls Student Service via gRPC instead.

---

## 2. Database Design

Each service owns its database (SQL Server):

### IdentityDb
| Table | Key Columns |
|-------|-------------|
| `Users` | UserId, Username, PasswordHash, Role |
| `RefreshTokens` | TokenId, UserId, Token, ExpiresAt, IsRevoked |

### StudentDb
| Table | Key Columns |
|-------|-------------|
| `Students` | StudentId, FullName, Email, DateOfBirth |

### CourseDb
| Table | Key Columns |
|-------|-------------|
| `Semesters` | SemesterId, SemesterName, StartDate, EndDate |
| `Courses` | CourseId, CourseName, SemesterId (FK) |
| `Enrollments` | EnrollmentId, StudentId, CourseId, EnrollDate, Status |

`Enrollments.StudentId` is a logical reference only — no FK to StudentDb. Existence is verified at runtime via gRPC.

---

## 3. API Gateway Configuration

The gateway uses **YARP Reverse Proxy** with JWT validation before forwarding protected routes.

**Routing rules:**

| Route Pattern | Target Service |
|---------------|----------------|
| `/api/auth/*` | Identity Service (public) |
| `/api/students/*` | Student Service (JWT required) |
| `/api/courses/*` | Course Service (JWT required) |
| `/api/enrollments/*` | Course Service (JWT required) |

**JWT validation flow:**
1. Client sends `Authorization: Bearer <token>` to gateway (port 8080)
2. Gateway validates issuer, audience, signature, and expiry
3. Public routes (`/api/auth/*`, `/health`) bypass authentication
4. Protected routes return HTTP 401 if token is missing or invalid
5. Valid requests are forwarded to the appropriate backend cluster

Configuration is in `PRN232.LMS.ApiGateway/appsettings.json` under the `ReverseProxy` section.

---

## 4. gRPC Communication Flow

### Protocol Definition

File: `PRN232.LMS.Contracts/Protos/students.proto`

```protobuf
service StudentGrpc {
  rpc GetStudent (GetStudentRequest) returns (StudentResponse);
  rpc StudentExists (StudentExistsRequest) returns (StudentExistsResponse);
}
```

### Server (Student Service)

- `StudentGrpcService` implements `StudentGrpc.StudentGrpcBase`
- Exposed on HTTP/2 endpoint (port 8081 inside container, mapped to 8083 on host)
- Methods delegate to `IStudentService` for data access

### Client (Course Service)

- Strongly typed client via `AddGrpcClient<StudentGrpc.StudentGrpcClient>()`
- Wrapped in `IStudentGrpcClient` for testability
- Configured URL: `GrpcServices:StudentService` → `http://student-service:8081` (Docker)

### Enrollment Business Flow

```
Client
  │  POST /api/courses/{id}/enrollments  (JWT)
  ▼
API Gateway  ── validate JWT ──►  Course Service
                                      │
                                      │  gRPC StudentExists(studentId)
                                      ▼
                                 Student Service
                                      │
                                      │  query StudentDb
                                      ▼
                                 exists? ──no──► HTTP 400
                                      │
                                     yes
                                      │
                                      │  gRPC GetStudent(studentId)
                                      ▼
                                 create Enrollment in CourseDb
                                      │
                                      ▼
                                 HTTP 201 + studentFullName
```

Enrollment is allowed only when gRPC confirms the student exists. Invalid student IDs are rejected with HTTP 400.

---

## 5. Cross-Cutting Concerns

| Concern | Implementation |
|---------|---------------|
| **Authentication** | JWT (HS256), shared secret across gateway and services |
| **Authorization** | Role-based: `[Authorize(Roles = "Admin")]` on sensitive endpoints |
| **Logging** | Serilog – logs HTTP method, path, status code, execution time (ms) |
| **Swagger** | Each service exposes `/swagger` with Bearer JWT support |
| **Docker** | 7 containers: 3 DBs + 3 services + 1 gateway via `docker-compose.yml` |

---

## 6. Deployment

```bash
docker compose up --build
```

| Container | Host Port |
|-----------|-----------|
| api-gateway | 8080 |
| identity-service | 8081 |
| student-service | 8082 (HTTP), 8083 (gRPC) |
| course-service | 8084 |
| identity-db | 1433 |
| student-db | 1434 |
| course-db | 1435 |

**Default credentials:** `admin` / `123456` (Admin), `teacher` / `123456`, `student` / `123456`

---

## 7. Testing Summary

| Test Case | Endpoint | Expected Result |
|-----------|----------|-----------------|
| Login | `POST /api/auth/login` | JWT token generated |
| Protected API | `GET /api/students` + Bearer | HTTP 200, data returned |
| Unauthorized | `GET /api/students` (no token) | HTTP 401 |
| gRPC (enrollment) | `POST /api/courses/1/enrollments` | HTTP 201, studentFullName populated |
| Invalid student | Enroll studentId=9999 | HTTP 400 |
| Course Enrollment | Valid studentId | Enrollment completed |

Use the Postman collection at `postman/LMS-Lab3.postman_collection.json` to run all scenarios.
