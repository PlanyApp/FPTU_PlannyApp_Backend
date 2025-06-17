using System;
using System.Collections.Generic;

namespace PlanyApp.Service.Dto.Plan
{
    public class PlanDto
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string Status { get; set; } = "Draft";
        public decimal TotalCost { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PlanListDto> PlanItems { get; set; } = new List<PlanListDto>();
    }

    public class CreatePlanDto
    {
        public string Name { get; set; }
        public int OwnerId { get; set; }
    }

    public class UpdatePlanDto
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }
} 