# 🏢 Shared Office Booking – Backend

This is the **backend** of the **Shared Office Booking** platform, built with **.NET 8**, **Entity Framework Core**, and **Docker** for database orchestration.

The **frontend** is located in a separate repository:  
👉 [Frontend Repository](https://github.com/alijuu/SharedOfficeBookingClient)

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/alijuu/SharedOfficeBooking
cd SharedOfficeBooking
```

### 2. Start the Database

Make sure Docker is running, then start the database container:

```bash
docker-compose up -d
```

### 3. Apply Database Migrations

Apply the latest Entity Framework Core migrations:

```bash
dotnet ef database update --project .\SharedOfficeBooking.Infrastructure\ --startup-project .\SharedOfficeBooking.Api\
```

### 4. Run the Backend API

Run the backend server:

```bash
dotnet run --project .\SharedOfficeBooking.Api\
```
