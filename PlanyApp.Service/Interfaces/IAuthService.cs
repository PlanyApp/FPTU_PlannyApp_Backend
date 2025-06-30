using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Dto;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponseDto<string>> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> LoginWithGoogleAsync(GoogleLoginRequestDto googleLoginDto);
        Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ServiceResponseDto<string>> ActivateEmailAsync(ActivateEmailDto activateEmailDto);
        Task<ServiceResponseDto<string>> ResendActivationEmailAsync(ResendActivationEmailDto resendActivationEmailDto);
    }
} 