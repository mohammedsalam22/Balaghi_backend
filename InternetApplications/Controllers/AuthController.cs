using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Domain.Exceptions;
using Infrastructure.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
namespace InternetApplications.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
    RegisterUserService registerService,
    VerifyOtpService verifyOtpService,
    LoginService loginService,
    RefreshTokenService refreshTokenService,
    LogoutService logoutService,
    ForgotPasswordService forgotPasswordService,
    ResetPasswordService resetPasswordService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<VerifyOtpRequest> verifyValidator) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(
         [FromBody] RegisterRequest request,
         CancellationToken ct = default)
        {
            var validationResult = await registerValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            try
            {
                var response = await registerService.ExecuteAsync(request, ct);
                return Ok(response);
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken ct = default)
        {
            var validationResult = await verifyValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            try
            {
                var response = await verifyOtpService.ExecuteAsync(request, ct);
                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (OtpInvalidException)
            {
                return BadRequest(new { message = "The verification code is incorrect or expired" });
            }
        }
        [HttpPost("login")]
[EnableRateLimiting("LoginPolicy")]
public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct = default)
{
    try
    {
        var response = await loginService.ExecuteAsync(request, ct);
           Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,                   
            Secure = true,                    
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/",                         
        });
        return Ok(response);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
    catch (Exception)
    {
        return StatusCode(500, new { 
            message = "try again later" 
        });
    }
}
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct = default)
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await logoutService.ExecuteAsync(refreshToken, ct);
                }

                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { message = "You have successfully logged out" });
            }
            catch (Exception ex)
            {
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return StatusCode(500, new
                {
                    message = "Logout failed session deleted locally",
                    detail = ex.Message
                });
            }
        }

      [HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken(CancellationToken ct = default)
{
    try
    {

        if (!Request.Cookies.TryGetValue("refreshToken", out var oldToken) || 
            string.IsNullOrWhiteSpace(oldToken))
        {
            throw new UnauthorizedAccessException("Refresh Token required");
        }
        var response = await refreshTokenService.ExecuteAsync(oldToken, ct);
        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,      
            Secure = true,        
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/",          
        
        });
        return Ok(response);

    }
    catch (UnauthorizedAccessException ex)
    {
        
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Unauthorized(new { message = ex.Message });
    }
    catch (Exception)
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return StatusCode(500, new { 
            message = "try again later" 
        });
    }
}
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken ct = default)
        {
            try
            {
                await forgotPasswordService.ExecuteAsync(request, ct);
                return Ok(new { message = "A password reset link has been sent to your email" });
            }
            catch (UserNotFoundException)
            {
                
                return Ok(new { message = "If the email is registered a reset link will be sent" });
            }
            catch (UnauthorizedAccessException)
            {
                return Ok(new { message = "If the email is registered a reset link will be sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to send the reset link please try again later",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken ct = default)
        {
            try
            {
                await resetPasswordService.ExecuteAsync(request, ct);
                return Ok(new { message = "The password was successfully reset" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (UserNotFoundException)
            {
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Password reset failed please try again",
                    detail = ex.Message
                });
            }
        }
    }
}