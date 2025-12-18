# Approval Workflow System (Backend)

A backend-focused approval workflow system built using **ASP.NET Core** and **Entity Framework Core**, designed to model real-world approval processes with strong emphasis on **correctness, auditability, and controlled state transitions**.

This project prioritizes backend integrity and workflow safety over UI complexity.
> 🚧 Backend-first build. Frontend and deployment will be added after core workflow stabilization.

---

## ✨ Key Features

- **Explicit Request State Machine**
  - Guarded state transitions prevent invalid workflow jumps
  - States modeled explicitly instead of flag-based status handling

- **Approval Lifecycle**
  - Draft → Submitted → Assigned → Under Review  
  - Approved / Rejected / Escalated → Closed → Deactivated

- **Auditor Assignment Management**
  - Single active assignment enforced per request
  - Assignment lifecycle tracked explicitly

- **Redressal (Appeal) Handling**
  - Create redressals for closed requests
  - Re-enter workflow safely
  - Close redressals without losing historical context

- **Audit Logging**
  - Every state-changing action is recorded
  - Captures actor, action, previous state, next state, timestamp, and reason where applicable

- **Soft Delete Strategy**
  - Requests are deactivated, not deleted
  - Preserves audit trails and historical data
  - Implemented using query filters

---

## 🧠 Design Principles

- **State machines over boolean flags**  
  Prevents invalid combinations and implicit transitions

- **Service-layer business logic**  
  Controllers remain thin; all workflow rules live in services

- **Database constraints as safety nets**  
  Uniqueness and relationship rules enforced at the database level

- **Auditability by default**  
  No silent state changes

- **Clarity over abstraction**  
  EF Core used directly without generic repositories (for now)

---

## 🏗️ Architecture Overview

Controllers (planned)
↓
Application Services
↓
EF Core DbContext
↓
SQL Server


- `RequestService` owns the full request lifecycle
- `RequestStateGuard` enforces allowed transitions
- Child entities (Assignments, Audits, Redressals) survive soft deletion

---

## 🧩 Core Domain Concepts

### Request
Represents the primary approval entity.  
Holds current state, timestamps, and redressal count.

### RequestAssignment
Tracks which auditor is currently responsible for a request.  
Only one active assignment allowed at a time.

### RequestAudit
Immutable log of all state-changing actions.

### Redressal
Represents an appeal cycle for a closed request.  
Redressals are versioned and tracked independently of the main request lifecycle.

---

## 🛠 Tech Stack

- **Backend:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Language:** C#
- **Version Control:** Git, GitHub

---

## 🚧 Current Status

- Core backend workflow implemented
- Redressal creation and closure supported
- Full audit logging in place

### In Progress / Planned
- Role-based authorization (Admin / Auditor / Requestor)
- REST API controllers
- Unit tests for state guards and services
- Documentation diagrams (ER + state flow)
- Minimal frontend (Angular)
- Dockerization and cloud deployment

---

## 📝 Why This Project Exists

This project was built to explore **realistic backend workflow design**, focusing on:
- correctness under complex state transitions
- auditability for compliance-style systems
- maintainable service-layer architecture

It intentionally avoids being a CRUD demo.

---

## 📌 Notes

- This is an actively evolving project
- Backend-first by design
- UI and infrastructure will follow once core rules are stable

---

## 👤 Author

**Preethu Pradeep**  
Backend-focused .NET Developer  

- GitHub: https://github.com/PreethuPradeep
- LinkedIn: https://linkedin.com/in/preethu-pradeep-0443a836a
