﻿using AutoMapper;
using AxelCMS.Application.DTO;
using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Domain;
using AxelCMS.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IAuthenticationService = AxelCMS.Application.Interfaces.Services.IAuthenticationService;

namespace AxelCMS.Application.ServicesImplementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly IMapper _mapper;

        public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IOptions<EmailSettings> emailSettings, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = new EmailService(emailSettings);
            _emailSettings = emailSettings.Value;
            _mapper = mapper;
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var email = await _userManager.FindByEmailAsync(registerDto.Email);
                if (email != null)
                {
                    return new ApiResponse<string>(false, "User with this email already exist.", StatusCodes.Status400BadRequest, new List<string>());
                }
                var user = _mapper.Map<User>(registerDto);
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmEmailUrl = "https://localhost:7075/confirm-email=" + Uri.EscapeDataString(user.Email) + "&token=" + Uri.EscapeDataString(emailConfirmationToken);
                    var mailRequest = new MailRequest
                    {
                        ToEmail = user.Email,
                        Subject = "Email Confirmation for Axel Services",
                        Body = $"Thank you for registering! Please confirm your email address by clicking the link below:<br>" +
                               $"<a href='{confirmEmailUrl}'>Confirm Email</a>"
                    };
                    //await _emailService.SendEmailconfirmationAsync(mailRequest, emailConfirmationToken);
                    user.EmailConfirmed = false;
                    await _userManager.UpdateAsync(user);
                    return new ApiResponse<string>(true, "User registered successfully", StatusCodes.Status201Created, emailConfirmationToken);
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    return new ApiResponse<string>(false, "Registration failed", StatusCodes.Status404NotFound, errors);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in registering a user: {ex.Message}");
                return new ApiResponse<string>(false, "Error occurred during registration", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status404NotFound);
                }
                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    var token = GenerateJwtToken(user, role);
                    return new ApiResponse<string>(true, "Login successful", StatusCodes.Status200OK, token);
                }
                else if (result.IsLockedOut)
                {
                    return new ApiResponse<string>(false, "Account is locked out. Please try again later" +
                        $"You can unlock your account after {_userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes} minutes", StatusCodes.Status403Forbidden);
                }
                else if (result.IsNotAllowed)
                {
                    return new ApiResponse<string>(false, "Login failed, Email confirmation is required.", StatusCodes.Status401Unauthorized);
                }
                else
                {
                    return new ApiResponse<string>(false, "Login failed, invalid email or password.", StatusCodes.Status401Unauthorized);
                }
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Error occured during login", StatusCodes.Status500InternalServerError, errorList);
            }
        }

        public string GenerateJwtToken(User user, string roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                new Claim(ClaimTypes.Role, roles)
            };
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("JwtSettings:AccessTokenExpiration").Value)),
                signingCredentials: credential
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status404NotFound);
                }
                var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Email confirmation successful", StatusCodes.Status200OK);
                }
                else
                {
                    return new ApiResponse<string>(false, "Email confirmation failed", StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message };
                return ApiResponse<string>.Failed(false, "Error occurred during email confirmation", StatusCodes.Status500InternalServerError, errorList);
            }
        }

        public ApiResponse<string> ValidateToken(ValidateTokenDto validateTokenDto)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = null,
                    ValidAudience = null,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                var principal = tokenHandler.ValidateToken(validateTokenDto.Token, validationParameters, out SecurityToken securityToken);
                return new ApiResponse<string>(true, "Token is valid", StatusCodes.Status200OK);
            }
            catch (SecurityTokenException ex)
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Token validation failed", StatusCodes.Status400BadRequest, errorList);
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Error occurred during token validation", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found or email not confirmed", StatusCodes.Status404NotFound);
                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                user.PasswordResetToken = token;
                user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
                await _userManager.UpdateAsync(user);
                var resetPasswordUrl = "http://localhost:3000/reset-password?email=" + Uri.EscapeDataString(forgotPasswordDto.Email) + "&token=" + Uri.EscapeDataString(token);
                
                var mailRequest = new MailRequest
                {
                    ToEmail = forgotPasswordDto.Email,
                    Subject = "Axel Corporations Password Reset Instructions",
                    Body = $"Please reset your password by clicking <a href = '{resetPasswordUrl}'>here</a>"
                };
                //await _emailService.SendHtmlEmailAsync(mailRequest);
                return new ApiResponse<string>(true, "Password reset email sent successfully", StatusCodes.Status200OK, token);
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(true, "Error occurred while resolving password change", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status404NotFound);
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    user.PasswordResetToken = null;
                    user.ResetTokenExpires = null;

                    await _userManager.UpdateAsync(user);

                    return new ApiResponse<string>(true, "Password reset successful", StatusCodes.Status200OK);
                }
                else
                {
                    return new ApiResponse<string>(false, "Password reset failed", StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message};
                return new ApiResponse<string>(true, "Error occurred while resetting password", errorList);
            }
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(string authToken, UpdatePasswordDto updatePasswordDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    return new ApiResponse<string>(false, "Authorization token is missing", StatusCodes.Status401Unauthorized);
                }
                var userIdResponse = ExtractUserIdFromToken(authToken);

                if (!userIdResponse.Succeeded)
                {
                    return new ApiResponse<string>(false, "Failed to extract User Id from token", StatusCodes.Status401Unauthorized);
                }
                var userId = userIdResponse.Data;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found", StatusCodes.Status401Unauthorized);
                }
                var result = await _userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Password changed successfully", StatusCodes.Status200OK);
                }
                return new ApiResponse<string>(false, "Password change failed", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex) 
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(true, "Error occurred while changing password", StatusCodes.Status500InternalServerError, errorList);
            }
        }
                
        public ApiResponse<string> ExtractUserIdFromToken(string authToken)
        {
            try
            {
                var token = authToken.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new ApiResponse<string>(false, "Invalid or expired token", StatusCodes.Status401Unauthorized);
                }
                return new ApiResponse<string>(true, "User Id extracted successfully", StatusCodes.Status200OK, userId);
            }
            catch (Exception ex)
            {
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Error extracting User Id from token", StatusCodes.Status500InternalServerError, errorList);
            }
        }

        public async Task<ApiResponse<string>> VerifyAndAuthenticateUserAsync(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
                var userId = payload.Subject;
                var userEmail = payload.Email;
                var userName = payload.Name;
                var firstName = payload.GivenName;
                var lastName = payload.FamilyName;
                var existingUser = await _userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        Email = userEmail,
                        UserName = userEmail,
                        FirstName = firstName,
                        LastName = lastName,
                    };
                    var result = await _userManager.CreateAsync(newUser);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                        var Token = GenerateJwtToken(newUser, "User"); //how to assign role using google authentication?
                        return new ApiResponse<string>(true, "User created and authenticated successfully on the server side", StatusCodes.Status200OK, Token);
                    }
                    else
                    {
                        return new ApiResponse<string>(false, "User creation failed", StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    var Token = GenerateJwtToken(existingUser, "User"); //how to assign role using google authentication?
                    return new ApiResponse<string>(true, "User authenticated successfully on the server side", StatusCodes.Status200OK, Token);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error occurred while authenticating user", StatusCodes.Status500InternalServerError, new List<string> { ex.Message});
            }
        }
    }
}
