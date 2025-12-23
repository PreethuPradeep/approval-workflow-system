# Approval Workflow System (Backend)

A backend-focused approval workflow system built using **ASP.NET Core** and **Entity Framework Core**, designed to model real-world approval processes with strong emphasis on **correctness, auditability, and controlled state transitions**.

This project prioritizes backend integrity and workflow safety over UI complexity.
>  Backend-first build. Frontend and deployment will be added after core workflow stabilization.

---

##  Key Features

- **Explicit Request State Machine**
  - Guarded state transitions prevent invalid workflow jumps
  - States modeled explicitly instead of flag-based status handling

- **Approval Lifecycle**
  
      Draft → Submitted → Assigned → Under Review
                               ↓
                           Approved → Closed
                               ↓
                           Rejected → Closed
                               ↓
                           Escalated → PendingAdmin

- **Auditor Assignment Management**
  - Single active assignment enforced per request
  - Assignment lifecycle explicitly tracked
  - Assignments automatically closed on terminal actions

- **Redressal (Appeal) Handling**
  - Redressals can be created only for closed requests
  - Redressals can re-enter the workflow safely
  - Multiple redressals supported without losing historical context

- **Audit Logging**
  - Every state-changing action is recorded
  - Captures actor, action, previous state, next state, timestamp, and reason where applicable
  - Audit records are immutable

- **Soft Delete Strategy**
  - Requests are deactivated, not deleted
  - Preserves audit trails and historical data
  - Implemented using query filters

---

##  Design Principles

- **State machines over boolean flags**  
  Prevents invalid combinations and implicit transitions

- **Service-layer business logic**  
  Controllers remain thin; all workflow rules live in services

- **Database constraints as safety nets**  
  Uniqueness and relationship rules enforced at the database level

- **Auditability by default**  
  No silent state changes

- **Clarity over abstraction**  
  EF Core used directly without generic repositories

---

##  Architecture Overview

Controllers (API layer)
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

##  Core Domain Concepts

### Request
Represents the primary approval entity.  
Stores current state, timestamps, redressal count, and activity flag

### RequestAssignment
Tracks which auditor is currently responsible for a request.  
Only one active assignment allowed at a time.

### RequestAudit
Immutable log of every workflow action.

### Redressal
Represents an appeal cycle for a closed request.  
Redressals are versioned and tracked independently of the main request lifecycle.

---

##  Tech Stack

- **Backend:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Language:** C#
- **Version Control:** Git, GitHub
- **Authentication**: Cookie-based with claims
- **Authorization**: Role-based via ASP.NET Core role claims

---

##  Current Status

- Core backend workflow implemented
- Redressal creation and closure supported
- Full audit logging in place
- Controllers wired to service-layer rules

---

## Testing Strategy

This project uses targeted unit tests covering state guards, assignment ownership, and authorization-sensitive paths to enforce critical workflow invariants:

- Invalid state transitions are rejected
- Only assigned auditors can act on requests
- Assignments are closed on terminal actions
- Audit entries are created for every state change

The focus is on business-critical rules rather than exhaustive CRUD coverage.

---

## API Usage

### Submit Request
POST /api/requests/{id}/submit
Role: Requestor
Precondition: Draft

Response: 204 No Content

### Assign Request
POST /api/requests/{id}/assign
Role: Admin
Precondition: Submitted

### Open Request
POST /api/requests/{id}/open
Role: Auditor
Precondition: Assigned

### Approve Request
POST /api/requests/{id}/approve
Role: Auditor
Precondition: Opened

### Rejected Request
POST /api/requests/{id}/reject
Role: Auditor
Precondition: Opened

### Escalate Request
POST /api/requests/{id}/escalate
Role: Auditor
Precondition: UnderAuditorReview


### Close Request
POST /api/requests/{id}/close
Role: Admin
Precondition: Approved/Rejected

### Deactivate Request
POST /api/requests/{id}/deactivate
Role: Admin
Precondition: Closed


### Create Reddressal 
POST /api/requests/{id}/redressals
Role: Requestor
Precondition: Closed

### Close Redressal
POST /api/requests/{id}/redressals/close
Role: Admin
Precondition: Redressal created, Request closed

---

### In Progress / Planned
- Documentation diagrams (ER + state flow)
- Expanded negative-path tests
- Minimal frontend (Angular)
- Dockerization and cloud deployment

---

## State machine visualization

Detailed diagrams are maintained separately using Mermaid:

State machine: docs/state-machine.md

ER diagram: docs/er-diagram.md

These documents reflect the current implementation and are kept in sync with the codebase.

---

## Authorization Boundaries

| Action                  | Role       |
|-------------------------|------------|
| Submit                  | Requestor  |
| Assign                  | Admin      |
| Open                    | Auditor    |
| Approve                 | Auditor    |
| Reject                  | Auditor    |
| Escalate                | Auditor    |
| Close                   | Admin      |
| Deactivate              | Admin      |
| Redressal create        | Requestor  |
| Redressal close         | Admin      |


---

##  Why This Project Exists

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
