namespace PlanyApp.Service.Dto.Auth
{
    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public UserInfoDto? UserInfo { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 