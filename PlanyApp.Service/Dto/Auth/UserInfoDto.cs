namespace PlanyApp.Service.Dto.Auth
{
    public class UserInfoDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; } 
        public bool EmailVerified { get; set; }
    }
} 