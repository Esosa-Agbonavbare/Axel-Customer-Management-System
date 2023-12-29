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
        Task<ApiResponse<string>> ConfirmEmailAsync(User user, string token);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        ApiResponse<string> ValidateToken(string token);
        Task<ApiResponse<string>> ChangePasswordAsync(User user, string authToken, string currentPassword, string newPassword);
    }
}
