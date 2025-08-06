using System;

namespace PlanyApp.API.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public int? RoleId { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? City { get; set; }
        public decimal? MonthlyIncome { get; set; }
    }
} 