using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APICore.Services
{
    public interface IAccountService
    {
        Task SignUpAsync(SignUpRequest suRequest);

        Task<(User user, string accessToken, string refreshToken)> LoginAsync(LoginRequest loginRequest);

        Task LogoutAsync(string accessToken, int userId);

        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);

        Task GetRefreshTokenAsync(RefreshTokenRequest refreshToken, int userId);

        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string pAccessToken, string pRefreshToken, int userID);

        Task<bool> ChangeUserPasswordAsync(ChangePasswordRequest request, int userId);

        Task<User> GetUserAsync(int userId);

        Task ChangeAccountStatusAsync(ChangeAccountStatusRequest changeAccountStatus, int userId);

        Task<User> UploadAvatar(IFormFile file, int userId);

        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest forgotPassRequest);

        Task<bool> ValidateVerificationCodeAsync(VerificationCodeRequest request);
    }
}