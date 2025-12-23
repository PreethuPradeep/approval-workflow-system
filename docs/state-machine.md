```mermaid
stateDiagram-v2
    Draft --> Submitted
    Submitted --> AssignedToAuditor
    AssignedToAuditor --> UnderAuditorReview
    UnderAuditorReview --> Approved
    UnderAuditorReview --> Rejected
    UnderAuditorReview --> PendingAdmin
    PendingAdmin --> Approved
    PendingAdmin --> Rejected
    Approved --> Closed
    Rejected --> Closed
    Closed --> Deactivated
    Closed --> Submitted : Redressal
