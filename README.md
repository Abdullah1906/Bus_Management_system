# 🚌 Bus Position Solution (BPS) & Fleet Management System

![CI/CD Pipeline](https://github.com/Abdullah1906/Bus_Management_system/actions/workflows/deploy.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-17%2B-DD0031?style=flat&logo=angular)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC292B?style=flat&logo=microsoftsqlserver)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)
![License](https://img.shields.io/badge/License-MIT-green.svg)

**Bus Position Solution (BPS)** is an enterprise-grade, full-stack fleet management system engineered to streamline daily bus operations, automate trip schedules, manage fare/tip distribution, and deliver actionable operational reporting.

---

## 🌟 Key Modules & Features

* 📍 **Place & Route Management:** Define, map, and manage transit routes, checkpoints, and terminal stops with precision.
* 📋 **Trip & Fleet Records:** Record real-time bus positions, operational status, trip schedules, and vehicle logs.
* 💰 **Tip & Fare Engine:** Automated logic for daily fare calculations, trip tip allocations, driver-conductor payouts, and revenue metrics.
* 📊 **Dynamic Data Grid & Reporting:** Highly customisable UI tables featuring advanced filtering, column toggling, and exportable PDF summaries.

---

## 🛠️ Technology Stack

| Domain | Technologies Used |
| :--- | :--- |
| **Backend** | .NET Core Web API (C#), ADO.NET |
| **Frontend** | Angular, JavaScript, Bootstrap, jQuery, DataTables |
| **Database** | Microsoft SQL Server (Stored Procedures, Dynamic SQL, Table-Valued Functions) |
| **Reporting** | PDF Exporting (Handlebars templates, jsPDF, html2pdf.js) |
| **DevOps & CI/CD** | GitHub Actions (`.github/workflows`), Automated Build & Deployment |
| **Architecture** | Clean Architecture (DDD principles), Repository Pattern |

---

## 🏗️ System Architecture

The solution adheres strictly to **Clean Architecture** to ensure high maintainability, testability, and loose coupling across components:
├── 1. Core Domain Layer     --> Entities, Value Objects & Domain Exceptions
├── 2. Application Layer     --> Use Cases, DTOs, CQRS/Service Interfaces & Validation
├── 3. Infrastructure Layer  --> ADO.NET Repositories, SQL Queries & External Services
└── 4. Presentation Layer    --> RESTful APIs (.NET Web API) & Angular Web Client

* **Domain Layer:** Pure business entities and domain logic, isolated from frameworks.
* **Application Layer:** Encapsulates business workflows, service abstractions, DTO mappings, and request handlers.
* **Infrastructure Layer:** Implements data access using ADO.NET, optimized SQL Server execution scripts, and database connections.
* **Presentation Layer:** Exposes REST API endpoints and delivers a dynamic, responsive UI via Angular.

---

## 🔄 CI/CD Automation

This repository includes a fully configured **CI/CD Pipeline** using **GitHub Actions** (`.github/workflows/`):

* **Automated Builds:** Triggered on every `push` and `pull_request` to verify backend and frontend compilation.
* **Continuous Integration:** Automatically runs unit tests, code validation, and linter checks.
* **Continuous Deployment:** Builds production assets for .NET API and Angular web clients automatically.

---

## ⚡ Getting Started

### Prerequisites

Ensure you have the following installed locally:
* **.NET 8.0 SDK** or higher
* **Node.js** (v18+) & **Angular CLI**
* **Microsoft SQL Server** (2019 or later)

---

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/Abdullah1906/Bus_Management_system.git](https://github.com/Abdullah1906/Bus_Management_system.git)
   cd Bus_Management_system



   

# need one public ec2 for bastion host(backend_sg) and one private ec2(database_sg)
# security group
1. bastion_sg
ssh 22 custom 0.0.0.0/0
custom tcp 5000 custom 0.0.0.0/0
http 80 custom 0.0.0.0/0
https 443 custom 0.0.0.0/0
2.backend_sg
ssh 22 custom bastion_sg
mssql 1433 custom bastion_sg private ip

# ci/ cd pipeline 


after ci/cd file run then search
# http://13.212.230.200/swagger
