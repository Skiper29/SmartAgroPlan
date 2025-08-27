# 🌱 SmartAgroPlan

[![Build & Test](https://github.com/Skiper29/SmartAgroPlan/actions/workflows/build.yml/badge.svg)](https://github.com/Skiper29/SmartAgroPlan/actions)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Skiper29_SmartAgroPlan&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Skiper29_SmartAgroPlan)
[![License](https://img.shields.io/github/license/Skiper29/SmartAgroPlan?color=blue)](LICENSE)

---

## 📖 Overview

**SmartAgroPlan** is an intelligent farm management system designed to help farmers and agricultural organizations **optimize resources, predict crop yields, and manage risks**.  
It combines **machine learning, distributed information infrastructure, and data analytics** to provide actionable insights for agriculture.

Key features include:
- 📊 **Resource forecasting** (water, fertilizers, etc.)
- 🌦️ **Weather-aware predictions** using external APIs
- 🧮 **Statistical and AI-based forecasting models**
- 🗓️ **Smart scheduling** for irrigation and fertilization
- 🏢 **Multi-organization support** (farms, agri-firms)
- 🔐 **Secure authentication** with ASP.NET Identity + JWT
- 📡 **Cloud-ready** with CDN for distributed data access

---

## ⚙️ Tech Stack

- **Backend**: [ASP.NET Core 9](https://learn.microsoft.com/en-us/aspnet/core/) (C#)  
- **Database**: [PostgreSQL](https://www.postgresql.org/) with [EF Core](https://learn.microsoft.com/en-us/ef/core/)  
- **Frontend**: [React](https://reactjs.org/) (planned)  
- **Infrastructure**: Docker, GitHub Actions CI/CD  
- **Testing**: xUnit (unit tests), Moq (mocking)  
- **Code Quality**: [SonarCloud](https://sonarcloud.io/) integration  

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)  
- [PostgreSQL](https://www.postgresql.org/)  
- [Node.js](https://nodejs.org/) (for frontend)  
- Docker (optional, for deployment)

### Clone the Repository
```bash
git clone https://github.com/Skiper29/SmartAgroPlan.git
cd SmartAgroPlan
````

### Configure the Database

Edit **`appsettings.json`**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=SmartAgroPlan;Username=postgres;Password=yourpassword"
}
```

### Run Migrations

```bash
dotnet ef database update --project SmartAgroPlan.Persistence
```

### Start the API

```bash
dotnet run --project SmartAgroPlan.API
```

API will be available at: **`https://localhost:5001/api`**

---

## 🧪 Running Tests

```bash
dotnet test
```

Test results will also be published to **SonarCloud** during CI/CD.

---

## 🔍 SonarCloud Integration

This project is continuously analyzed by [SonarCloud](https://sonarcloud.io) to ensure:

* ✅ Code Quality
* 🔐 Security Checks
* 🐞 Bug Detection
* 📊 Test Coverage

You can explore the live dashboard here:
👉 [SmartAgroPlan on SonarCloud](https://sonarcloud.io/dashboard?id=Skiper29_SmartAgroPlan)

---

## 📦 Project Structure

```
SmartAgroPlan/
│── SmartAgroPlan.WebAPI/       # REST API (controllers)
│── SmartAgroPlan.BLL/          # Business logic layer (services, rules, DTOs)
│── SmartAgroPlan.DAL/          # Data access layer, EF Core migrations, DbContext (repositories, entities)
│── SmartAgroPlan.XUnitTests/   # Unit tests
│── docs/                       # Documentation
│── .github/workflows/          # CI/CD workflows
```

---

## 🔐 Security

* Authentication: **JWT + ASP.NET Identity**
* Role-based access control (Organizations → Users → Fields)
* API secured with HTTPS

---

## 📈 Roadmap

* [ ] Implement resource prediction with ARIMA & genetic algorithms
* [ ] Add real-time decision support with fuzzy logic
* [ ] Integrate external weather APIs
* [ ] Build React frontend dashboard
* [ ] Add reporting & data visualization

---

## 🙌 Acknowledgements

* [Lviv Polytechnic National University](https://lpnu.ua/)
* Open-source community for ASP.NET Core, EF Core, PostgreSQL
* [SonarCloud](https://sonarcloud.io/) for code quality analysis
