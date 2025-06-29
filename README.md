# Admin and User Dashboard System - ASP.NET Core MVC

![.NET Core](https://img.shields.io/badge/.NET-7.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-7.0-green)
![EF Core](https://img.shields.io/badge/EF_Core-7.0-purple)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-red)

A comprehensive admin and user dashboard system built with ASP.NET Core MVC (.NET 7), Entity Framework Core, and SQL Server, featuring role-based access control, MCQ test management, and performance analytics.

## Features

### Admin Dashboard
- **User Management**
  - List non-admin users with login counts
  - Activate/deactivate user accounts
- **Job/Theme Management**
  - Aggregate job categories with statistics
  - Manage job titles and associated metrics
- **MCQ Test Management**
  - List all MCQ tests with detailed information
  - Track test results and pass rates
- **Performance Analytics**
  - Identify top 10 users by scores
  - Calculate overall performance metrics
- **Difficulty Configuration**
  - Map difficulty levels with allocation percentages
- **Dashboard Aggregation**
  - Compile totals for admin overview (users, job titles, tests, questions)

### User Dashboard
- **Personalized Statistics**
  - Display MCQ test results, points, and progress percentage
- **Achievement Tracking**
  - Award badges based on test performance
  - Provide feedback based on progress
- **Test History**
  - List all achieved tests with details
- **Account Management**
  - User account deactivation

### Core Modules
- **Job Categories (Themes)**
  - CRUD operations for job categories
  - Role-based access control
- **Job Levels**
  - Management of job hierarchies (Junior, Senior, Lead)
  - Standardization of roles
- **Job Titles**
  - Definition and categorization of job roles
  - Link between roles and assessments
- **MCQ Tests**
  - Full lifecycle management of assessments
  - Test delivery and scoring
- **Question Management**
  - Creation and categorization of questions
  - Bulk import system
- **Difficulty Management**
  - Configuration of difficulty levels and point values

## Technologies Used

### Core Framework
- ASP.NET Core MVC (.NET 7)
- Dependency Injection
- Async/Await pattern

### Data Access
- Entity Framework Core
- Repository Pattern
- SQL Server

### Security
- ASP.NET Core Identity
- Role-Based Authorization
- Anti-Forgery Tokens

### Frontend
- Razor Views
- ViewModels
- AJAX for dynamic filtering

### Business Logic
- Custom algorithms for:
  - Progress calculation
  - Badge eligibility
  - Test scoring
- LINQ for data processing

## Getting Started

### Prerequisites
- .NET 7 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/admin-user-dashboard.git
