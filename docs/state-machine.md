# Request State Machine

This document describes the allowed state transitions for a Request.

The system enforces transitions using a dedicated guard (`RequestStateGuard`).
Invalid transitions throw exceptions and are never persisted.

---

## States

- Draft
- Submitted
- AssignedToAuditor
- UnderAuditorReview
- Approved
- Rejected
- PendingAdmin
- Closed
- Deactivated

---

## Allowed Transitions

Draft → Submitted

Submitted → AssignedToAuditor

AssignedToAuditor → UnderAuditorReview

UnderAuditorReview → Approved  
UnderAuditorReview → Rejected  
UnderAuditorReview → PendingAdmin

PendingAdmin → Approved  
PendingAdmin → Rejected

Approved → Closed  
Rejected → Closed

Closed → Deactivated

Closed → Submitted (via Redressal)

---

## Invalid Transitions

- Draft → Approved
- Submitted → Approved
- Closed → Approved
- Deactivated → Any

All invalid transitions are rejected by the guard.

---

## Redressal Flow

A redressal can only be created when a request is Closed.

Closed → Submitted (Redressal created)

The request then re-enters the normal workflow with a new redressal version.
