using AxelCMS.Application.DTO;
using AxelCMS.Common.Utilities;
using AxelCMS.Domain;

namespace AxelCMS.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<PageResult<IEnumerable<UserDto>>>> GetUsersByPaginationAsync(int page, int perPage);
        Task<string> UpdateUserPhotoByUserId(string userId, UpdatePhotoDto updatePhotoDto);
        Task<ApiResponse<bool>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<ApiResponse<bool>> DeleteUserByIdAsync(string userId);
    }
}
