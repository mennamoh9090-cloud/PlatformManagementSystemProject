# Troubleshooting & Diagnostic Guide

This document lists common issues, root causes, and step-by-step diagnostic procedures for **PlatformManagementSystem (PMS)**.

---

## 1. Database & Persistence Issues

### 1.1 `SqlException: Cannot open database "PlatformManagementSystemDB"`
- **Symptoms**: The application crashes during startup with `SqlException: Cannot open database "PlatformManagementSystemDB" requested by the login.` or EF Core migration fails.
- **Root Cause**: The database has not been created yet or migrations have not been applied to your SQL Server instance.
- **Resolution**:
  1. Verify the connection string in `PlatformManagementSystem.API/appsettings.json`.
  2. Run the migration command from the solution root:
     ```powershell
     dotnet ef database update --project PlatformManagementSystem.Infrastructure --startup-project PlatformManagementSystem.API
     ```
  3. If using LocalDB, verify that the LocalDB instance is running:
     ```powershell
     sqllocaldb start mssqllocaldb
     sqllocaldb info mssqllocaldb
     ```

---

### 1.2 `SqlException: A network-related or instance-specific error occurred...`
- **Symptoms**: Connection timeout or error 26 / error 40 when trying to reach SQL Server.
- **Root Cause**: The SQL Server service is stopped, or the instance name in the connection string does not match your local environment.
- **Resolution**:
  - Open Windows **Services** (`services.msc`) and ensure `SQL Server (MSSQLSERVER)` or `SQL Server (SQLEXPRESS)` is running.
  - If using a named instance (such as SQLEXPRESS), update `appsettings.json`:
    ```json
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PlatformManagementSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
    ```

---

## 2. API & Network Connectivity Issues

### 2.1 MVC Throws `HttpRequestException: No connection could be made because the target machine actively refused it`
- **Symptoms**: Logging into the MVC application or loading course lists results in an unhandled HTTP exception or a blank page.
- **Root Cause**: The MVC application communicates with the backend via `https://localhost:7102/api/`. If `PlatformManagementSystem.API` is not running, outbound HTTP calls fail.
- **Resolution**:
  - Ensure **both projects are running concurrently**.
  - In Visual Studio, verify that the solution is configured for **Multiple Startup Projects** (both `PlatformManagementSystem.API` and `PlatformManagementSystem.MVC`).
  - If using the CLI, ensure you executed `dotnet run --project PlatformManagementSystem.API` in a dedicated terminal window.

---

### 2.2 Browser Console Displays CORS Errors
- **Symptoms**: Console displays:
  ```
  Access to fetch at 'https://localhost:7102/api/...' from origin 'https://localhost:7063'
  has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource.
  ```
- **Root Cause**: The MVC client is accessing the API directly via JavaScript (`fetch` or SignalR), and the origin of the MVC client is not present in the CORS policy.
- **Resolution**:
  - Inspect `PlatformManagementSystem.API/Program.cs`:
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
  - If your MVC client is running on a different port, add that origin to `WithOrigins()`.

---

## 3. Authentication & Authorization Issues

### 3.1 Cannot Access `/AdminDashboard` Using Default Seeded Account (`admin@test.com`)
- **Symptoms**: Logging in with `admin@test.com` and navigating to `/AdminDashboard` redirects to the login screen or produces an `AccessDenied` response.
- **Root Cause**: In `PlatformManagementSystem.Infrastructure/Identity/IdentitySeeder.cs`, the initial seeder creates the `Student` and `Instructor` roles and assigns `admin@test.com` to the `Instructor` role.
- **Resolution**:
  - The controller requires `[Authorize(Roles = "Admin")]`.
  - Add the user to the `Admin` role using SQL Server Management Studio:
    ```sql
    -- Ensure Admin role exists
    IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
    BEGIN
        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
        VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
    END

    -- Assign Admin role
    DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'admin@test.com');
    DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');

    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
    END
    ```
  - Log out and log back in to refresh the authentication cookie and session claims.

---

### 3.2 401 Unauthorized on API Endpoints
- **Symptoms**: API requests return `401 Unauthorized` despite being logged into the MVC frontend.
- **Root Cause**: The session variable `UserToken` is empty or has expired (default lifetime is 2 hours for session / 3 hours for JWT).
- **Resolution**:
  - Verify that the login POST action in `AccountController.cs` successfully stored `HttpContext.Session.SetString("UserToken", result.Token)`.
  - Log out via `/Account/Logout` and log in again to generate a fresh token.

---

## 4. SignalR & Live Whiteboard Diagnostics

### 4.1 WebSocket Connection to `/liveboardHub` Fails
- **Symptoms**: Whiteboard does not synchronize, and the browser console logs:
  ```
  WebSocket connection to 'wss://localhost:7102/liveboardHub?access_token=...' failed:
  Error during WebSocket handshake: Unexpected response code: 401
  ```
- **Root Cause**: SignalR requires passing the JWT bearer token as a query string parameter during the initial WebSocket handshake. If the token is missing or expired, the JWT middleware rejects the handshake.
- **Resolution**:
  - Verify that `window.userToken` is rendered in `Views/LiveSession/Board.cshtml`:
    ```html
    <script>
        window.sessionId = @ViewBag.SessionId;
        window.userToken = "@ViewBag.Token";
    </script>
    ```
  - Inspect the network tab in browser developer tools to ensure `?access_token=...` contains the JWT.

---

## 5. HTTPS & SSL Certificate Issues

### 5.1 `The remote certificate is invalid according to the validation procedure`
- **Symptoms**: Outbound HTTP requests from MVC to API fail with an SSL/TLS certificate validation exception.
- **Root Cause**: ASP.NET Core development HTTPS certificate is either missing or untrusted on the local operating system.
- **Resolution**:
  - Run the following commands in an elevated PowerShell terminal:
    ```powershell
    dotnet dev-certs https --clean
    dotnet dev-certs https --trust
    ```
  - Restart all running instances of the API and MVC applications.
