using System;

namespace PlanyApp.Repository.Models
{
    public class PlanAuditLog
    {
        public int PlanAuditLogId { get; set; }
        public int PlanId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = string.Empty; // e.g., CreatePlan, UpdatePlan, UpdatePlanItem, DeletePlanItem, DeletePlan
        public string? ChangesJson { get; set; } // optional JSON describing changes
        public DateTime CreatedAt { get; set; }

        public virtual Plan Plan { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
} 