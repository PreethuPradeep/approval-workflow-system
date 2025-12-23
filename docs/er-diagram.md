# Entity Relationship Overview

This document describes the core domain entities and their relationships.

---

## User
- Id
- FullName
- Email
- PasswordHash

Relationships:
- One User → Many UserRoles
- One User → Many RequestAudits (Actor)

---

## Role
- Id
- Name

Relationships:
- One Role → Many UserRoles

---

## UserRole
- UserId
- RoleId

Purpose:
- Maps users to roles (Admin, Auditor, Requestor)

---

## Request
- Id
- RequesterId
- CurrentState
- IsActive
- CreatedAt
- SubmittedAt
- ClosedAt
- RedressalCount

Relationships:
- One Request → Many RequestAssignments
- One Request → Many RequestAudits
- One Request → Many Redressals

---

## RequestAssignment
- Id
- RequestId
- AuditorId
- AssignedAt
- IsActive

Rules:
- Only one active assignment per request

---

## RequestAudit
- Id
- RequestId
- ActorId
- ActorRole
- FromState
- ToState
- Action
- Reason
- CreatedAt

Purpose:
- Immutable audit trail of all state changes

---

## Redressal
- Id
- RequestId
- RedressalCount
- IsActive
- CreatedAt
- ClosedAt

Relationships:
- One Redressal → Many RedressalContents

---

## RedressalContent
- Id
- RedressalId
- PayLoad
- CreatedAt

Purpose:
- Stores appeal content separately from lifecycle metadata
