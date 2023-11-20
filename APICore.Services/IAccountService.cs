using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Services.Utils;
using System.Security.Claims;
using System.Threading.Tasks;
using APICore.Common.DTO.Response;
using Microsoft.AspNetCore.Http;

namespace APICore.Services
{
    public interface IAccountService
    {
        Task<User> SignUpAsync(SignUpRequest suRequest);

        Task<(User user, string accessToken, string refreshToken)> LoginAsync(LoginRequest loginRequest);
        Task<(string accessToken, string refreshToken, User user)> SignUpWithFirebaseAsync(SignUpFirebaseRequest suRequest);
        Task LogoutAsync(string accessToken, int userId);

        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);

        Task GetRefreshTokenAsync(RefreshTokenRequest refreshToken, int userId);

        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string pAccessToken, string pRefreshToken, int userID);

        Task<bool> ChangeUserPasswordAsync(ChangePasswordRequest request, int userId);

        Task<bool> RecoveryUserPasswordAsync(RecoveryPasswordRequest request);

        Task<User> GetUserAsync(int userId);

        Task ChangeAccountStatusAsync(ChangeAccountStatusRequest changeAccountStatus, int userId);

        Task<User> UploadAvatar(IFormFile file, int userId);

        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest forgotPassRequest);

        Task<bool> ValidateVerificationCodeAsync(VerificationCodeRequest request);
        Task<(bool registered, string AccessToken, string RefreshToken, User user)> AuthenticateWithFirebaseAsync(string idToken);
        Task<PaginatedList<User>> GetUserList(int userId, int page, int perPage);
        Task<PaginatedList<UserWithMatch>> GetUserMatches(int userId, int page, int perPage);
        Task<User> EditProfile(int userId, List<IFormFile> pictures, EditProfileRequest request);
        Task SendVerificationEmailCode(int userId, string newEmail);
    }
}