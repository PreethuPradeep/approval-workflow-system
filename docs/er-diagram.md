# Entity Relationship Overview

This document describes the core domain entities and their relationships.

---

```mermaid
erDiagram
    USER ||--o{ USER_ROLE : has
    ROLE ||--o{ USER_ROLE : defines

    USER ||--o{ REQUEST : submits
    USER ||--o{ REQUEST_AUDIT : performs
    USER ||--o{ REQUEST_ASSIGNMENT : assigned_to

    REQUEST ||--o{ REQUEST_ASSIGNMENT : has
    REQUEST ||--o{ REQUEST_AUDIT : logs
    REQUEST ||--o{ REDRESSAL : may_have

    REDRESSAL ||--o{ REDRESSAL_CONTENT : contains

    USER {
        int Id
        string FullName
        string Email
        string PasswordHash
    }

    ROLE {
        int Id
        string Name
    }

    USER_ROLE {
        int UserId
        int RoleId
    }

    REQUEST {
        int Id
        int RequesterId
        string CurrentState
        bool IsActive
        datetime CreatedAt
        datetime SubmittedAt
        datetime ClosedAt
        int RedressalCount
    }

    REQUEST_ASSIGNMENT {
        int Id
        int RequestId
        int AuditorId
        datetime AssignedAt
        bool IsActive
    }

    REQUEST_AUDIT {
        int Id
        int RequestId
        int ActorId
        string ActorRole
        string FromState
        string ToState
        string Action
        string Reason
        datetime CreatedAt
    }

    REDRESSAL {
        int Id
        int RequestId
        int RedressalCount
        bool IsActive
        datetime CreatedAt
        datetime ClosedAt
    }

    REDRESSAL_CONTENT {
        int Id
        int RedressalId
        string PayLoad
        datetime CreatedAt
    }

