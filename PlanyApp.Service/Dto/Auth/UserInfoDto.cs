namespace PlanyApp.Service.Dto.Auth
{
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; } 
        public bool EmailVerified { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? City { get; set; }
        public decimal? MonthlyIncome { get; set; }

        public int? CurrentPackageId { get; set; }
        public string? CurrentPackageName { get; set; }
        public DateTime? CurrentPackageStartDate { get; set; }
        public DateTime? CurrentPackageEndDate { get; set; }
    }
} 