# REST API & SignalR Hub Reference

This document provides a comprehensive technical reference for all REST endpoints and SignalR Hubs exposed by **PlatformManagementSystem.API** (`https://localhost:7102`).

---

## 1. Authentication & Security Conventions

- **Base URL**: `https://localhost:7102/api/`
- **Security Scheme**: HTTP Bearer Authentication (JWT).
- **Header Format**: `Authorization: Bearer <JWT_TOKEN>`
- **SignalR Query Parameter**: `?access_token=<JWT_TOKEN>`
- **Content Type**: `application/json`

---

## 2. Authentication API (`/api/Auth`)

### 2.1 User Login
Authenticates user credentials and issues a signed JWT token.

- **Endpoint**: `POST /api/Auth/login`
- **Access**: Anonymous (Public)
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "password": "Password123!"
  }
  ```
- **Success Response (200 OK)**:
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "user@example.com",
    "role": "Student",
    "userId": "d72bcf74-889d-4e92-a10c-f3708848e421"
  }
  ```
- **Error Response (401 Unauthorized)**: Returned if the user does not exist or the password is invalid.

---

### 2.2 User Registration
Registers a new application user account.

- **Endpoint**: `POST /api/Auth/register`
- **Access**: Anonymous (Public)
- **Request Body**:
  ```json
  {
    "fullName": "Jane Doe",
    "email": "janedoe@example.com",
    "password": "SecurePassword123!",
    "role": "Student"
  }
  ```
- **Success Response (200 OK)**: `"User Created"`
- **Error Response (400 Bad Request)**: Validation errors or identity constraint failures (e.g., password complexity, email already exists).

---

## 3. Course Management API (`/api/Course`)

### 3.1 Get All Courses
- **Endpoint**: `GET /api/Course`
- **Access**: Anonymous (Public)
- **Response (200 OK)**:
  ```json
  [
    {
      "id": 1,
      "title": "ASP.NET Core Architecture",
      "description": "Comprehensive guide to Clean Architecture in .NET 8."
    }
  ]
  ```

---

### 3.2 Create Course
- **Endpoint**: `POST /api/Course`
- **Access**: `Instructor`
- **Request Body**:
  ```json
  {
    "title": "Mastering Microservices",
    "description": "Building cloud-native distributed systems.",
    "price": 49.99,
    "categoryId": 2
  }
  ```
- **Response (200 OK)**:
  ```json
  {
    "message": "Course Created Successfully"
  }
  ```

---

### 3.3 Get Courses by Instructor
- **Endpoint**: `GET /api/Course/ByInstructor/{instructorId}`
- **Access**: `Instructor`
- **Response (200 OK)**:
  ```json
  [
    {
      "courseId": 1,
      "title": "Mastering Microservices",
      "description": "Building cloud-native distributed systems.",
      "lessonsCount": 8,
      "studentsCount": 24
    }
  ]
  ```

---

### 3.4 Get Instructor Aggregate Statistics
- **Endpoint**: `GET /api/Course/InstructorStats/{instructorId}`
- **Access**: `Instructor`
- **Response (200 OK)**:
  ```json
  {
    "totalCourses": 4,
    "totalStudents": 112,
    "totalExams": 6
  }
  ```

---

### 3.5 Get Course Details
- **Endpoint**: `GET /api/Course/details/{id}`
- **Access**: Anonymous (Public)
- **Response (200 OK)**:
  ```json
  {
    "id": 1,
    "title": "ASP.NET Core Architecture",
    "description": "Detailed syllabus...",
    "price": 29.99,
    "instructorName": "Dr. Alan Turing",
    "studentsCount": 45,
    "lessons": [
      {
        "id": 101,
        "title": "Introduction to Dependency Injection"
      }
    ]
  }
  ```

---

## 4. Lesson Management API (`/api/Lesson`)

### 4.1 Get Lessons by Course
- **Endpoint**: `GET /api/Lesson/course/{courseId}`
- **Access**: Anonymous (Public)
- **Response (200 OK)**:
  ```json
  [
    {
      "id": 101,
      "title": "Introduction to Clean Architecture",
      "content": "Lesson markdown or rich text content..."
    }
  ]
  ```

---

### 4.2 Create Lesson
- **Endpoint**: `POST /api/Lesson`
- **Access**: `Instructor`
- **Request Body**:
  ```json
  {
    "courseId": 1,
    "title": "Configuring Entity Framework Core",
    "content": "In this module we explore DbContext pooling..."
  }
  ```
