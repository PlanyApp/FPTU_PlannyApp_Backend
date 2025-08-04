using System;
using System.Collections.Generic;
using PlanyApp.Service.Dto.Province;

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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<PlanListDto> PlanItems { get; set; } = new List<PlanListDto>();
        
        // Province information
        public int? ProvinceId { get; set; }
        public ProvinceDto? Province { get; set; }
        
        // Rating information
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
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