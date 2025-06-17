using System;
using PlanyApp.Service.Dto.Items;

namespace PlanyApp.Service.Dto.Plan
{
    public class PlanListDto
    {
        public int PlanListId { get; set; }
        public int PlanId { get; set; }
        public int ItemId { get; set; }
        public int DayNumber { get; set; }
        public int ItemNo { get; set; }
        public decimal Price { get; set; }
        public ItemDto Item { get; set; }
    }

    public class CreatePlanListDto
    {
        public int ItemId { get; set; }
        public int DayNumber { get; set; }
        public int ItemNo { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdatePlanListDto
    {
        public int DayNumber { get; set; }
        public int ItemNo { get; set; }
        public decimal Price { get; set; }
    }
} 