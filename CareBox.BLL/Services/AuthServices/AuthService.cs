using CareBox.BLL.DTOs.AuthDto;
using CareBox.BLL.DTOs.AuthDto.ForgotPasswordDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.AuthServices.Interfaces;
using CareBox.BLL.Services.AuthServices.Settings;
using CareBox.BLL.Services.EmailServices.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.AuthServices
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly JwtOptions _jwtOptions;
        private readonly IFileService _fileService;

        public AuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService, IOptions<JwtOptions> jwtOptions, IFileService fileService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _jwtOptions = jwtOptions.Value;
            _fileService = fileService;
        }



        #region RegisterClient
        public async Task<AuthResponseDto> RegisterClientAsync(RegisterClientDto model)
        {
            // 1. التأكد إن الإيميل مش موجود قبل كده
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthResponseDto { Message = "Email is already registered!" };
            
                // 2. إنشاء الـ User (Identity)
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Email, // بنستخدم الإيميل كاسم مستخدم
                    PhoneNumber = model.PhoneNumber,
                    SecurityStamp = Guid.NewGuid().ToString() // مهم جداً للأمان
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                        errors.Add(error.Description);

                    return new AuthResponseDto { Message = "Registration Failed", Errors = errors };
                }

                // 3. تعيين دور (Role) - اختياري بس مفيد
                await _userManager.AddToRoleAsync(user, "Client"); // تأكد إن الـ Role "Client" موجود في الداتابيز

                // 4. إنشاء الـ Client Profile وربطه بالـ User
                var client = new Client
                {
                    AppUserId = user.Id, // هنا الربط 1:1
                    FullName = model.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    // بنستخدم الـ UnitOfWork لإضافة الـ Client
                    await _unitOfWork.Clients.AddAsync(client);
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    // لو حصل مشكلة في حفظ الـ Client، لازم نمسح الـ User عشان ميحصلش بيانات معلقة
                    await _userManager.DeleteAsync(user);
                    return new AuthResponseDto { Message = "Failed to create client profile: " + ex.Message };
                }

                // 5. توليد OTP (6 أرقام عشوائية)
                var otp = new Random().Next(100000, 999999).ToString();

                // 6. حفظ الـ OTP في الداتابيز
                user.OTPCode = otp;
                user.OTPExpiryTime = DateTime.UtcNow.AddMinutes(30); // صلاحية 30 دقايق
                await _userManager.UpdateAsync(user);

                // 7. إرسال الإيميل
                var emailBody = $@"
                <h3>Welcome to CareBox!</h3>
                <p>Thanks for registering. Please use the code below to verify your account:</p>
                <h2 style='color:blue;'>{otp}</h2>
                <p>This code is valid for 10 minutes.</p>";

                await _emailService.SendEmailAsync(user.Email, "CareBox Verification Code", emailBody);

                return new AuthResponseDto
                {
                    IsAuthenticated = false, // لسه مش Authenticated لحد ما يعمل Verify
                    Message = "User registered successfully. Please check your email for the OTP.",
                    Email = user.Email
                };

        }
        #endregion

        #region RegisterProvider

        public async Task<AuthResponseDto> RegisterProviderAsync(RegisterProviderDto model)
        {
            // 1. التأكد إن الإيميل مش موجود
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthResponseDto { Message = "Email is already registered!" };

                // 2. إنشاء حساب Identity User
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors) errors.Add(error.Description);
                    return new AuthResponseDto { Message = "Registration Failed", Errors = errors };
                }

                // 3. إضافة الدور (Role)
                await _userManager.AddToRoleAsync(user, "ServiceProvider"); // تأكد إن الرول ده موجود في الـ DB

                // 4. تجهيز الـ Location Point (Geometry)
                // SRID 4326 هو المعيار العالمي للـ GPS (WGS 84)
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var locationPoint = geometryFactory.CreatePoint(new Coordinate(model.Longitude, model.Latitude));

                string? logoUrl = null;
                if (model.LogoImage != null)
                {
                    // بنحفظ الصورة في فولدر اسمه "providers"
                    logoUrl = await _fileService.SaveFileAsync(model.LogoImage, "providers");
                }

            // 5. إنشاء بروفايل مقدم الخدمة
            var provider = new ServiceProvider
                {
                    AppUserId = user.Id, // الربط 1:1
                    Name = model.Name,
                    Address = model.Address,
                    ProviderTypeId = model.ProviderTypeId,
                    WorkingHours = model.WorkingHours,
                    Location = locationPoint,
                    LogoImageUrl = logoUrl,
                    CreatedAt = DateTime.UtcNow
                    // LogoImageUrl هيفضل null لحد ما نعمل Upload API
                };

                try
                {
                    await _unitOfWork.ServiceProviders.AddAsync(provider);
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    // لو فشل حفظ البروفايل، لازم نمسح اليوزر عشان ميبقاش عندنا بيانات "يتيمة"
                    await _userManager.DeleteAsync(user);
                    return new AuthResponseDto { Message = "Failed to create provider profile: " + ex.Message };
                }

                // 6. توليد OTP وإرسال الإيميل (نفس منطق الـ Client)
                var otp = new Random().Next(100000, 999999).ToString();
                user.OTPCode = otp;
                user.OTPExpiryTime = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                var emailBody = $@"
            <h3>Welcome to CareBox Partner Program!</h3>
            <p>Your business account has been created. Use code below to verify:</p>
            <h2 style='color:green;'>{otp}</h2>";

                await _emailService.SendEmailAsync(user.Email, "Verify Your Provider Account", emailBody);

                return new AuthResponseDto
                {
                    IsAuthenticated = false,
                    Message = "Provider registered successfully. Please verify your email.",
                    Email = user.Email
                };
        }
        #endregion

        #region VerifyOtpAsync
        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto model)
        {
            // 1. البحث عن المستخدم
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDto { Message = "User not found", IsAuthenticated = false };

            // 2. التحقق من وجود OTP وصحته
            if (user.OTPCode != model.OtpCode)
                return new AuthResponseDto { Message = "Invalid OTP", IsAuthenticated = false };

            // 3. التحقق من الصلاحية (Expiry)
            if (user.OTPExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { Message = "OTP has expired", IsAuthenticated = false };

            // 4. تفعيل الحساب وتنظيف الـ OTP
            user.EmailConfirmed = true; // دي خاصية في IdentityUser
            user.OTPCode = null;       // بنمسحه عشان ميتعملش بيه Verify تاني
            user.OTPExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new AuthResponseDto { Message = "Error verifying account", IsAuthenticated = false };

            // 5. الرد بنجاح (بدون Token حسب طلبك)
            return new AuthResponseDto
            {
                Message = "Account verified successfully. You can now login.",
                IsAuthenticated = true, // العملية نجحت
                Email = user.Email,
                Token = null, // التوكين هيتولد في الـ Login
                RefreshToken = null
            };
        }
        #endregion

        #region Login /Logout
        //login
        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            // 1. البحث عن المستخدم بالإيميل
            var user = await _userManager.FindByEmailAsync(model.Email);

            // 2. التحقق من وجود المستخدم وصحة الباسوورد
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResponseDto
                {
                    Message = "Invalid Email or Password",
                    IsAuthenticated = false
                };
            }

            // 3. التحقق من تفعيل الحساب (OTP Verification)
            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    Message = "Account is not verified. Please verify your email first.",
                    IsAuthenticated = false
                };
            }

            // 4. إنشاء الـ JWT Access Token (باستخدام الدالة المساعدة اللي عملناها)
            var jwtSecurityToken = await CreateJwtToken(user);
            var accessToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // 5. إنشاء الـ Refresh Token (باستخدام الدالة المساعدة اللي عملناها)
            var refreshToken = GenerateRefreshToken();

            // 6. ربط الـ Refresh Token بالمستخدم
            refreshToken.UserId = user.Id;
            refreshToken.DeviceId = model.DeviceId ?? "Unknown Device"; // لو مبعتش اسم الجهاز

            // 7. حفظ الـ Refresh Token في قاعدة البيانات
            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveAsync();

            // 8. تجهيز وإرجاع الرد النهائي
            return new AuthResponseDto
            {
                Message = "Login Successful",
                IsAuthenticated = true,
                Token = accessToken,
                TokenExpiration = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken.TokenHash,
                RefreshTokenExpiration = refreshToken.ExpiryDate,
                Email = user.Email,
                Username = user.UserName,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }



        //Logout
        public async Task<bool> RevokeTokenAsync(string token)
        {
            // 1. البحث عن التوكين في قاعدة البيانات
            var refreshToken = await _unitOfWork.RefreshTokens.FindAsync(t => t.TokenHash == token);

            // 2. لو التوكين مش موجود، نرجع false
            if (refreshToken == null)
                return false;

            // 3. لو هو أصلاً ملغي، نرجع true (لأن الهدف تحقق خلاص)
            if (refreshToken.IsRevoked)
                return true;

            // 4. إلغاء التوكين وتحديث الحالة
            refreshToken.IsRevoked = true;

            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.SaveAsync();

            return true;
        }




        #endregion

        #region Tokens
        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            // 1. تحديد الـ Claims (المعلومات الموجودة جوه التوكين)
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // الـ ID
            new Claim(ClaimTypes.Name, user.UserName),                // الـ Username
            new Claim(ClaimTypes.Email, user.Email),                  // الـ Email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // معرف فريد للتوكين
        };

            // 2. إضافة أدوار المستخدم (Roles) للـ Claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. تجهيز مفتاح التشفير (Security Key)
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            // 4. إنشاء التوكين
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }


        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32]; // 32 bytes = 256 bits (قوي جداً)

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return new RefreshToken
                {
                    TokenHash = Convert.ToBase64String(randomNumber), // ده التوكين اللي هيروح لليوزر
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenValidityInDays), // مدة الصلاحية (مثلاً 7 أيام)
                    IsRevoked = false, // لسه جديد
                                       // الـ UserId والـ DeviceId هيتحطوا في دالة الـ Login
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token)
        {
            // 1. البحث عن الـ Refresh Token في قاعدة البيانات
            var storedRefreshToken = await _unitOfWork.RefreshTokens.FindAsync(t => t.TokenHash == token);

            // 2. التحقق من وجود التوكين وصحته
            if (storedRefreshToken == null)
                return new AuthResponseDto { Message = "Invalid Token", IsAuthenticated = false };

            // 3. التحقق من تاريخ الانتهاء
            if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
                return new AuthResponseDto { Message = "Token Expired", IsAuthenticated = false };

            // 4. التحقق هل تم إيقافه (Revoked) من قبل؟ (حماية من السرقة)
            if (storedRefreshToken.IsRevoked)
                return new AuthResponseDto { Message = "Token Revoked", IsAuthenticated = false };

            // 5. جلب المستخدم صاحب التوكين
            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
            if (user == null)
                return new AuthResponseDto { Message = "User not found", IsAuthenticated = false };

            // 6. إنشاء توكينات جديدة (JWT + Refresh Token)
            var newJwtToken = await CreateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // ربط الـ Refresh Token الجديد بالمستخدم
            newRefreshToken.UserId = user.Id;
            newRefreshToken.DeviceId = storedRefreshToken.DeviceId; // بنحافظ على نفس الجهاز

            // 7. إيقاف التوكين القديم (Revoke Old Token)
            storedRefreshToken.IsRevoked = true;
            _unitOfWork.RefreshTokens.Update(storedRefreshToken);

            // 8. حفظ التوكين الجديد في قاعدة البيانات
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
            await _unitOfWork.SaveAsync();

            // 9. إرجاع النتيجة
            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(newJwtToken),
                TokenExpiration = newJwtToken.ValidTo,
                RefreshToken = newRefreshToken.TokenHash,
                RefreshTokenExpiration = newRefreshToken.ExpiryDate,
                Email = user.Email,
                Username = user.UserName,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Message = "Token Refreshed Successfully"
            };
        }
        #endregion

        #region FogotPassword & reset Password
        // forgot password
        public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResponseDto { Message = "If email exists, OTP sent.", IsAuthenticated = false };
            // رسالة غامضة لدواعي الأمان عشان الهاكر ميعرفش مين مسجل ومين لأ

            // 1. توليد OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // 2. حفظ الـ OTP في الداتابيز
            user.OTPCode = otp;
            user.OTPExpiryTime = DateTime.UtcNow.AddMinutes(10); // صلاحية 10 دقايق
            await _userManager.UpdateAsync(user);

            // 3. إرسال الإيميل
            var emailBody = $@"
            <h3>Password Reset Request</h3>
            <p>To reset your password, please use the following code:</p>
            <h2 style='color:red;'>{otp}</h2>
            <p>If you didn't request this, please ignore this email.</p>";

            await _emailService.SendEmailAsync(email, "CareBox Password Reset", emailBody);

            return new AuthResponseDto
            {
                Message = "OTP sent to your email.",
                IsAuthenticated = true // العملية نجحت (كإرسال)
            };
        }


        //reset password
        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDto { Message = "User not found", IsAuthenticated = false };

            // 1. التحقق من الـ OTP
            if (user.OTPCode != model.Otp)
                return new AuthResponseDto { Message = "Invalid OTP", IsAuthenticated = false };

            if (user.OTPExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { Message = "OTP has expired", IsAuthenticated = false };

            // 2. تغيير الباسوورد باستخدام Identity Token داخلي
            // (لأن Identity بتحتاج Token عشان تغير الباسوورد بأمان)
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors) errors.Add(error.Description);
                return new AuthResponseDto { Message = "Failed to reset password", Errors = errors, IsAuthenticated = false };
            }

            // 3. تنظيف الـ OTP بعد الاستخدام
            user.OTPCode = null;
            user.OTPExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Message = "Password has been reset successfully. You can login now.",
                IsAuthenticated = true
            };
        }



        #endregion




    }
}