- **Response (200 OK)**:
  ```json
  {
    "id": 102
  }
  ```

---

### 4.3 Delete Lesson
- **Endpoint**: `DELETE /api/Lesson/{id}`
- **Access**: `Instructor`
- **Response (200 OK)**: Status code `200 OK` on successful removal.

---

## 5. Enrollment & Progress API (`/api/Enrollment`)

### 5.1 Enroll Student in Course
- **Endpoint**: `POST /api/Enrollment/{courseId}`
- **Access**: `Student`
- **Response (200 OK)**: `"Enrolled successfully"`

---

### 5.2 Get Student Enrollments
- **Endpoint**: `GET /api/Enrollment/my-courses`
- **Access**: `Student`
- **Response (200 OK)**: List of `Enrollment` records for the authenticated student.

---

### 5.3 Update Lesson Progress
- **Endpoint**: `PUT /api/Enrollment/progress?courseId={courseId}&progress={progressPercentage}`
- **Access**: `Student`
- **Response (200 OK)**: `"Progress updated"`

---

### 5.4 Get Students in Course
- **Endpoint**: `GET /api/Enrollment/ByCourse/{courseId}`
- **Access**: `Instructor`
- **Response (200 OK)**:
  ```json
  [
    {
      "studentId": "d72bcf74-889d-4e92-a10c-f3708848e421",
      "enrollmentDate": "2026-03-01T10:15:30Z",
      "progressPercentage": 75,
      "isCompleted": false
    }
  ]
  ```

---

## 6. Exam & Assessment API (`/api/Exam`)

### 6.1 Create Exam
- **Endpoint**: `POST /api/Exam/create?courseId={courseId}&title={title}&duration={duration}`
- **Alternative**: `POST /api/Exam` with JSON body:
  ```json
  {
    "courseId": 1,
    "title": "Midterm Examination",
    "durationMinutes": 60
  }
  ```
- **Access**: `Instructor`
- **Response (200 OK)**: Newly created integer `ExamId`.

---

### 6.2 Add Question with Answers
- **Endpoint**: `POST /api/Exam/AddQuestion`
- **Access**: `Instructor`
- **Request Body**:
  ```json
  {
    "examId": 5,
    "text": "What is the primary benefit of the Dependency Inversion Principle?",
    "answers": [
      { "text": "It decouples high-level modules from low-level implementations", "isCorrect": true },
      { "text": "It speeds up database execution", "isCorrect": false },
      { "text": "It reduces memory usage", "isCorrect": false },
      { "text": "It eliminates the need for unit testing", "isCorrect": false }
    ]
  }
  ```
- **Response (200 OK)**: Status code `200 OK`.

---

### 6.3 Get Exam by ID
- **Endpoint**: `GET /api/Exam/{examId}`
- **Access**: `Instructor`, `Student`
- **Response (200 OK)**:
  ```json
  {
    "id": 5,
    "title": "Midterm Examination",
    "questions": [
      {
        "id": 20,
        "text": "What is the primary benefit of the Dependency Inversion Principle?",
        "answers": [
          { "id": 81, "text": "It decouples high-level modules...", "isCorrect": true },
          { "id": 82, "text": "It speeds up database execution", "isCorrect": false }
        ]
      }
    ]
  }
  ```

---

### 6.4 Submit Exam Answers
- **Endpoint**: `POST /api/Exam/{examId}/submit`
- **Alternative**: `POST /api/Exam/Submit` with JSON body:
  ```json
  {
    "examId": 5,
    "submittedAnswers": {
      "20": 81,
      "21": 85
    }
  }
  ```
- **Access**: `Student`
- **Response (200 OK)**:
  ```json
  {
    "score": 2
  }
  ```

---

### 6.5 Get Exams by Course
- **Endpoint**: `GET /api/Exam/ByCourse/{courseId}`
- **Access**: `Instructor`
- **Response (200 OK)**: List of `ExamDto` objects.

---

## 7. Dashboard API (`/api/Dashboard`)

