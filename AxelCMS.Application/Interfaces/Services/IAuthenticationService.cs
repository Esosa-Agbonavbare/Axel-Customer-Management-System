using AxelCMS.Application.DTO;
using AxelCMS.Domain;
using AxelCMS.Domain.Entities;

namespace AxelCMS.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<string>> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(User user, string roles);
        ApiResponse<string> ExtractUserIdFromToken(string authToken);
        Task<ApiResponse<string>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        ApiResponse<string> ValidateToken(ValidateTokenDto validateTokenDto);
        Task<ApiResponse<string>> ChangePasswordAsync(string authToken, UpdatePasswordDto updatePasswordDto);
        Task<ApiResponse<string>> VerifyAndAuthenticateUserAsync(string idToken);
    }
}
