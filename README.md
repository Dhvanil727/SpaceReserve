SpaceReserve – Smart Workspace Seat Booking System

SpaceReserve is a seat reservation and workspace management system designed to efficiently manage office seating for:

🏢 On-site employees

🏠 WFH employees

🔄 Hybrid employees

It provides:

- Secure authentication

- Role-based access

- Real-time seat availability

- Intuitive seat status & color coding

 Problem Statement

In modern hybrid work environments, managing office seats manually often leads to:

❌ Double bookings

❌ Poor space utilization

❌ Lack of transparency

✅ SpaceReserve solves this problem by offering a centralized, automated, and secure seat reservation platform.

✨ Features
🔐 Authentication & Authorization

🔑 Integrated Keycloak for authentication and authorization

🖥 Secure login & registration via Keycloak UI

🛡 Role-Based Access Control (RBAC):

- Admin

- User

🪑 Seat Management

🔍 View all seats based on:

- Date

- City

- Floor

⏱ Real-time seat availability


🎨 Seat Status & Color Coding Color Meaning
🟦 Blue	Booked by regular users
🟩 Green	Booked by WFH users
🟥 Red	Reserved by Admin (HR / Management)
🟨 Yellow	Under maintenance
⚪ Gray	Available
📆 Booking System

🗓 Book seats for a selected date

🚫 Prevents double booking

🧑‍💼 Handles different booking types:

- Hybrid users

- Regular users

- Admin reservations

🔄 Automatically updates seat status

🛠 Admin Capabilities

 - Reserve seats for specific users

 - Configure seat availability

 - Mark seats under maintenance

 - Manage seat configurations

⚙ Backend Architecture

 - 3-Tier Architecture:

 - Presentation Layer (API)

 - Business Layer (Services)

 - Infrastructure Layer (Repositories)

 - Clean separation of concerns

 - Common Utilities

 - Global exception handling

 - Common API response structure

 - Logging using Log4Net

 - Request validation using FluentValidation

 Tech Stack
🔹 Backend

 - ASP.NET Core Web API (.NET 8)

 - Entity Framework Core

 - SQL Server

 - Keycloak (OIDC)

🔹 Tools & Libraries

 - FluentValidation

 - Log4Net

 - JWT / OIDC

 - Swagger (API Documentation)

👨‍💻 Author

Dhvanil Patel
.NET Backend Developer
