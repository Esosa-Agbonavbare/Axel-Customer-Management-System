using AutoMapper;
using AxelCMS.Application.DTO;
using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Common.Utilities;
using AxelCMS.Domain;
using AxelCMS.Domain.Entities;

namespace AxelCMS.Application.ServicesImplementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService<User> _cloudinaryService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService<User> cloudinaryService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<UserDto>.Failed(false, "User not found", 404, new List<string> { "User not found" });
                }
                var userDto = _mapper.Map<UserDto>(user);
                return ApiResponse<UserDto>.Success(userDto, "User found", 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the user. UserID: {userId}", userId);
                return ApiResponse<UserDto>.Failed(false, "An error occurred", 500, new List<string> { ex.Message});
            }
        }

        public async Task<ApiResponse<PageResult<IEnumerable<UserDto>>>> GetUsersByPaginationAsync(int page, int perPage)
        {
            try
            {
                var allUsers = _unitOfWork.UserRepository.GetAllAsync();
                var pagedUsers = await Pagination<User>.GetPager(
                    (IEnumerable<User>)allUsers,
                    perPage,
                    page,
                    user => user.LastName,
                    user => user.Id.ToString()
                );
                var pagedUserDtos = _mapper.Map<PageResult<IEnumerable<UserDto>>>(pagedUsers);
                return ApiResponse<PageResult<IEnumerable<UserDto>>>.Success(pagedUserDtos, "Users found", 200);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving users by pagination. Page: {Page}, PerPage: {PerPage}", page, perPage);
                return ApiResponse<PageResult<IEnumerable<UserDto>>>.Failed(false, "An error occurred", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.Failed(false, "User not found", 404, new List<string> { "User not found" });
                }
                _mapper.Map(updateUserDto, user);
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "User updated successfully", 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating the user. UserID: {UserId}", userId);
                return ApiResponse<bool>.Failed(false, "An error occurred while updating the user", 500, new List<string> { ex.Message});
            }
        }

        public async Task<string> UpdateUserPhotoByUserId(string userId, UpdatePhotoDto updatePhotoDto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

                if (user == null)
                    return "User not found";

                var file = updatePhotoDto.PhotoFile;

                if (file == null || file.Length <= 0)
                    return "Invalid file size";

                _mapper.Map(updatePhotoDto, user);
                var imageUrl = await _cloudinaryService.UploadImage(userId, file);

                if (imageUrl == null)
                {
                    Console.WriteLine($"Failed to upload image for user with Id {userId}");
                    return null;
                }
                user.ImageUrl = imageUrl;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                return imageUrl;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in UpdateUserPhotoByUserId: {ex.Message}");
                throw;
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserByIdAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return ApiResponse<bool>.Failed(false, "User not found", 404, new List<string> { "User not found" });
                }
                await _unitOfWork.UserRepository.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "User deleted successfully", 200);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the user. UserID: {userId}", userId);
                return ApiResponse<bool>.Failed(false, "An error occurred", 500, new List<string> { ex.Message });
            }
        }
    }
}
