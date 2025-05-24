using PlanyApp.Service.Dto.Auth;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> LoginWithGoogleAsync(GoogleLoginRequestDto googleLoginDto);
        Task<ServiceResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ServiceResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
} 