# Automated Testing Framework

This repository contains a comprehensive testing framework covering both UI automation (TODO web application) and API testing (Reqres.in) using SpecFlow, Selenium, RestSharp, and NUnit.

## Features

### UI Testing (TODO Web Application)
- **Login Functionality**
  - Successful login with valid credentials
  - Failed login with error validation
- **Task Management**
  - Create new tasks with details
  - Delete existing tasks
  - Form validation

### API Testing (Reqres.in)
- **CRUD Operations**
  - GET: Fetch list of users
  - POST: Create new users
  - PUT: Update existing users
  - DELETE: Remove users
- **Response Validation**
  - Status codes
  - Response schemas
  - Data integrity

## Technology Stack
- **Framework**: SpecFlow (BDD)
- **UI Automation**: Selenium WebDriver
- **API Testing**: RestSharp
- **Assertions**: NUnit
- **Reporting**: Extent Reporting**
- **Logging**: Serilog**
- **CI/CD Ready**: GitHub Actions compatible

## Prerequisites
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download)
- Chrome/Firefox browser (for UI tests)
- Visual Studio 2022 with SpecFlow extension (recommended)

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/kazimlilani/SpecFlowProject
   cd automation-framework
