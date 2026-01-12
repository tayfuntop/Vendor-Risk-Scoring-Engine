# Vendor Risk Scoring Engine (Rule-Based Edition)

<p align="center">
  <img src="./assets/demo.gif" alt="Vendor Risk Scoring Demo" width="900" />
</p>

> **Rule-Based Vendor Risk Scoring Engine** - A modern .NET 8 Web API and Next.js Fullstack project developed with Clean Architecture.

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Technologies](#-technologies)
- [Quick Start](#-quick-start)
- [Ports and Access](#-ports-and-access)

---

## 🎯 About the Project

**Vendor Risk Scoring Engine** is a rule-based assessment system that produces **explainable and consistent risk scores** for vendors based on financial health, operational reliability, and security compliance criteria.

This project was developed as a **case study** and incorporates enterprise-level architectural principles suitable for real-world scenarios.

### Core Capabilities

- ✅ Rule-based vendor risk scoring
- ✅ Evaluation across financial, operational, and security dimensions
- ✅ Explainable risk scores (clear reasoning for each score)
- ✅ Relational analysis with risk similarity matrix
- ✅ Performance optimization with Redis cache
- ✅ Structured logging in JSON format for ELK Stack integration (Serilog)
- ✅ Full container support with Docker
- ✅ Automatic database migration and seed data

---

## 🛠 Technologies

### Backend (.NET 8)

| Technology                | Description                                          |
| ------------------------- | ---------------------------------------------------- |
| **.NET 8**                | Modern, high-performance web API framework           |
| **PostgreSQL**            | Relational database management system                |
| **Redis**                 | Distributed caching solution                         |
| **MediatR**               | Mediator library for CQRS pattern implementation     |
| **FluentValidation**      | Powerful and readable validation rules               |
| **Serilog**               | Structured logging (JSON Sink, ELK Stack compatible) |
| **Entity Framework Core** | ORM and database migration management                |
| **xUnit + Moq**           | Unit and integration testing                         |

### Frontend (Next.js 14)

| Technology            | Description                                |
| --------------------- | ------------------------------------------ |
| **Next.js 14**        | Modern React-based frontend framework      |
| **TypeScript**        | Type-safe JavaScript development           |
| **React Hook Form**   | Performant form management                 |
| **Material-UI (MUI)** | Modern and responsive UI component library |
| **Axios**             | HTTP client for API requests               |

### DevOps

- **Docker & Docker Compose** - Containerization and orchestration
- **Multi-stage Dockerfile** - Optimized image size
- **Health Checks** - Container health monitoring

---

## 🚀 Quick Start

### Prerequisites

The following software must be installed on your system:

- [Docker](https://www.docker.com/get-started) (v20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (v2.0+)

> **Note**: For Windows users, [Docker Desktop](https://www.docker.com/products/docker-desktop/) is sufficient.

### Installation and Running

Follow these steps to run the project:

#### 1. Clone the Project

```bash
git clone <repository-url>
cd Vendor-Risk-Scoring-Engine
```

#### 2. Start the Application

The following command builds and starts all services (PostgreSQL, Redis, Backend API, Frontend):

```bash
docker-compose up --build -d
```

#### 3. xUnit Tests

```bash
cd Backend
dotnet test
```

---

## 🌐 Ports and Access

| Service         | Port | URL                           | Description                          |
| --------------- | ---- | ----------------------------- | ------------------------------------ |
| **Frontend**    | 3000 | http://localhost:3000         | Next.js web interface                |
| **Backend API** | 5000 | http://localhost:5000/api     | .NET 8 Web API                       |
| **Swagger UI**  | 5000 | http://localhost:5000/swagger | API documentation and test interface |
| **PostgreSQL**  | 5435 | localhost:5435                | Database (for external access)       |
| **Redis**       | 6379 | localhost:6379                | Cache service                        |

### First Access

After starting the application:

1. **Frontend**: Navigate to http://localhost:3000
2. **15 sample vendors** will be automatically loaded
3. You can view risk scores and details in the vendor list
4. Test the API via Swagger UI: http://localhost:5000/swagger
