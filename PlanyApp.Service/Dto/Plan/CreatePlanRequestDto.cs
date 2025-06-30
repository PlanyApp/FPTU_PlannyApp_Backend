using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Plan
{
    public class CreatePlanRequestDto
    {
        [Required]
        public string Name { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsPublic { get; set; } = false;

        [Required]
        public List<PlanListItemDto> Items { get; set; }
    }

    public class PlanListItemDto
    {
        public int? ItemId { get; set; }
        
        public int DayNumber { get; set; }

        public int ItemNo { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string? Notes { get; set; }

        public decimal? Price { get; set; }
    }
} 