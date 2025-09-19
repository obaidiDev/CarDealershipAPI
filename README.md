# CDMS (.NET 9 Car Dealership Management System)

This project is a demo **Car Dealership Management System** implemented using **.NET 9 Web API**. It demonstrates CRUD operations, role-based access, OTP authentication, and basic security practices.

---

## Table of Contents

* [Overview](#overview)
* [Features](#features)
* [Tech Stack](#tech-stack)
* [Getting Started](#getting-started)
* [Available Endpoints](#available-endpoints)
* [OTP Flow](#otp-flow)
* [Assumptions & Design Decisions](#assumptions--design-decisions)
* [Docker](#docker)

---

## Overview

This system allows:

* Admins to manage cars, view customers, and process purchases.
* Customers to browse vehicles, view details, request purchases, and see purchase history.
* OTP-based actions for security (register, login, purchase, update car).

Pre-populated with:

* 1 Admin user (`admin@dealer.com / Admin123!`)
* 10 sample cars

---

## Features

**User Management**

* Register (Customer/Admin)
* Login with OTP
* Role-based access (Admin / Customer)

**Admin Use Cases**

* Add, update, delete cars
* View all registered customers
* Process purchase transactions (OTP-protected)

**Customer Use Cases**

* Browse available cars
* View car details
* Request vehicle purchase (OTP-protected)
* View purchase history

**Security & Validation**

* JWT authentication
* OTP-based validation for critical actions
* Input validation and error handling
* Logging of requests and errors

---

## Tech Stack

* **.NET 9 ASP.NET Core Web API**
* **Entity Framework Core** (In-Memory DB for demo; can switch to SQL Server/Postgres)
* **JWT Authentication**
* **Middleware for error handling**
* **Swagger/OpenAPI**
* **Docker**

---

## Getting Started

1. Install [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Clone this repository
3. Restore dependencies:

```bash
dotnet restore
```

4. Run the API:

```bash
dotnet run
```

By default, the API runs at:

```
http://localhost:5000
```

Access Swagger UI at:

```
http://localhost:5000/swagger
```

---

## Available Endpoints

### AuthController (`api/auth`)

| Method | Endpoint           | Role          | Description                               |
| ------ | ------------------ | ------------- | ----------------------------------------- |
| POST   | `/register`        | Public        | Register a new user (OTP sent to console) |
| POST   | `/verify-register` | Public        | Verify registration OTP                   |
| POST   | `/login`           | Public        | Login and request login OTP               |
| POST   | `/verify-login`    | Public        | Verify login OTP and receive JWT          |
| POST   | `/request-otp`     | Authenticated | Request OTP for actions (update/purchase) |

---

### VehiclesController (`api/vehicles`)

| Method | Endpoint                    | Role   | Description                                     |
| ------ | --------------------------- | ------ | ----------------------------------------------- |
| GET    | `/`                         | Public | List all cars (optional query: `make`, `model`) |
| GET    | `/{id}`                     | Public | Get car details by ID                           |
| POST   | `/`                         | Admin  | Add new car (OTP required: X-OTP-Code header)   |
| POST   | `/{id}/generate-update-otp` | Admin  | Generate OTP for updating car                   |
| PUT    | `/{id}/confirm-update`      | Admin  | Confirm update with OTP (X-OTP-Code header)     |
| DELETE | `/{id}`                     | Admin  | Delete car                                      |

> **Add Car Logic**:
>
> * If a car with the same **Make, Model, Year, TrimLevel, MarketRegion** exists, only **Color, LicensePlate, IsAvailable** are updated.
> * Otherwise, all attributes must be provided.

---

### PurchasesController (`api/purchases`)

| Method | Endpoint            | Role     | Description                               |
| ------ | ------------------- | -------- | ----------------------------------------- |
| POST   | `/request/generate` | Customer | Generate OTP for purchase (check console) |
| POST   | `/request/confirm`  | Customer | Confirm purchase with OTP                 |
| GET    | `/history`          | Customer | View customer's purchase history          |
| GET    | `/all`              | Admin    | View all purchases                        |

---

## OTP Flow

1. **Generation**:
   OTP is generated for registration, login, purchase, and update actions, then printed to console.

2. **Validation**:
   OTP must be provided via `X-OTP-Code` header or request body for sensitive actions:

   * Login
   * Registration
   * Purchase Request
   * Add/Update Vehicle (Admin)

3. **Expiration**: OTP expires in 5 minutes.

4. **Delivery**: Console-based for demo only.

---

## Assumptions & Design Decisions

* **In-Memory Database** for demo. Production should use SQL Server/Postgres.
* **JWT** authentication for all secured endpoints.
* **Role-based access** via `[Authorize(Roles="Admin")]` or `[Authorize(Roles="Customer")]`.
* **Input validation** enforced using DTOs and server-side checks.
* **Logging** enabled for requests and errors.
* **OTP** stored in memory, tied to user and action.
* **Pre-population** of 1 admin and 10 sample cars for demonstration.
* **Skipped Details** I skipped some details because this is my first time learning c# and .NET these and I did not have time to implement them. The details inlude:
- Payment methods managment: adding, updating, deleting
- Avoid Sending the OTP for each car update PUT request and rather sending it one time for better usability
- Populating with accurate car details
- Skipping metdata about the user such as the birthday to make the tests easier

---

## Docker

Build Docker image:

```bash
docker build -t cdms-api .
```

Run container:

```bash
docker run -d -p 5000:5000 -p 5001:5001 --name cdms cdms-api
```

API accessible at:

```
http://localhost:5000
https://localhost:5001
```

Swagger at:

```
http://localhost:5000/swagger
```
"# CarDealershipAPI" 
