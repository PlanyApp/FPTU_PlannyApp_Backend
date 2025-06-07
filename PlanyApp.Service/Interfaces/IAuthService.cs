using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Dto;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> LoginWithGoogleAsync(GoogleLoginRequestDto googleLoginDto);
        Task<ServiceResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ServiceResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ServiceResponseDto> ActivateEmailAsync(ActivateEmailDto activateEmailDto);
        Task<ServiceResponseDto> ResendActivationEmailAsync(ResendActivationEmailDto resendActivationEmailDto);
    }
} 