using CareBox.BLL.DTOs.AuthDto;
using CareBox.BLL.DTOs.AuthDto.ForgotPasswordDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        // 1. Registration
        Task<AuthResponseDto> RegisterClientAsync(RegisterClientDto model);
        Task<AuthResponseDto> RegisterProviderAsync(RegisterProviderDto model);

        // 2. Authentication & Verification
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto model);

        // 3. Token Management
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);

        // 4. Password Management
        Task<AuthResponseDto> ForgotPasswordAsync(string email);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto model);
    }
}
