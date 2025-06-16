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
using PlanyApp.Repository.Context;

namespace PlanyApp.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository; 
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly PlanyDbContext _context;

        private static readonly Dictionary<string, DateTime> _lastEmailSentTime = new();
        private const int EMAIL_COOLDOWN_SECONDS = 60;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IEmailService emailService,
            PlanyDbContext context
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
                return new AuthResultDto { Success = false, Errors = new List<string> { "Invalid email or password." } };
            }

            if (!user.EmailVerified)
            {
                return new AuthResultDto { Success = false, Errors = new List<string> { "Please verify your email before logging in." } };
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
                    return new AuthResultDto { Success = false, Errors = new List<string> { "Invalid Google token." } };
                }

                var user = await _userRepository.GetByGoogleIdAsync(payload.Subject);

                if (user == null) // No user with this Google ID, check by email
                {
                    user = await _userRepository.GetByEmailAsync(payload.Email);
                    if (user != null) // Email exists, link Google ID
                    {
                        user.GoogleId = payload.Subject;
                        user.EmailVerified = true; // Google verified the email
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
                            PasswordHash = "GOOGLE_AUTH_USER_NO_PASSWORD_" + Guid.NewGuid().ToString()
                        };
                        await _userRepository.AddAsync(user);
                        // Re-fetch to get Id and any other DB-generated fields like Role if assigned by default
                        user = await _userRepository.GetByGoogleIdAsync(payload.Subject); 
                        if(user == null) return new AuthResultDto { Success = false, Errors = new List<string> { "Failed to create or retrieve user after Google Sign-In." } };
                    }
                }
                
                var token = GenerateJwtToken(user);
                return CreateAuthResult(user, token);
            }
            catch (InvalidJwtException ex) // Catch specific exception from Google validation
            {
                Console.WriteLine($"Google token validation error: {ex.Message}");
                return new AuthResultDto { Success = false, Errors = new List<string> { "Invalid Google token." } };
            }
            catch (Exception ex) // Catch broader exceptions
            {
                Console.WriteLine($"An unexpected error occurred during Google login: {ex.Message}");
                return new AuthResultDto { Success = false, Errors = new List<string> { "An unexpected error occurred." } };
            }
        }

        public async Task<ServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email!);
            if (existingUser != null)
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Email already exists." } };
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

            return new ServiceResponseDto { Success = true, Message = "Registration successful. Please check your email to activate your account." };
        }

        public async Task<ServiceResponseDto> ActivateEmailAsync(ActivateEmailDto activateEmailDto)
        {
            var user = await _userRepository.GetByEmailAsync(activateEmailDto.Email!);

            if (user == null)
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "User not found." } };
            }

            if (user.EmailVerified)
            {
                return new ServiceResponseDto { Success = true, Message = "Email already verified." };
            }

            // Verify the activation token (you'll need to implement this based on your token storage/verification method)
            if (!VerifyActivationToken(activateEmailDto.Token!, user))
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Invalid activation link." } };
            }

            user.EmailVerified = true;
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            return new ServiceResponseDto { Success = true, Message = "Email verified successfully. You can now log in." };
        }

        public async Task<ServiceResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email!);
            if (user == null)
            {
                // Do not reveal if the user exists or not for security reasons.
                // Always return a generic success message.
                return new ServiceResponseDto { Success = true, Message = "If an account with this email exists, a password reset link has been sent." };
            }

            // Generate a secure, URL-friendly token
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var tokenExpiryMinutes = _configuration.GetValue<int>("AppSettings:PasswordResetTokenExpiryMinutes", 60); // Default to 60 minutes

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes); // datetime2(7) can handle UTC directly
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            // Construct reset link (ensure FrontendBaseUrl is in config)
            var frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"];
            if (string.IsNullOrEmpty(frontendBaseUrl))
            {
                Console.WriteLine("Warning: FrontendBaseUrl not configured. Cannot send password reset email.");
                // Still return success to user to not leak info, but log the error.
                return new ServiceResponseDto { Success = true, Message = "If an account with this email exists, a password reset link has been sent (check server logs for config issue)." };
            }
            var resetLink = $"{frontendBaseUrl.TrimEnd('/')}/reset-password?token={token}&email={Uri.EscapeDataString(user.Email)}";

            var emailMessage = new EmailMessageDto(
                toAddress: user.Email,
                subject: "Reset Your PlanyApp Password",
                body: $"Hello {user.FullName},<br/><br/>Please click the link below to reset your password:<br/><a href=\"{resetLink}\">{resetLink}</a><br/><br/>This link will expire in {tokenExpiryMinutes} minutes.<br/><br/>If you did not request a password reset, please ignore this email.",
                isHtml: true
            );

            try
            {
                await _emailService.SendEmailAsync(emailMessage);
                return new ServiceResponseDto { Success = true, Message = "If an account with this email exists, a password reset link has been sent." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send password reset email to {user.Email}: {ex.Message}");
                // Log the error, but still return a generic success message to the user.
                return new ServiceResponseDto { Success = true, Message = "If an account with this email exists, a password reset link has been sent (email sending might have failed, check server logs)." };
            }
        }

        public async Task<ServiceResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDto.Token!) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword!))
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Token and new password are required." } };
            }

            // It's common to also pass the email with the token to make the lookup more specific
            // and to ensure the token is being used by the intended email holder.
            // For this example, we assume the token is globally unique enough for lookup,
            // but in a real app, you might get email from DTO or a claim if the user is semi-authenticated.
            
            var user = await _userRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token!);

            if (user == null || user.PasswordResetToken != resetPasswordDto.Token || user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Invalid or expired password reset token." } };
            }

            user.PasswordHash = HashPassword(resetPasswordDto.NewPassword!);
            user.PasswordResetToken = null; // Invalidate the token
            user.PasswordResetTokenExpiresAt = null; // Clear expiry
            user.EmailVerified = true; // Optionally, if password reset implies email ownership verification
            user.UpdatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _userRepository.UpdateAsync(user);

            // Optionally send a confirmation email that the password was changed.
            // var emailMessage = new EmailMessageDto(user.Email, "Your Password Has Been Changed", "Your PlanyApp password was successfully changed.");
            // await _emailService.SendEmailAsync(emailMessage);

            return new ServiceResponseDto { Success = true, Message = "Password has been reset successfully." };
        }

        public async Task<ServiceResponseDto> ResendActivationEmailAsync(ResendActivationEmailDto resendActivationEmailDto)
        {
            var user = await _userRepository.GetByEmailAsync(resendActivationEmailDto.Email!);
            if (user == null)
            {
                // Don't reveal if user exists
                return new ServiceResponseDto { Success = true, Message = "If your email exists and is not verified, an activation link has been sent." };
            }

            if (user.EmailVerified)
            {
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Email is already verified." } };
            }

            // Check rate limiting
            if (_lastEmailSentTime.TryGetValue(user.Email, out DateTime lastSentTime))
            {
                var timeSinceLastEmail = DateTime.UtcNow - lastSentTime;
                if (timeSinceLastEmail.TotalSeconds < EMAIL_COOLDOWN_SECONDS)
                {
                    var secondsToWait = EMAIL_COOLDOWN_SECONDS - (int)timeSinceLastEmail.TotalSeconds;
                    return new ServiceResponseDto 
                    { 
                        Success = false, 
                        Errors = new List<string> { $"Please wait {secondsToWait} seconds before requesting another activation email." } 
                    };
                }
            }

            var activationToken = GenerateSecureToken();

            try
            {
                await SendActivationEmail(user.Email, activationToken);
                _lastEmailSentTime[user.Email] = DateTime.UtcNow;
                return new ServiceResponseDto { Success = true, Message = "Activation email has been sent. Please check your inbox." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send activation email: {ex.Message}");
                return new ServiceResponseDto { Success = false, Errors = new List<string> { "Failed to send activation email. Please try again later." } };
            }
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
                Success = true,
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
            var frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"];
            if (string.IsNullOrEmpty(frontendBaseUrl))
            {
                throw new InvalidOperationException("Frontend base URL is not configured");
            }

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

            var activationLink = $"{frontendBaseUrl.TrimEnd('/')}/activate-account?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            var emailMessage = new EmailMessageDto(
                toAddress: email,
                subject: "Activate Your PlanyApp Account",
                body: $@"
                    <h2>Welcome to PlanyApp!</h2>
                    <p>Thank you for registering. Please click the link below to activate your account:</p>
                    <p><a href='{activationLink}'>{activationLink}</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not create this account, please ignore this email.</p>",
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