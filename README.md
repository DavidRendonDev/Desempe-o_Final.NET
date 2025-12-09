# TalentoPlus (Desempeño Final) — Documentation

This repository contains a **.NET 9** solution built with a **DDD / Clean Architecture** style and delivered as:
- **TalentoP.Api**: REST API (JWT authentication, departments, employee profile endpoints, email + PDF services).
- **TalentoP.Web**: MVC Web app (Admin login via ASP.NET Identity, Excel import, dashboards, and AI-powered questions using Gemini).

---

## 1) Tech Stack

- **.NET 9 (C#)** — solution targets `net9.0`
- **ASP.NET Core Web API** — `TalentoP.Api`
- **ASP.NET Core MVC** — `TalentoP.Web`
- **Entity Framework Core + MySQL** — persistence layer
- **JWT Authentication** — for API auth
- **ASP.NET Core Identity** — for Web admin authentication
- **QuestPDF** — PDF generation (API side)
- **Gemini API** — AI intent parsing for dashboard questions (Web side)
- **Docker (optional)** — for running MySQL locally

---

## 2) Solution Structure (DDD / Clean)

At the root you’ll find:

- `1-Application/`  
  Use cases, DTOs, application services (`*AppService`), interfaces (ports).
- `2-Domain/`  
  Entities + domain rules + repository interfaces.
- `3-Infrastructure/`  
  EF Core DbContexts, migrations, repository implementations, external services (JWT, Email, PDF).
- `TalentoP.Api/`  
  API controllers + dependency injection + Swagger.
- `TalentoP.Web/`  
  MVC controllers + views + Identity login + Excel import + Gemini integration.

---

## 3) Prerequisites

Install the following:

1. **.NET SDK 9**
2. **MySQL 8** (either installed locally OR via Docker)
3. **EF Core CLI tools** (recommended):
   ```bash
   dotnet tool install --global dotnet-ef

If you already have it:

dotnet tool update --global dotnet-ef

4) Important Security Note (Before Running)

This repository includes example configuration values in appsettings.json files (DB connection string, Gemini key, SMTP settings, JWT key).
For a professional setup, you should NOT commit real secrets.

✅ Recommended approach:

    Use User Secrets (local development)

    Or Environment Variables (Docker/CI/CD)

You should update:

    Jwt:Key (must be long; 32+ chars recommended)

    Smtp:Username / Smtp:Password

    Gemini:ApiKey

    Database connection string credentials

5) Configuration
5.1 Database connection string (API + Web)

Both projects read:

ConnectionStrings:DefaultConnection

Example format used by the solution:

"server=localhost;port=3306;database=talentoplus_db;user=root;password=123456;"

If you use Docker MySQL, ensure the credentials match your container settings.
5.2 JWT (API)

TalentoP.Api/appsettings.json contains:

"Jwt": {
  "Key": "PUT_A_LONG_SECRET_KEY_HERE_32CHARS_MIN",
  "Issuer": "TalentoP.Api",
  "Audience": "TalentoP.Clients",
  "ExpiresMinutes": 120
}

You must set a real JWT Key value.
5.3 Gemini (Web)

TalentoP.Web/appsettings.json contains:

"Gemini": {
  "ApiKey": "YOUR_API_KEY",
  "Model": "gemini-2.5-flash"
}

You must set a real Gemini API key to use the AI dashboard questions feature.
6) Database Setup

You have two options:
Option A — Run MySQL with Docker (recommended)

From the repository root (where docker-compose.yml is):

docker compose up -d

Then confirm MySQL is running on:

    localhost:3306

    Note: If your docker-compose.yml credentials differ from your appsettings connection string, update either Docker or DefaultConnection so they match.

Option B — Run MySQL installed locally

Create a MySQL database (example):

CREATE DATABASE talentoplus_db;

Ensure your MySQL user/password matches the connection string.
7) Apply EF Core Migrations

Migrations are stored under 3-Infrastructure/Migrations.

Run migrations using the API as startup project (recommended because it configures the context cleanly).

From the solution root (where PSolution.sln is), run:

dotnet ef database update --project 3-Infrastructure --startup-project TalentoP.Api

If you get an error about finding the DbContext, ensure:

    You’re running it from the solution folder

    The projects restore correctly (dotnet restore)

8) Run the Projects (Local Development)
8.1 Run the API

dotnet run --project TalentoP.Api

Default local URLs (from launch settings):

    HTTP: http://localhost:5021

    HTTPS: https://localhost:7139

Swagger is enabled:

    http://localhost:5021/swagger

    https://localhost:7139/swagger

8.2 Run the Web (MVC)

dotnet run --project TalentoP.Web

Default local URLs:

    HTTP: http://localhost:5153

    HTTPS: https://localhost:7141

9) Web Admin Login (Seeded User)

When the MVC app starts, it automatically seeds:

    Departments

    A default admin user

Default admin credentials:

    Email: admin@talentoplus.com

    Password: admin123

⚠️ For real deployments, change these immediately.
10) Features Overview
10.1 API Features (TalentoP.Api)

Controllers included:

    POST /api/auth/register
    Register a new employee user.

    POST /api/auth/login
    Login and receive JWT token.

    GET /api/me/... (authorized)
    Endpoints for the logged-in employee (requires JWT).

    ... /api/departments ...
    Department-related endpoints.

    Use Swagger to see the full list of endpoints and schemas.

JWT Usage

    Call POST /api/auth/login

    Copy the returned token

    In Swagger: click Authorize and paste:

    Bearer YOUR_TOKEN

10.2 Web Features (TalentoP.Web)

Main features:

    Admin authentication (Identity)

    Employee management views (MVC)

    Import employees from Excel (.xlsx)

    AI Questions on dashboard using Gemini (intent → query → answer)

11) Excel Import Format (Employees)

The Excel import expects an .xlsx file and reads columns by exact header names.

Expected headers:

    DocumentNumber

    FirstName

    LastName

    Email

    Phone

    JobTitle

    Salary

    HireDate

    EmploymentStatus (example values: Active, Inactive, Vacation)

    EducationLevel

    ProfessionalProfile

    Department

If headers are misspelled or missing, the import may fail or skip fields.
12) AI Dashboard Questions (Gemini)

The Web project includes an AI query service that:

    Sends your question to Gemini

    Gemini returns JSON intent

    The app executes the intent against the database

Currently supported action:

    countEmployees

Supported filters:

    department

    employmentStatus (Active, Inactive, Vacation)

    jobTitleContains

Example questions:

    “How many employees are in Technology?”

    “How many inactive employees?”

    “How many employees have ‘Developer’ in job title?”

If Gemini returns something unexpected, the system attempts to extract JSON from the response.
13) Common Troubleshooting
“Cannot connect to MySQL”

    Verify MySQL is running on localhost:3306

    Confirm database exists: talentoplus_db

    Confirm credentials match DefaultConnection

    If using Docker, run:

    docker ps

“JWT Key is missing or too short”

    Set Jwt:Key in TalentoP.Api/appsettings.json (or user-secrets/env var)

    Use a long value (32+ chars)

“Gemini not working”

    Ensure Gemini:ApiKey is valid

    Ensure internet access is available

    Check logs for Gemini response formatting issues

EF migration command errors

Try restoring first:

dotnet restore

Then:

dotnet ef database update --project 3-Infrastructure --startup-project TalentoP.Api

14) Recommended Next Improvements (Professional Checklist)

    Move secrets to User Secrets / Environment Variables

    Add .env + docker-compose variables for local dev

    Add validation for Excel import (required columns + row-level error report)

    Add more AI intents (group-by department, salary ranges, etc.)

    Add role-based authorization for the API

    Add logging & error-handling middleware

15) Quick Start (Copy/Paste)

# 1) Restore
dotnet restore

# 2) Start MySQL (Docker)
docker compose up -d

# 3) Apply migrations
dotnet ef database update --project 3-Infrastructure --startup-project TalentoP.Api

# 4) Run API
dotnet run --project TalentoP.Api

# 5) Run Web MVC
dotnet run --project TalentoP.Web

Open:

    API Swagger: http://localhost:5021/swagger

    Web: http://localhost:5153


If you want, I can also generate a **second markdown file** like `API_ENDPOINTS.md` with a clean list of endpoints + request/response examples based on the controllers/DTOs in your project.

