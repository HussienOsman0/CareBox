using CareBox.BLL.DTOs.AuthDto;
using CareBox.BLL.DTOs.AuthDto.ForgotPasswordDto;
using CareBox.BLL.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 1. تسجيل عميل جديد
        // POST: api/auth/register/client
        [HttpPost("register/client")]
        public async Task<IActionResult> RegisterClient([FromBody] RegisterClientDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterClientAsync(model);

            if (!string.IsNullOrEmpty(result.Message) && !result.IsAuthenticated && result.Errors != null)
                return BadRequest(result); 

            return Ok(result); 
        }

        // 2. تسجيل مقدم خدمة جديد
        // POST: api/auth/register/provider
        [HttpPost("register/provider")]
        public async Task<IActionResult> RegisterProvider([FromForm] RegisterProviderDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterProviderAsync(model);

            if (!string.IsNullOrEmpty(result.Message) && !result.IsAuthenticated && result.Errors != null)
                return BadRequest(result);

            return Ok(result);
        }

        // 3. تفعيل الحساب (OTP)
        // POST: api/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.VerifyOtpAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }

        // 4. تسجيل الدخول
        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(result); // 401 Unauthorized لو البيانات غلط

            return Ok(result);
        }

        // 5. تجديد التوكين (Refresh Token)
        // POST: api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RevokeTokenDto model)
        {
            // ملحوظة: استخدمت RevokeTokenDto هنا لأنه يحتوي على خاصية Token فقط، 
            // وده اللي محتاجينه (عشان مبعتش string بس في الـ Body ويعمل مشاكل JSON)

            if (string.IsNullOrEmpty(model.Token))
                return BadRequest("Token is required");

            var result = await _authService.RefreshTokenAsync(model.Token);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }

        // 6. تسجيل الخروج (Revoke Token)
        // POST: api/auth/revoke-token
        [HttpPost("revoke-token/logout")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            if (string.IsNullOrEmpty(model.Token))
                return BadRequest("Token is required");

            var result = await _authService.RevokeTokenAsync(model.Token);

            if (!result)
                return BadRequest("Token is invalid or already revoked");

            return Ok(new { message = "Token revoked successfully" });
        }

        // 7. نسيت كلمة المرور (إرسال OTP)
        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(model.Email);

            return Ok(result);
        }

        // 8. إعادة تعيين كلمة المرور
        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
