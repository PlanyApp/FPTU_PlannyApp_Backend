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
    }
} 