| Route | Method | Access | Description |
| :--- | :--- | :--- | :--- |
| `/api/Dashboard/admin` | `GET` | `Admin` | Returns total students, instructors, courses, and approved counts |
| `/api/Dashboard/instructor` | `GET` | `Instructor` | Returns course counts and total enrolled students |
| `/api/Dashboard/student` | `GET` | `Student` | Returns enrolled courses and completed course metrics |
| `/api/Dashboard/StudentStats` | `GET` | `Student` | Returns `{ totalCourses, completedCourses, pendingCourses }` |
| `/api/Dashboard/MyCoursesDetailed`| `GET` | `Student` | Detailed course progress cards |
| `/api/Dashboard/MyExamResults` | `GET` | `Student` | Historical exam scores |
| `/api/Dashboard/CourseDetails/{id}`| `GET` | `Student` | Syllabus and completion checkboxes per lesson |
| `/api/Dashboard/GenerateCertificate/{id}`| `GET`| `Student`| Returns PDF stream (or 400 if course is incomplete) |

---

## 8. Administration API (`/api/Admin`)

All endpoints require `[Authorize(Roles = "Admin")]`.

### 8.1 Platform Statistics
- **Endpoint**: `GET /api/Admin/PlatformStats`
- **Response (200 OK)**:
  ```json
  {
    "totalUsers": 25,
    "totalCourses": 12,
    "totalEnrollments": 84,
    "totalExams": 16
  }
  ```

---

### 8.2 User Management
- `GET /api/Admin/Users`: Returns user list with IDs, emails, full names, and roles.
- `DELETE /api/Admin/DeleteUser/{id}`: Deletes user by GUID identifier.

---

### 8.3 Role Management
- `GET /api/Admin/Roles`: Lists all Identity roles (`Id`, `Name`).
- `POST /api/Admin/CreateRole`: Creates a new role (`{"roleName": "Manager"}`).
- `DELETE /api/Admin/DeleteRole/{roleName}`: Removes an existing role.
- `POST /api/Admin/AssignRole`: Adds a user to a role (`{"userId": "...", "roleName": "..."}`).
- `POST /api/Admin/RemoveRole`: Removes a user from a role.

---

## 9. Live Session API (`/api/LiveSession`)

### 9.1 Create Live Session
- **Endpoint**: `POST /api/LiveSession/Create?courseId={id}&title={title}&startTime={isoDate}&meetingUrl={url}`
- **Access**: `Instructor`
- **Response (200 OK)**: Returns integer `LiveSessionId`.

---

### 9.2 Get Live Sessions by Course
- **Endpoint**: `GET /api/LiveSession/ByCourse/{courseId}`
- **Access**: Anonymous (Public)
- **Response (200 OK)**: List of `LiveSessionDto` objects.

---

### 9.3 Start Live Session
- **Endpoint**: `POST /api/LiveSession/Start/{id}?courseId={courseId}`
- **Access**: `Instructor`
- **Side Effect**: Marks session `IsActive = true` and notifies SignalR group `Course_{courseId}`.

---

### 9.4 End Live Session
- **Endpoint**: `POST /api/LiveSession/End/{id}`
- **Access**: `Instructor`
- **Response (200 OK)**: Sets `IsActive = false` and records `EndTime`.

---

## 10. Real-Time SignalR Hubs

### 10.1 LiveBoardHub (`/liveboardHub`)
Requires authentication. Tokens must be passed via query string `?access_token={JWT}` or `accessTokenFactory`.

#### Client Invocations (Client -> Server):
- `JoinSessionGroup(int sessionId)`: Adds connection to `session-{sessionId}`.
- `LeaveSessionGroup(int sessionId)`: Removes connection from `session-{sessionId}`.
- `Draw(int sessionId, object drawData)`: Broadcasts drawing packet to session group.
- `SendMessage(int sessionId, string message)`: Broadcasts chat message to session group.

#### Client Events (Server -> Client):
- `ReceiveDraw(object drawData)`: Receives real-time vector or raster draw payload:
  ```json
  {
    "type": "freehand",
    "startX": 120,
    "startY": 140,
    "endX": 150,
    "endY": 180,
    "color": "#007bff"
  }
  ```
- `ReceiveMessage(string userName, string message)`: Receives chat broadcast.

---

### 10.2 LiveSessionHub (`/liveSessionHub`)
Requires authentication.

#### Client Invocations:
- `JoinCourseGroup(int courseId)`: Adds connection to `Course_{courseId}`.

#### Client Events:
- `SessionStarted(int courseId)`: Fired when an instructor triggers `/api/LiveSession/Start/{id}`.
