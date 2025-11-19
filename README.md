# 📝 Claims Management System (Part 3)

Welcome to my **Claims Management System** project for PROG6212! This is Part 3, where the system levels up — we added a **super-powered HR role**, integrated **Microsoft Identity for login**, and switched to a **SQL Server database**.  

This README will guide you through everything I’ve implemented and how it works.

---

## 🎬 YouTube Demo
https://www.youtube.com/watch?v=8pcYoQU4wjo

---

## 📌 Overview

This project is a **C# ASP.NET Core MVC application** that manages lecturer claims with role-based access. For Part 3, I focused on **HR management**, **automated claim calculation**, and **secure authentication using Microsoft Identity**.

**Tech Stack & Key Features:**
- ASP.NET Core MVC
- **Microsoft Identity** for authentication and role management
- **SQL Server** database for storing users, claims, and documents
- Entity Framework (EF) for data access
- LINQ for report generation
- PDF generation for invoices and reports
- File uploads with validation (.pdf, .docx, .xlsx)
- Session management for secure navigation
- Meaningful error handling

---

## 👥 User Roles

### 1️⃣ HR (Super User)

HR is the **gatekeeper of the system** and manages users and reports.

**Features:**
- Add new users (Name, Surname, Email, Hourly Rate, Role)
- Update existing user information
- Generate reports/invoices using LINQ
- Export reports as PDF
- No public registration: HR creates user accounts and provides login credentials

### 2️⃣ Lecturer

Lecturers submit claims for worked hours. Workflow is **streamlined** so manual entry is minimal.

**Features:**
- Login required (handled via Microsoft Identity)
- Personal info auto-filled (Name, Surname, Hourly Rate)
- Enter hours worked and upload supporting documentation
- Auto-calculation of claim total
- Validation: Cannot exceed max hours (e.g., 180/month)
- Track claim status through the approval process

### 3️⃣ Programme Coordinator & Academic Manager (Admins)

Admins review and approve/reject claims. Separate dashboards ensure proper access control.

**Features:**
- View pending claims with supporting documents
- Coordinator: Verify/Reject claims
- Academic Manager: Approve/Reject claims
- Session-based page restrictions

---

## 🛠 Application Flow

1. **HR creates users** : lecturers get login credentials
2. **Lecturer logs in** :submits claim with auto-calculated total, uploads documents
3. **Coordinator reviews**  : verifies or rejects claims
4. **Academic Manager reviews** : approves or rejects claims
5. **Lecturer tracks claim status** : sees real-time updates

---

## 📁 File Handling

- File uploads linked to claims
- Stored securely in SQL Server or file system
- Restricted to `.pdf`, `.docx`, `.xlsx`, 5MB
- Size validation and error handling
- Uploaded file names displayed on the form

---

## 📊 Reports & Invoices

- HR generates reports using LINQ queries
- Export as PDF with claim totals, status, and lecturer info

---

## ✅ Validation & Error Handling

- Hourly limits enforced (max 180 hours/month)
- File type and size restrictions
- Meaningful error messages

---

## 🖥 UI / Design

- Clean, intuitive layout
- Color-coded claim status indicators
- Forms designed for simplicity and usability
- HR dashboard, Lecturer dashboard, Admin dashboards

---

## 🔐 Authentication & Security

- Microsoft Identity handles login and role management
- HR, Lecturer, and Admin roles properly separated
- Sessions prevent unauthorized access
- Passwords stored securely in SQL Server via Identity and are hashed 

---

