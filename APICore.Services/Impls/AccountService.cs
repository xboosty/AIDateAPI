using Amazon.S3.Model;
using APICore.Common.DTO.Request;
using APICore.Common.Helpers;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using APICore.Services.Exceptions.BadRequest;
using APICore.Services.Exceptions.Unauthorized;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using PasswordGenerator;
using PhoneNumbers;
using rlcx.suid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace APICore.Services.Impls
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IAccountService> _localizer;
        private readonly IDetectionService _detectionService;
        private readonly IStorageService _storageService;
        private readonly ITwilioService _twilioService;

        public AccountService(IConfiguration configuration, IUnitOfWork uow,
            IStringLocalizer<IAccountService> localizer,
            IDetectionService detectionService, IStorageService storageService,
            ITwilioService twilioService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _detectionService = detectionService ?? throw new ArgumentNullException(nameof(detectionService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _twilioService = twilioService ?? throw new ArgumentNullException(nameof(twilioService));
        }

        public async Task<(User user, string accessToken, string refreshToken)> LoginAsync(LoginRequest loginRequest)
        {
            var hashedPass = GetSha256Hash(loginRequest.Password);
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Email == loginRequest.Email.ToLower());
            if (user == null)
            {
                throw new InvalidCredentialsBadrequestException(_localizer);
            }
            if (user.Password != hashedPass)
            {
                throw new InvalidCredentialsBadrequestException(_localizer);
            }
            if (user.Status != StatusEnum.ACTIVE)
            {
                throw new AccountInactiveForbiddenException(_localizer);
            }
            var userData = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == user.Id);

            var dd = GetDeviceDetectorConfigured();
            var clientInfo = dd.GetClient();
            var osrInfo = dd.GetOs();
            var device1 = dd.GetDeviceName();
            var brand = dd.GetBrandName();
            var model = dd.GetModel();
            var claims = GetClaims(user);
            var token = GetToken(claims);
            var refreshToken = GetRefreshToken();
            var t = new UserToken();
            t.AccessToken = token;
            t.AccessTokenExpiresDateTime = DateTime.UtcNow.AddHours(int.Parse(_configuration.GetSection("BearerTokens")["AccessTokenExpirationHours"]));
            t.RefreshToken = refreshToken;
            t.RefreshTokenExpiresDateTime = DateTime.UtcNow.AddHours(int.Parse(_configuration.GetSection("BearerTokens")["RefreshTokenExpirationHours"]));
            t.UserId = user.Id;
            t.DeviceModel = model;
            t.DeviceBrand = brand;
            t.OS = osrInfo.Match?.Name;
            t.OSPlatform = osrInfo.Match?.Platform;
            t.OSVersion = osrInfo.Match?.Version;
            t.ClientName = clientInfo.Match?.Name;
            t.ClientType = clientInfo.Match?.Type;
            t.ClientVersion = clientInfo.Match?.Version;
            await _uow.UserTokenRepository.AddAsync(t);
            await _uow.CommitAsync();
            return (userData, token, refreshToken);
        }

        private async Task<bool> VerifyCode(VerificationCodeRequest request, User user)
        {
            if (string.IsNullOrWhiteSpace(request.VerificationCode)) throw new VerificationCodeDoesntMatchBadrequestException(_localizer);

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (!ValidateEmail(request.Email)) throw new EmailNotValidBadRequestException(_localizer);

                if (request.VerificationCode != user.VerificationCode) throw new VerificationCodeDoesntMatchBadrequestException(_localizer);

                if (((DateTime.UtcNow - user.CreatedCode).TotalSeconds / 60) > 10) throw new VerificationCodeExpiredBadrequestException(_localizer);
            }
            else if (request.Phone != null)
            {
                if (ValidatePhone(request.Phone.Number, request.Phone.Code) == null) throw new PhoneNotValidBadRequestException(_localizer);

                var verification = await _twilioService
                    .CheckSentVerificationCode(request.Phone.Code + request.Phone.Number, request.VerificationCode);
                if ((bool)!verification.Valid) throw new VerificationCodeDoesntMatchBadrequestException(_localizer);
            }
            else throw new IdentityFieldBadRequestException(_localizer);

            return true;
        }

        public async Task<bool> ValidateVerificationCodeAsync(VerificationCodeRequest request)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower() && !string.IsNullOrEmpty(request.Email));
            user = (user == null) ? (await _uow.UserRepository.FirstOrDefaultAsync(u => (u.PhoneCode == request.Phone.Code && u.Phone == request.Phone.Number) && !string.IsNullOrEmpty(request.Phone.Code + request.Phone.Number)))
                : user;
            if (user == null) throw new UserNotFoundException(_localizer);
            var verify = await VerifyCode(request, user);
            if (verify)
            {
                if (!String.IsNullOrEmpty(request.Email))
                {
                    user.IsEmailVerified = true;
                }
                else
                {
                    user.IsPhoneVerified = true;
                }

                if (user.Status == StatusEnum.INACTIVE)
                {
                    user.Status = StatusEnum.ACTIVE;
                }
                user.VerificationCode = request.VerificationCode;
                await _uow.UserRepository.UpdateAsync(user, user.Id);
                await _uow.CommitAsync();
            }

            return verify;
        }

        public bool ValidateEmail(string email)
        {
            return (new EmailAddressAttribute().IsValid(email));
        }

        private string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GetToken(IEnumerable<Claim> claims)
        {
            var issuer = _configuration.GetSection("BearerTokens")["Issuer"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("BearerTokens")["Key"]));

            var jwt = new JwtSecurityToken(issuer: issuer,
                audience: _configuration.GetSection("BearerTokens")["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(int.Parse(_configuration.GetSection("BearerTokens")["AccessTokenExpirationHours"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task LogoutAsync(string accessToken, int userId)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new UserNotFoundException(_localizer);
            if (user.Status == StatusEnum.INACTIVE) throw new AccountInactiveForbiddenException(_localizer);

            var token = await _uow.UserTokenRepository.FirstOrDefaultAsync(t => t.UserId == userId);
            if (token == null) throw new UnauthorizedAccessException();
            _uow.UserTokenRepository.Delete(token);
            await _uow.CommitAsync();
        }

        public async Task<bool> RecoveryUserPasswordAsync(RecoveryPasswordRequest request)
        {
            User user = await GetUserByPhoneAsync(request.Phone.Code, request.Phone.Number);

            if (user.Status == StatusEnum.INACTIVE) throw new AccountInactiveForbiddenException(_localizer);

            if (user.Password == GetSha256Hash(request.Password))
            {
                throw new SameNewAndOldPasswordBadRequestException(_localizer);
            }

            if (user.VerificationCode != request.twilioCode)
            {
                throw new IncorrectVerificationCodeUnauthorizedException(_localizer);
            }

            return await ChangeUserPassword(request, user);
        }

        public async Task<User> GetUserByPhoneAsync(string phoneCode, string phoneNumber)
        {
            var phoneNumber1 = ValidatePhone(phoneNumber, phoneCode);
            if (phoneNumber1 == null)
            {
                throw new PhoneNotValidBadRequestException(_localizer);
            }
            return (await _uow.UserRepository.FirstOrDefaultAsync(u => (u.PhoneCode == phoneCode && u.Phone == phoneNumber) && !string.IsNullOrEmpty(phoneCode + phoneNumber))) ?? throw new UserNotFoundException(_localizer);
        }

        public async Task<bool> ChangeUserPasswordAsync(ChangePasswordRequest request, int userId)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new UserNotFoundException(_localizer);

            if (user.Password != GetSha256Hash(request.OldPassword)) throw new UnauthorizedException(_localizer);

            return await ChangeUserPassword(new RecoveryPasswordRequest()
            {
                Password = request.Password,
                ConfirmationPassword = request.ConfirmationPassword
            }, user);
        }

        private async Task<bool> ChangeUserPassword(RecoveryPasswordRequest request, User user)
        {
            if (!request.Password.MatchWithPasswordPolicy()) throw new PasswordRequirementsBadRequestException(_localizer);
            if (request.Password != request.ConfirmationPassword) throw new PasswordsDoesntMatchBadRequestException(_localizer);

            user.Password = GetSha256Hash(request.Password);
            user.IsGeneratedPassChanged = true;
            await _uow.UserRepository.UpdateAsync(user, user.Id);
            await _uow.CommitAsync();

            return true;
        }

        public async Task<User> SignUpAsync(SignUpRequest suRequest)
        {
            if (!suRequest.Password.MatchWithPasswordPolicy()) throw new PasswordRequirementsBadRequestException(_localizer);
            if (suRequest.Password != suRequest.ConfirmationPassword) throw new PasswordsDoesntMatchBadRequestException(_localizer);

            if (string.IsNullOrEmpty(suRequest.Email) || string.IsNullOrEmpty(suRequest.Phone.Code + suRequest.Phone.Number))
            {
                throw new BaseBadRequestException();
            }

            string verificationCode = new Password().IncludeNumeric().LengthRequired(6).Next();
            User existingUser = null;

            var userEmail = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Email == suRequest.Email.ToLower() && u.Status == StatusEnum.ACTIVE);
            if (userEmail != null) throw new EmailInUseBadRequestException(_localizer);

            userEmail = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Email == suRequest.Email.ToLower() &&
                                                                     (u.PhoneCode != suRequest.Phone.Code || u.Phone != suRequest.Phone.Number));
            if (userEmail != null) throw new EmailInUseBadRequestException(_localizer);

            existingUser = await RegisterPhone(suRequest.Phone.Code, suRequest.Phone.Number, false);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Email = suRequest.Email.ToLower(),
                    IsEmailVerified = false,
                    FullName = suRequest.FullName,
                    Phone = suRequest.Phone.Number,
                    PhoneCode = suRequest.Phone.Code,
                    IsPhoneVerified = false,
                    Password = GetSha256Hash(suRequest.Password),
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    Identity = Suid.NewLettersOnlySuid(),
                    Status = StatusEnum.INACTIVE,
                    VerificationCode = verificationCode,
                    CreatedCode = DateTime.UtcNow,
                    IsGeneratedPassChanged = true,
                    SexualOrientation = (SexualOrientationEnum)suRequest.SexualOrientation
                };

                await _uow.UserRepository.AddAsync(existingUser);
            }
            else
            {
                existingUser.Email = suRequest.Email.ToLower();
                existingUser.IsEmailVerified = false;
                existingUser.FullName = suRequest.FullName;
                existingUser.Phone = suRequest.Phone.Number;
                existingUser.SexualOrientation = (SexualOrientationEnum)suRequest.SexualOrientation;
                existingUser.PhoneCode = suRequest.Phone.Code;
                existingUser.IsPhoneVerified = false;
                existingUser.Password = GetSha256Hash(suRequest.Password);
                existingUser.CreatedAt = DateTime.UtcNow;
                existingUser.ModifiedAt = DateTime.UtcNow;
                existingUser.VerificationCode = verificationCode;
                existingUser.CreatedCode = DateTime.Now;
                existingUser.IsGeneratedPassChanged = true;

                _uow.UserRepository.Update(existingUser);
            }
            await _uow.CommitAsync();

            return existingUser;
        }

        public async Task<User> RegisterPhone(string phoneCode, string phoneNumber, bool toValidateOldPhone)
        {
            if (ValidatePhone(phoneNumber, phoneCode) == null) throw new PhoneNotValidBadRequestException(_localizer);

            if (!toValidateOldPhone)
            {
                var userPhone = await _uow.UserRepository.FirstOrDefaultAsync(u => (u.PhoneCode == phoneCode && u.Phone == phoneNumber && u.Status == StatusEnum.ACTIVE));
                if (userPhone != null) throw new PhoneInUseBadRequestException(_localizer);
            }

            var verificationCheck = await _twilioService.SendVerificationCodeAsync(phoneCode + phoneNumber);
            if (!verificationCheck) throw new SendSMSBadRequestException(_localizer);
            return await _uow.UserRepository.FirstOrDefaultAsync(u => (u.PhoneCode == phoneCode && u.Phone == phoneNumber && u.Status == StatusEnum.INACTIVE));
        }

        public PhoneNumber ValidatePhone(string phoneNumber, string phoneCode)
        {
            string phone = phoneCode + phoneNumber;
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var phoneNumber1 = phoneNumberUtil.Parse(phone, null);
            bool isValid = phoneNumberUtil.IsValidNumber(phoneNumber1);
            return isValid ? phoneNumber1 : null;
        }

        public PhoneNumber ValidatePhone(string phone)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var phoneNumber1 = phoneNumberUtil.Parse(phone, null);
            bool isValid = phoneNumberUtil.IsValidNumber(phoneNumber1);
            return isValid ? phoneNumber1 : null;
        }

        private string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("BearerTokens")["Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidTokenBadRequestException(_localizer);

            return Task.FromResult(principal);
        }

        public async Task GetRefreshTokenAsync(RefreshTokenRequest refreshToken, int userId)
        {
            var refToken = await _uow.UserTokenRepository
                .FirstOrDefaultAsync(u => u.UserId == userId && u.AccessToken == refreshToken.Token);
            if (refToken == null)
            {
                throw new RefreshTokenNotFoundException(_localizer);
            }
            if (refToken.RefreshToken != refreshToken.RefreshToken)
            {
                throw new InvalidRefreshTokenBadRequestException(_localizer);
            }
        }

        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string pAccessToken, string pRefreshToken, int userID)
        {
            var accessToken = await _uow.UserTokenRepository.FirstOrDefaultAsync(t => t.AccessToken == pAccessToken);
            var refreshToken = await _uow.UserTokenRepository.FirstOrDefaultAsync(t => t.RefreshToken == pRefreshToken);

            var token = await _uow.UserTokenRepository.FirstOrDefaultAsync(t => t.UserId == userID
                                                                            && t.AccessToken == pAccessToken
                                                                            && t.RefreshToken == pRefreshToken);
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userID);

            if (token == null && ((user != null) && (user.Status == StatusEnum.ACTIVE)))
            {
                if (accessToken != null) _uow.UserTokenRepository.Delete(accessToken);
                if (refreshToken != null) _uow.UserTokenRepository.Delete(refreshToken);
                await _uow.CommitAsync();

                throw new UnauthorizedException(_localizer);
            }
            if (token == null && user == null)
            {
                if (accessToken != null) _uow.UserTokenRepository.Delete(accessToken);
                if (refreshToken != null) _uow.UserTokenRepository.Delete(refreshToken);
                await _uow.CommitAsync();

                throw new UnauthorizedException(_localizer);
            }

            var dd = GetDeviceDetectorConfigured();
            var clientInfo = dd.GetClient();
            var osrInfo = dd.GetOs();
            var device1 = dd.GetDeviceName();
            var brand = dd.GetBrandName();
            var model = dd.GetModel();
            var claims = GetClaims(user);
            var newToken = GetToken(claims);
            var newRefreshToken = GetRefreshToken();
            var t = new UserToken();
            t.AccessToken = newToken;
            t.AccessTokenExpiresDateTime = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration.GetSection("BearerTokens")["AccessTokenExpirationHours"]));
            t.RefreshToken = newRefreshToken;
            t.RefreshTokenExpiresDateTime = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration.GetSection("BearerTokens")["RefreshTokenExpirationHours"]));
            t.UserId = user.Id;
            t.DeviceModel = model;
            t.DeviceBrand = brand;
            t.OS = osrInfo.Match?.Name;
            t.OSPlatform = osrInfo.Match?.Platform;
            t.OSVersion = osrInfo.Match?.Version;
            t.ClientName = clientInfo.Match?.Name;
            t.ClientType = clientInfo.Match?.Type;
            t.ClientVersion = clientInfo.Match?.Version;

            await _uow.UserTokenRepository.AddAsync(t);
            _uow.UserTokenRepository.Delete(token);
            await _uow.CommitAsync();

            return (newToken, newRefreshToken);
        }

        private List<Claim> GetClaims(User user)
        {
            var issuer = _configuration.GetSection("BearerTokens")["Issuer"];
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, issuer),
                new Claim(ClaimTypes.AuthenticationMethod, "bearer", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.NameIdentifier, user.FullName ?? "", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, issuer)
            };

            return claims;
        }

        private DeviceDetector GetDeviceDetectorConfigured()
        {
            var ua = _detectionService.UserAgent;

            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            var dd = new DeviceDetector(ua.ToString());
            dd.SetCache(new DictionaryCache());
            dd.DiscardBotInformation();
            dd.SkipBotDetection();
            dd.Parse();
            return dd;
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var user = await _uow.UserRepository.GetAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException(_localizer);
            }

            return user;
        }

        public async Task ChangeAccountStatusAsync(ChangeAccountStatusRequest changeAccountStatus, int userId)
        {
            var master = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);

            if (master == null)
            {
                throw new UserNotFoundException(_localizer);
            }

            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Identity == changeAccountStatus.Identity);

            if (user == null)
            {
                throw new UserNotFoundException(_localizer);
            }

            if (user.Id == userId && user.Status == StatusEnum.INACTIVE)
            {
                throw new AccountDeactivatedForbiddenException(_localizer);
            }

            if (changeAccountStatus.Active == false)
            {
                user.Status = StatusEnum.INACTIVE;
            }
            else
            {
                user.Status = StatusEnum.ACTIVE;
            }

            user.ModifiedAt = DateTime.UtcNow;

            _uow.UserRepository.Update(user);

            await _uow.CommitAsync();
        }

        public async Task<User> UploadAvatar(IFormFile file, int userId)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UserNotFoundException(_localizer);
            }

            if (file == null)
            {
                throw new FileNullBadRequestException(_localizer);
            }

            if (file.Length == 0)
            {
                throw new FileNullBadRequestException(_localizer);
            }

            var maxFileSizeInMBAllowed = _configuration.GetSection("S3")["MaxFileSizeInMegabytesAllowed"];

            if (file.Length > int.Parse(maxFileSizeInMBAllowed) * 1024 * 1024)
            {
                throw new FileInvalidSizeBadRequestException(_localizer);
            }

            var mime = file.ContentType;
            if (!mime.Equals("image/png") && !mime.Equals("image/jpg") && !mime.Equals("image/jpeg"))
            {
                throw new FileInvalidTypeBadRequestException(_localizer);
            }

            string extension = file.ContentType.Split("/")[1];
            string guid = Guid.NewGuid().ToString() + "." + extension;
            var objectS3 = await _storageService.UploadFile(file, guid, "profile");
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                var oldGuid = user.Avatar;
                await _storageService.DeleteFile(oldGuid, "profile");
            }

            user.Avatar = guid;
            user.AvatarMimeType = mime;

            await _uow.UserRepository.UpdateAsync(user, userId);
            await _uow.CommitAsync();

            return user;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest forgotPassRequest)
        {
            if (!ValidateEmail(forgotPassRequest.Email)) throw new EmailNotValidBadRequestException(_localizer);
            var user = await _uow.UserRepository.FindBy(u => u.Email.Equals(forgotPassRequest.Email) && u.Status == StatusEnum.ACTIVE).FirstOrDefaultAsync() ?? throw new UserNotFoundException(_localizer);
            var verificationCheck = await _twilioService.SendVerificationCodeAsync(user.PhoneCode + user.Phone);
            if (!verificationCheck) throw new SendSMSBadRequestException(_localizer);
            return true;
        }
    }
}