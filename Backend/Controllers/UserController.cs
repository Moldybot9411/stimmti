using Backend.Dto;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<UserController> _logger;
    private readonly IWebHostEnvironment _env;

    public UserController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<UserController> logger,
        IWebHostEnvironment env)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto data)
    {
        var user = new User
        {
            UserName = data.Username,
            Email = data.Email,
        };

        var result = await _userManager.CreateAsync(user, data.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("New User created");

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok(new { message = "Registration Successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto data)
    {
        var user = await _userManager.FindByEmailAsync(data.Email);
        if (user == null)
        {
            return BadRequest("Incorrect E-Mail or Password");
        }

        var result = _signInManager.PasswordSignInAsync(
            user,
            data.Password,
            isPersistent: data.StaySignedIn,
            lockoutOnFailure: true
        );

        if (result.Result.Succeeded)
        {
            await _signInManager.SignInAsync(user, data.StaySignedIn);
            return Ok(new { message = "Login Successful" });
        }

        if (result.Result.IsLockedOut)
        {
            return BadRequest("This account was temporarily locked because of too many failed Sign-In requests");
        }

        return BadRequest("Incorrect E-Mail or Password");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Successfully logged out" });
    }

    [HttpGet("checkUsername")]
    [ProducesResponseType(typeof(UserUsernameAvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserUsernameAvailabilityResponseDto>> CheckUsername([FromQuery] UserUsernameCheckRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user != null)
        {
            return Ok(new UserUsernameAvailabilityResponseDto
            {
                IsAvailable = false,
                Message = "User with this name already exists"
            });
        }

        return Ok(new UserUsernameAvailabilityResponseDto
        {
            IsAvailable = true,
            Message = "Username is available"
        });
    }

    [HttpPost("checkEmail")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            return BadRequest("User with E-Mail already exists");
        }

        return Ok(new { message = "E-Mail is available" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserAuthDto>> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var userData = new UserAuthDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            ProfilePictureUrl = user.ProfilePictureUrl
        };

        return Ok(userData);
    }

    [HttpPatch("username")]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ChangeUsername([FromBody] UserUsernameCheckRequestDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        data.Username = data.Username.Trim();

        var res = await _userManager.SetUserNameAsync(user, data.Username);

        if (!res.Succeeded)
        {
            return Conflict(res.Errors);
        }

        return Ok(data.Username);
    }

    [HttpPatch("email")]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] string newEmail)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        newEmail = newEmail.Trim();

        var res = await _userManager.SetEmailAsync(user, newEmail);

        if (!res.Succeeded)
        {
            return Conflict(res.Errors);
        }

        return Ok(newEmail);
    }

    [HttpPatch("password")]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] UserPasswordDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var res = await _userManager.ChangePasswordAsync(user, data.OldPassword, data.NewPassword);

        if (!res.Succeeded)
        {
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok();
    }

    [HttpPost("profilePicture")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid file", Detail = "No file was uploaded" });
        }

        if (file.Length > 2 * 1024 * 1024 /*2MB*/)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid file", Detail = "File is too big (max. 2MB)" });
        }

        var allowedExtension = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtension.Contains(extension))
        {
            return BadRequest(new ProblemDetails { Title = "Invalid File", Detail = "Only allowed extensions are jpg, jpeg, png and webp" });
        }

        var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            try
            {
                var oldFileUri = new Uri(user.ProfilePictureUrl);
                var oldFileName = Path.GetFileName(oldFileUri.LocalPath);

                var oldFilePath = Path.Combine(webRootPath, "uploads", "avatars", oldFileName);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Couldn't delete old profile picture: {ex.Message}");
            }
        }

        var fileName = $"avatar_{user.Id}_{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(webRootPath, "uploads", "avatars");

        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/avatars/{fileName}";

        user.ProfilePictureUrl = fileUrl;
        var res = await _userManager.UpdateAsync(user);

        if (!res.Succeeded)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ProblemDetails { Title = "Database Error", Detail = "Failed to save profile picture" }
            );
        }

        return Ok(fileUrl);
    }

    [HttpDelete("DeleteUser")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserAuthDto>> DeleteUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (user.ProfilePictureUrl != null)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

            var oldFileUri = new Uri(user.ProfilePictureUrl);
            var oldFileName = Path.GetFileName(oldFileUri.LocalPath);

            var oldFilePath = Path.Combine(webRootPath, "uploads", "avatars", oldFileName);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok();
    }
}