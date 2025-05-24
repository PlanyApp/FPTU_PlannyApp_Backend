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

namespace PlanyApp.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository; 
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IEmailService emailService
            )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email!);
            if (user == null || !VerifyPassword(user.PasswordHash, loginDto.Password!))
            {
                return new AuthResultDto { Success = false, Errors = new List<string> { "Invalid email or password." } };
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
                        user.UpdatedDate = DateTime.UtcNow;
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
                            // For users registering via Google, PasswordHash can be null or a placeholder
                            // as they won't log in with a traditional password unless they set one up later.
                            PasswordHash = "GOOGLE_AUTH_USER_NO_PASSWORD_" + Guid.NewGuid().ToString(), 
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow
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

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.EmailExistsAsync(registerDto.Email!))
            {
                return new AuthResultDto { Success = false, Errors = new List<string> { "Email already exists." } };
            }
            var hashedPassword = HashPassword(registerDto.Password!);
            var user = new User
            {
                FullName = registerDto.FullName!,
                Email = registerDto.Email!,
                PasswordHash = hashedPassword,
                EmailVerified = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            var createdUser = await _userRepository.GetByEmailAsync(user.Email);
            if(createdUser == null) return new AuthResultDto { Success = false, Errors = new List<string> { "Failed to retrieve user after registration." } };

            var token = GenerateJwtToken(createdUser);
            return CreateAuthResult(createdUser, token);
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

            user.PasswordResetToken = token; // Consider hashing this token if it's stored for a long time and could be guessed
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes);
            user.UpdatedDate = DateTime.UtcNow;

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
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // Optionally send a confirmation email that the password was changed.
            // var emailMessage = new EmailMessageDto(user.Email, "Your Password Has Been Changed", "Your PlanyApp password was successfully changed.");
            // await _emailService.SendEmailAsync(emailMessage);

            return new ServiceResponseDto { Success = true, Message = "Password has been reset successfully." };
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
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var jwtExpiryInMinutes = _configuration.GetValue<int>("Jwt:ExpiryInMinutes");

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
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!), // Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("UserId", user.UserId), // Changed from user.Id.ToString()
                // Add other claims as needed, e.g., roles
            };

            if (user.Role != null && !string.IsNullOrEmpty(user.Role.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 