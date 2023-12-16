using AxelCMS.Application.DTO;
using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AxelCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _userService.GetUserByIdAsync(userId));
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsersByPagination(int page, int perPage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _userService.GetUsersByPaginationAsync(page, perPage));
        }

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUserAsync(string userId, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _userService.UpdateUserAsync(userId, updateUserDto));
        }

        [HttpPatch("photo/{userId}")]
        public async Task<IActionResult> UpdateUserPhotoByUserId(string userId, [FromForm] UpdatePhotoDto updatePhotoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            try
            {
                var imageUrl = await _userService.UpdateUserPhotoByUserId(userId, updatePhotoDto);

                if (imageUrl == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(false, "An error occurred while updating user photo", StatusCodes.Status500InternalServerError));

                return Ok(new ApiResponse<string>(true, "User photo updated successfully", StatusCodes.Status200OK, new List<string> { imageUrl }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUserByIdAsync(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _userService.DeleteUserByIdAsync(userId));
        }
    }
}
