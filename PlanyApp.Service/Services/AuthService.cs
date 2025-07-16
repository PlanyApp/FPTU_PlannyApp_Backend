using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Auth;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;
using PlanyApp.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using System.Linq;


namespace PlanyApp.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository; 
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly PlanyDBContext _context;

        private static readonly Dictionary<string, DateTime> _lastEmailSentTime = new();
        private const int EMAIL_COOLDOWN_SECONDS = 60;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IEmailService emailService,
            PlanyDBContext context
            )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email!);
            if (user == null || !VerifyPassword(user.PasswordHash, loginDto.Password!))
            {
                return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Email hoặc mật khẩu không hợp lệ." } };
            }

            if (!user.EmailVerified)
            {
                return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Vui lòng xác minh email của bạn trước khi đăng nhập." } };
            }

            var token = GenerateJwtToken(user);
            return CreateAuthResult(user, token);
        }

        public async Task<AuthResultDto> LoginWithGoogleAsync(GoogleLoginRequestDto googleLoginDto)
        {
            var googleClientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrEmpty(googleClientId))
            {
                Console.WriteLine("Critical Error: Google ClientId not configured.");
                throw new InvalidOperationException("Google ClientId is not configured.");
            }

            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, validationSettings);

                if (payload == null)
                {
                    return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Token Google không hợp lệ." } };
                }

                var user = await _userRepository.GetByGoogleIdAsync(payload.Subject);

                if (user == null) // No user with this Google ID, check by email
                {
                    user = await _userRepository.GetByEmailAsync(payload.Email);
                    if (user != null) // Email exists, link Google ID
                    {
                        user.GoogleId = payload.Subject;
                        user.EmailVerified = true; // Google verified the email
                        // Assign user role if not already assigned
                        if (user.RoleId == null)
                        {
                            user.RoleId = 2; // Assign standard user role
                        }
                        user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                        await _userRepository.UpdateAsync(user);
                    }
                    else // New user via Google
                    {
                        user = new User
                        {
                            FullName = payload.Name,
                            Email = payload.Email,
                            GoogleId = payload.Subject,
                            EmailVerified = payload.EmailVerified,
                            RoleId = 2, // Assign standard user role for Google users
                            PasswordHash = "GOOGLE_AUTH_USER_NO_PASSWORD_" + Guid.NewGuid().ToString()
                        };
                        await _userRepository.AddAsync(user);
                        // Re-fetch to get Id and any other DB-generated fields like Role if assigned by default
                        user = await _userRepository.GetByGoogleIdAsync(payload.Subject); 
                        if(user == null) return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Không thể tạo hoặc lấy thông tin người dùng sau khi đăng nhập Google." } };
                    }
                }
                
                var token = GenerateJwtToken(user);
                return CreateAuthResult(user, token);
            }
            catch (InvalidJwtException ex) // Catch specific exception from Google validation
            {
                Console.WriteLine($"Google token validation error: {ex.Message}");
                return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Token Google không hợp lệ." } };
            }
            catch (Exception ex) // Catch broader exceptions
            {
                Console.WriteLine($"An unexpected error occurred during Google login: {ex.Message}");
                return new AuthResultDto { IsSuccess = false, Errors = new List<string> { "Đã xảy ra lỗi không mong muốn." } };
            }
        }

        public async Task<ServiceResponseDto<string>> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email!);
            if (existingUser != null)
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Email đã tồn tại." } };
            }

            var hashedPassword = HashPassword(registerDto.Password!);
            var activationToken = GenerateSecureToken();

            var user = new User
            {
                FullName = registerDto.Email!,
                Email = registerDto.Email!,
                Phone = registerDto.Phone,
                PasswordHash = hashedPassword,
                EmailVerified = false
            };

            await _userRepository.AddAsync(user);

            // Send activation email
            await SendActivationEmail(user.Email, activationToken);

            return new ServiceResponseDto<string> { IsSuccess = true, Message = "Đăng ký thành công. Vui lòng kiểm tra email để kích hoạt tài khoản của bạn.", Data = "Success" };
        }

        public async Task<ServiceResponseDto<string>> ActivateEmailAsync(ActivateEmailDto activateEmailDto)
        {
            var user = await _userRepository.GetByEmailAsync(activateEmailDto.Email!);

            if (user == null)
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Không tìm thấy người dùng." } };
            }

            if (user.EmailVerified)
            {
                return new ServiceResponseDto<string> { IsSuccess = true, Message = "Email đã được xác minh." };
            }

            // Verify the activation token (you'll need to implement this based on your token storage/verification method)
            if (!VerifyActivationToken(activateEmailDto.Token!, user))
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Liên kết kích hoạt không hợp lệ." } };
            }

            user.EmailVerified = true;
            user.RoleId = 2; // Assign standard user role after email verification
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            return new ServiceResponseDto<string> { IsSuccess = true, Message = "Email đã được xác minh thành công. Bạn có thể đăng nhập ngay bây giờ." };
        }

        public async Task<ServiceResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email!);
            if (user == null)
            {
                // Do not reveal if the user exists or not for security reasons.
                // Always return a generic success message.
                return new ServiceResponseDto<string> { IsSuccess = true, Message = "Nếu tài khoản với email này tồn tại, một liên kết đặt lại mật khẩu đã được gửi." };
            }

            // Generate a secure, URL-friendly token
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var tokenExpiryMinutes = _configuration.GetValue<int>("AppSettings:PasswordResetTokenExpiryMinutes", 60); // Default to 60 minutes

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes); // datetime2(7) can handle UTC directly
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            // Use your landing page for password reset
            var resetLink = $"https://plany-landing.japao.dev/reset-password?token={token}&email={Uri.EscapeDataString(user.Email)}";

            var emailMessage = new EmailMessageDto(
                toAddress: user.Email,
                subject: "Đặt lại mật khẩu PlanyApp của bạn",
                body: $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
                        <div style='background-color: #2563eb; padding: 20px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>PlanyApp</h1>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <h2 style='color: #1f2937; margin-bottom: 20px;'>Yêu cầu đặt lại mật khẩu</h2>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.5;'>Xin chào {user.FullName},</p>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.5;'>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản PlanyApp của mình. Nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{resetLink}' 
                                   style='display: inline-block; background-color: #2563eb; color: #ffffff; text-decoration: none; padding: 12px 30px; border-radius: 6px; font-weight: bold; font-size: 16px;'>
                                    Đặt lại mật khẩu
                                </a>
                            </div>
                            
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Nếu nút không hoạt động, hãy sao chép và dán liên kết này vào trình duyệt của bạn:<br/>
                                <a href='{resetLink}' style='color: #2563eb; word-break: break-all;'>{resetLink}</a>
                            </p>
                            
                            <div style='background-color: #fef3c7; padding: 15px; border-radius: 6px; margin-top: 20px;'>
                                <p style='color: #92400e; font-size: 14px; margin: 0;'>
                                    ⚠️ Liên kết này sẽ hết hạn sau {tokenExpiryMinutes} phút vì lý do bảo mật.
                                </p>
                            </div>
                            
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này và mật khẩu của bạn sẽ không thay đổi.
                            </p>
                        </div>
                        
                        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #6b7280; font-size: 12px; margin: 0;'>
                                © 2025 PlanyApp.
                            </p>
                        </div>
                    </div>",
                isHtml: true
            );

            try
            {
                await _emailService.SendEmailAsync(emailMessage);
                return new ServiceResponseDto<string> { IsSuccess = true, Message = "Nếu tài khoản với email này tồn tại, một liên kết đặt lại mật khẩu đã được gửi." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send password reset email to {user.Email}: {ex.Message}");
                // Log the error, but still return a generic success message to the user.
                return new ServiceResponseDto<string> { IsSuccess = true, Message = "Nếu tài khoản với email này tồn tại, một liên kết đặt lại mật khẩu đã được gửi (việc gửi email có thể thất bại, kiểm tra log máy chủ)." };
            }
        }

        public async Task<ServiceResponseDto<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDto.Token!) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword!))
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Token và mật khẩu mới là bắt buộc." } };
            }

            // It's common to also pass the email with the token to make the lookup more specific
            // and to ensure the token is being used by the intended email holder.
            // For this example, we assume the token is globally unique enough for lookup,
            // but in a real app, you might get email from DTO or a claim if the user is semi-authenticated.
            
            var user = await _userRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token!);

            if (user == null || user.PasswordResetToken != resetPasswordDto.Token || user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Token đặt lại mật khẩu không hợp lệ hoặc đã hết hạn." } };
            }

            user.PasswordHash = HashPassword(resetPasswordDto.NewPassword!);
            user.PasswordResetToken = null; // Invalidate the token
            user.PasswordResetTokenExpiresAt = null; // Invalidate the token expiry
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            return new ServiceResponseDto<string> { IsSuccess = true, Message = "Mật khẩu của bạn đã được đặt lại thành công." };
        }

        public async Task<ServiceResponseDto<string>> ResendActivationEmailAsync(ResendActivationEmailDto resendActivationEmailDto)
        {
            var user = await _userRepository.GetByEmailAsync(resendActivationEmailDto.Email!);

            if (user == null)
            {
                return new ServiceResponseDto<string> { IsSuccess = true, Message = "Nếu tài khoản với email này tồn tại, một liên kết kích hoạt mới đã được gửi." };
            }

            if (user.EmailVerified)
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { "Email này đã được xác minh." } };
            }

            // Simple cooldown to prevent email spamming
            if (_lastEmailSentTime.TryGetValue(user.Email, out var lastSent) && (DateTime.UtcNow - lastSent).TotalSeconds < EMAIL_COOLDOWN_SECONDS)
            {
                return new ServiceResponseDto<string> { IsSuccess = false, Errors = new List<string> { $"Vui lòng đợi ít nhất {EMAIL_COOLDOWN_SECONDS} giây trước khi gửi lại email." } };
            }

            var activationToken = GenerateSecureToken();
            
            // Here you should ideally save the new activation token and its expiry to the user record in the DB.
            // For simplicity, we are just sending it, assuming the verification logic can handle multiple valid tokens or the latest one.
            // Example of updating user with a new token:
            // user.ActivationToken = activationToken;
            // user.ActivationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            // await _userRepository.UpdateAsync(user);

            await SendActivationEmail(user.Email, activationToken);

            _lastEmailSentTime[user.Email] = DateTime.UtcNow;

            return new ServiceResponseDto<string> { IsSuccess = true, Message = "Một liên kết kích hoạt mới đã được gửi đến email của bạn." };
        }

        // Helper method for password hashing (example)
        private string HashPassword(string password)
        {
           return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Helper method to verify password hash
        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // This can happen if the hash is not a valid BCrypt hash (e.g. our old placeholder)
                // Or if the salt is malformed. Treat as verification failure.
                return false;
            }
            catch (System.ArgumentException) 
            {
                // BCrypt.Verify can throw ArgumentException for invalid hash or password
                return false;
            }
        }

        private AuthResultDto CreateAuthResult(User user, string token)
        {
            return new AuthResultDto
            {
                IsSuccess = true,
                Token = token,
                UserInfo = new UserInfoDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    EmailVerified = user.EmailVerified,
                    Avatar = user.Avatar,
                    Role = user.Role?.Name
                }
            };
        }

        // Helper method to generate JWT
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["JWT:Key"];
            var jwtIssuer = _configuration["JWT:Issuer"];
            var jwtAudience = _configuration["JWT:Audience"];
            var jwtExpiryInMinutes = _configuration.GetValue<int>("JWT:ExpiryInMinutes");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || jwtExpiryInMinutes <= 0)
            {
                // Log this critical configuration error
                Console.WriteLine("Critical Error: JWT configuration is missing or invalid in appsettings.json or user secrets.");
                // Consider throwing a specific exception or handling it in a way that alerts administrators
                throw new InvalidOperationException("JWT configuration is missing or invalid.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Changed to use ClaimTypes.NameIdentifier
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.Email, user.Email!), // Changed to use ClaimTypes.Email
                new Claim("UserId", user.UserId.ToString())
            };

            // Ensure role is properly loaded and added
            if (user.Role?.Name != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name.ToLower())); // Add ToLower() to match role checks
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SendActivationEmail(string email, string token)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Save the activation token
            var activationToken = new UserActivationToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expires in 24 hours
            };
            await _context.UserActivationTokens.AddAsync(activationToken);
            await _context.SaveChangesAsync();

            // Use your landing page for account activation
            var activationLink = $"https://plany-landing.japao.dev/activation?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            var emailMessage = new EmailMessageDto(
                toAddress: email,
                subject: "Kích hoạt tài khoản PlanyApp của bạn",
                body: $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
                        <div style='background-color: #2563eb; padding: 20px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>PlanyApp</h1>
                        </div>
                        
                        <div style='padding: 30px 20px;'>
                            <h2 style='color: #1f2937; margin-bottom: 20px;'>Chào mừng bạn đến với PlanyApp!</h2>
                            <p style='color: #4b5563; font-size: 16px; line-height: 1.5;'>Cảm ơn bạn đã đăng ký PlanyApp. Để hoàn tất đăng ký và bắt đầu lên kế hoạch cho những chuyến phiêu lưu của mình, vui lòng kích hoạt tài khoản bằng cách nhấp vào nút bên dưới:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{activationLink}' 
                                   style='display: inline-block; background-color: #16a34a; color: #ffffff; text-decoration: none; padding: 12px 30px; border-radius: 6px; font-weight: bold; font-size: 16px;'>
                                    Kích hoạt tài khoản
                                </a>
                            </div>
                            
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Nếu nút không hoạt động, hãy sao chép và dán liên kết này vào trình duyệt của bạn:<br/>
                                <a href='{activationLink}' style='color: #2563eb; word-break: break-all;'>{activationLink}</a>
                            </p>
                            
                            <div style='background-color: #fef3c7; padding: 15px; border-radius: 6px; margin-top: 20px;'>
                                <p style='color: #92400e; font-size: 14px; margin: 0;'>
                                    ⚠️ Liên kết kích hoạt này sẽ hết hạn sau 24 giờ vì lý do bảo mật.
                                </p>
                            </div>
                            
                            <p style='color: #6b7280; font-size: 14px; line-height: 1.5; margin-top: 20px;'>
                                Nếu bạn không tạo tài khoản này, vui lòng bỏ qua email này.
                            </p>
                        </div>
                        
                        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='color: #6b7280; font-size: 12px; margin: 0;'>
                                © 2025 PlanyApp. Bản quyền thuộc về chúng tôi.
                            </p>
                        </div>
                    </div>",
                isHtml: true
            );
            await _emailService.SendEmailAsync(emailMessage);
        }

        private string GenerateSecureToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private bool VerifyActivationToken(string token, User user)
        {
            var activationToken = _context.UserActivationTokens
                .FirstOrDefault(t => t.Token == token && t.UserId == user.UserId);

            if (activationToken == null || activationToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            // Token is valid, remove it
            _context.UserActivationTokens.Remove(activationToken);
            return true;
        }
    }
} 