# Local Development & Setup Guide

This guide provides end-to-end instructions for configuring, building, running, and testing **PlatformManagementSystem (PMS)** on your local machine.

---

## 1. Prerequisites & Tooling

Before setting up the project, ensure the following software is installed:

| Tool | Minimum Version | Installation Link |
| :--- | :--- | :--- |
| **.NET SDK** | 8.0.100 or newer | [Download .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Microsoft SQL Server** | 2019+ or LocalDB | [SQL Server Downloads](https://www.microsoft.com/sql-server/sql-server-downloads) |
| **Entity Framework CLI** | 8.0.0+ | `dotnet tool install --global dotnet-ef` |
| **IDE** | VS 2022 v17.8+ / VS Code / Rider | [Visual Studio 2022](https://visualstudio.microsoft.com/) |

Verify your environment by running:
```powershell
dotnet --version
dotnet ef --version
```

---

## 2. Cloning & Restoring the Solution

1. Open PowerShell or your preferred terminal:
   ```powershell
   git clone <repository-url>
   cd PlatformManagementSystem/PlatformManagementSystem
   ```

2. Restore NuGet dependencies across the entire solution:
   ```powershell
   dotnet restore
   ```

> [!NOTE]
> The solution contains a repository-level `nuget.config` that references the official public NuGet registry (`https://api.nuget.org/v3/index.json`).

---

## 3. Database Configuration & Migrations

PlatformManagementSystem uses **Entity Framework Core 8.0** with Microsoft SQL Server.

### 3.1 Verify Connection String
Review `PlatformManagementSystem.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PlatformManagementSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- **LocalDB (Default)**: If using Visual Studio on Windows, SQL Server LocalDB is preinstalled and requires no changes.
- **SQL Server Express or Standard Instance**: Update the connection string:
  ```json
  "DefaultConnection": "Server=localhost;Database=PlatformManagementSystemDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
  ```

### 3.2 Apply Database Migrations
Run the EF Core CLI command to create the database schema and apply all migration files:

```powershell
dotnet ef database update --project PlatformManagementSystem.Infrastructure --startup-project PlatformManagementSystem.API
```

If successful, EF Core outputs:
```
Applying migration '20260217004109_InitialCreate'...
...
Applying migration '20260221003200_AddThumbnailToCourse'...
Done.
```

---

## 4. Default Seeded Data & Credentials

When `PlatformManagementSystem.API` starts up, the `IdentitySeeder.SeedRolesAndAdminAsync` routine runs automatically:

1. **Identity Roles**: Creates `Student` and `Instructor` roles if not already present.
2. **Default Administrative User**:
   - **Email / Username**: `admin@test.com`
   - **Password**: `Admin@123`
   - **Default Role**: `Instructor`

### Accessing the Admin Dashboard:
Because the seed script assigns `admin@test.com` to the `Instructor` role, accessing endpoints protected by `[Authorize(Roles = "Admin")]` (such as `/AdminDashboard` in the MVC portal) requires the `Admin` role.

To assign the `Admin` role to `admin@test.com`, you can run this SQL command in SQL Server Management Studio (SSMS) or SQLCMD:

```sql
-- Ensure Admin role exists
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END

-- Assign Admin role to admin@test.com
DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'admin@test.com');
DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
END
```

---

## 5. Running the Application

Because PlatformManagementSystem separates the frontend (MVC) and backend (API), **both projects must run at the same time**.

### Method 1: Visual Studio Multi-Project Startup (Recommended)
1. Open `PlatformManagementSystem.sln` in Visual Studio 2022.
2. Right-click the Solution in Solution Explorer and select **Configure Startup Projects...**
3. Choose **Multiple startup projects**:
   - `PlatformManagementSystem.API` -> Action: **Start**
   - `PlatformManagementSystem.MVC` -> Action: **Start**
4. Click **Apply** and **OK**.
5. Press `Ctrl + F5` (Start Without Debugging) or `F5` (Start Debugging).
6. Visual Studio launches both services:
   - **Swagger UI**: `https://localhost:7102/swagger`
   - **MVC Web Application**: `https://localhost:7063`

---

### Method 2: Command Line (.NET CLI)

Open two terminal windows:

#### Terminal 1 — Start the API:
```powershell
dotnet run --project PlatformManagementSystem.API
```
*Listens on:*
- HTTPS: `https://localhost:7102`
- HTTP: `http://localhost:5092`

#### Terminal 2 — Start the MVC Frontend:
```powershell
dotnet run --project PlatformManagementSystem.MVC
```
*Listens on:*
- HTTPS: `https://localhost:7063`
- HTTP: `http://localhost:5176`

Navigate to `https://localhost:7063` in your browser.

---

## 6. Verifying the Setup

1. **Verify Backend Health**:
   - Open `https://localhost:7102/swagger`.
   - Verify the Swagger UI renders and displays all API endpoints (`Auth`, `Course`, `Lesson`, `Exam`, `LiveSession`, `Dashboard`, `Admin`, etc.).
2. **Verify Frontend UI**:
   - Open `https://localhost:7063`.
   - Verify that the home page displays the hero banner and trending courses.
3. **Verify Authentication**:
   - Navigate to `https://localhost:7063/Account/Login`.
   - Sign in with:
     - Email: `admin@test.com`
     - Password: `Admin@123`
   - You will be redirected to the **Instructor Dashboard** (`/InstructorDashboard`).
4. **Verify Live Whiteboard**:
   - As an instructor, navigate to `/LiveSession/Board?sessionId=1&courseId=1`.
   - Test drawing on the canvas to ensure SignalR connects successfully without errors in the browser console.

---

## 7. Configuration Reference

### 7.1 Port Allocation Map

| Project | Protocol | Port | Description |
| :--- | :--- | :--- | :--- |
| **API** | HTTPS | `7102` | Primary secure API endpoint |
| **API** | HTTP | `5092` | Unencrypted fallback port |
| **MVC** | HTTPS | `7063` | Primary secure web portal |
| **MVC** | HTTP | `5176` | Unencrypted fallback port |
| **MVC (IIS Express)** | HTTPS | `44359` | IIS Express SSL port |
| **MVC (IIS Express)** | HTTP | `30398` | IIS Express standard port |

### 7.2 Cross-Origin Resource Sharing (CORS)
The API configures a dedicated CORS policy named `"AllowMVC"` in `PlatformManagementSystem.API/Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins(
                  "https://localhost:7063",
                  "http://localhost:5176",
                  "https://localhost:44359",
                  "http://localhost:30398")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

If you host the MVC frontend under a different port or custom domain, update this policy accordingly.

---

## 8. Production Deployment Guidelines

When deploying PlatformManagementSystem to staging or production:

1. **Replace JWT Secrets**:
   - Override `JWT:Key` in `appsettings.json` with a cryptographically secure 256-bit key via environment variables or secret vaults:
     ```powershell
     $env:JWT__Key = "YOUR_SECURE_PRODUCTION_KEY_OF_AT_LEAST_32_CHARACTERS"
     ```
2. **Database Engine**:
   - Migrate from LocalDB to a production-grade SQL Server instance (e.g., Azure SQL Database, AWS RDS SQL Server, or dedicated SQL Server VM).
3. **HTTPS & Cookie Security**:
   - Ensure the reverse proxy (Nginx, IIS, Cloudflare) enforces HTTPS.
   - Configure ASP.NET Core Forwarded Headers Middleware (`UseForwardedHeaders`) if terminating SSL at a reverse proxy.
4. **Disable Swagger in Production**:
   - The API pipeline automatically restricts Swagger to `Development`:
     ```csharp
     if (app.Environment.IsDevelopment())
     {
         app.UseSwagger();
         app.UseSwaggerUI();
     }
     ```
