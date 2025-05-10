# 🏢 Shared Office Booking – Backend

This is the **backend** of the **Shared Office Booking** platform, built with **.NET 8**, **Entity Framework Core**, and **Docker** for database orchestration.

The **frontend** is located in a separate repository:  
👉 [Frontend Repository]([https://github.com/your-org/frontend-repo](https://github.com/alijuu/SharedOfficeBookingClient))

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/backend-repo.git
cd backend-repo
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

## 💻 Frontend Setup

The frontend is hosted in a separate repository.

### Steps:

1. Clone the frontend repo:

```bash
git clone https://github.com/your-org/frontend-repo.git
cd frontend-repo
```

2. Install dependencies:

```bash
npm install
```

3. Start the development server:

```bash
npm run dev
```

By default, the frontend should be available at `http://localhost:5173`

---
