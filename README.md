SpaceReserve – Smart Workspace Seat Booking System

SpaceReserve is a seat reservation and workspace management system designed to efficiently manage office seating for hybrid, WFH, and on-site employees.
It provides secure authentication, role-based access, and real-time seat availability with intuitive status and color coding.

📌 Problem Statement

In modern hybrid work environments, managing office seats manually leads to:

Double bookings

Poor space utilization

Lack of transparency

SpaceReserve solves this by offering a centralized, automated, and secure seat reservation platform.

✨ Features
🔐 Authentication & Authorization

Integrated Keycloak for authentication and authorization

Secure login & registration via Keycloak UI

Role-based access control (RBAC)

Admin

User

🪑 Seat Management

View all seats based on:

📅 Date

🌆 City

🏢 Floor

Real-time seat availability

Single API to fetch all seats with computed status

🎨 Seat Status & Color Coding
Status	Meaning
🟦 Blue	Booked by regular users
🟩 Green	Booked by WFH users
🟥 Red	Reserved by Admin (HR, Management)
🟨 Yellow	Under maintenance
⚪ Gray	Available
📆 Booking System

Book seats for a selected date

Prevents double booking

Handles:

Hybrid users

Regular users

Admin reservations

Automatically updates seat status

🛠 Admin Capabilities

Reserve seats for specific users

Configure seat availability

Mark seats under maintenance

Manage seat configurations

⚙ Backend Architecture

3-Tier Architecture

Presentation Layer (API)

Business Layer (Services)

Infrastructure Layer (Repositories)

Clean separation of concerns

Projection-based queries for optimized performance

📄 Common Utilities

Global exception handling

Common API response structure

Logging using Log4Net

FluentValidation for request validation

🧱 Tech Stack
Backend

ASP.NET Core Web API (.NET 8)

Entity Framework Core

SQL Server

Keycloak (OIDC)

Frontend

Any frontend supporting Keycloak authentication (login & register via Keycloak pages)

Tools & Libraries

FluentValidation

Log4Net

JWT / OIDC

Swagger (API documentation)

🗂 Database Entities

Users

Seats

SeatConfigurations

Bookings

Maintenance

🔄 Seat Status Logic (High Level)
If seat is under maintenance → Unavailable
Else if admin configuration exists → Reserved
Else if booking exists:
    - Hybrid user → Green
    - Regular user → Blue
Else → Available

🧪 API Highlights

Fetch all seats with computed status in one API

Filtered by date, city, and floor

Optimized using projection-based queries

📈 Future Enhancements

Dashboard analytics

Calendar-based booking view

Notification system (email / in-app)

Mobile-friendly UI

Reporting for admins

👨‍💻 Author

Dhvanil Patel
Bachelor’s in Information Technology
.NET Backend Developer | MERN Stack Enthusiast
