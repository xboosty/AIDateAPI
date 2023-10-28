using APICore.BasicResponses;
using APICore.Utils;
using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace APICore.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public AccountController(IAccountService accountService, IMapper mapper, IEmailService emailService, IWebHostEnvironment env)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Register a user.
        /// </summary>
        /// <param name="register">
        /// Register request object. Include email used as username, password, full name and
        /// birthday. Valid password should have: 1- Non alphanumeric characters 2- Uppercase
        /// letters 3- Six characters minimun
        /// Sexual Orientation - 0 (Hetero), 1(Bisexual), 2(Homosexual), 3(Transexual)
        /// </param>
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] SignUpRequest register)
        {
            var user = await _accountService.SignUpAsync(register);
            var mapped = _mapper.Map<UserResponse>(user);
            return Ok(new ApiOkResponse(mapped));
        }

        /// <summary>
        /// Login a user.
        /// </summary>
        /// <param name="loginRequest">
        /// Login request object. Include email used as username and password.
        /// </param>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _accountService.LoginAsync(loginRequest);
            HttpContext.Response.Headers["Authorization"] = "Bearer " + result.accessToken;
            HttpContext.Response.Headers["RefreshToken"] = result.refreshToken;
            var user = _mapper.Map<UserResponse>(result.user);
            return Ok(new ApiOkResponse(user));
        }

        /// <summary>
        /// Logout a user. Requires authentication.
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Logout()
        {
            var loggedUser = User.GetUserIdFromToken();
            await _accountService.LogoutAsync(Request.Headers["Authorization"], loggedUser);
            return Ok();
        }

        /// <summary>
        /// Recovery user password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("recovery-password")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> RecoveryUserPassword([FromBody] RecoveryPasswordRequest request)
        {
            var result = await _accountService.RecoveryUserPasswordAsync(request);
            return Ok(new ApiOkResponse(result));
        }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <param name="refreshToken">
        /// Refresh token request object. Include old token and refresh token. This info will be
        /// used to validate the info against our database.
        /// </param>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshToken)
        {
            var principal = await _accountService.GetPrincipalFromExpiredTokenAsync(refreshToken.Token);
            var loggedUser = principal.GetUserIdFromToken();

            await _accountService.GetRefreshTokenAsync(refreshToken, loggedUser);
            var result = await _accountService.RefreshTokenAsync(refreshToken.Token, refreshToken.RefreshToken, User.GetUserIdFromToken());

            HttpContext.Response.Headers["Authorization"] = "Bearer " + result.accessToken;
            HttpContext.Response.Headers["RefreshToken"] = result.refreshToken;
            HttpContext.Response.Headers["Access-Control-Expose-Headers"] = "Authorization, RefreshToken";

            return Ok();
        }

        /// <summary>
        /// Change Password.
        /// </summary>
        /// <param name="changePassword">
        /// Change password request object. Include old password, new password, and confirm password.
        /// </param>
        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePassword)
        {
            var loggedUser = User.GetUserIdFromToken();
            await _accountService.ChangeUserPasswordAsync(changePassword, loggedUser);
            return Ok();
        }

        /// <summary>
        /// Change Account Status. Requires authentication.
        /// </summary>
        /// <param name="changeAccountStatus">
        /// Change account status request object. Include identity and status to change.
        /// </param>
        [Authorize]
        [HttpPost("change-status")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeAccountStatus([FromBody] ChangeAccountStatusRequest changeAccountStatus)
        {
            var loggedUser = User.GetUserIdFromToken();
            await _accountService.ChangeAccountStatusAsync(changeAccountStatus, loggedUser);
            return Ok();
        }

        /// <summary>
        /// Validate verification code sent by Email or Phone.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify-code")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> VerifyUserCode([FromBody] VerificationCodeRequest request)
        {
            var result = await _accountService.ValidateVerificationCodeAsync(request);
            return Ok(new ApiOkResponse(result));
        }

        /// <summary>
        /// Upload Avatar. Requires authentication.
        /// </summary>
        /// <param name="file">Avatar file.</param>
        [Authorize]
        [HttpPost("upload-avatar")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var loggedUser = User.GetUserIdFromToken();
            var result = await _accountService.UploadAvatar(file, loggedUser);
            var user = _mapper.Map<UserResponse>(result);
            return Ok(new ApiOkResponse(user));
        }

        /// <summary>
        /// Forgot password endpoint. The user receive an verification code to change the password.
        /// </summary>
        /// <param name="forgotPassRequest"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPassRequest)
        {
            var newPass = await _accountService.ForgotPasswordAsync(forgotPassRequest);
            return Ok(new ApiOkResponse(newPass));
        }
        /// <summary>
        /// Firebase authentication endpoint. 
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        [HttpGet("firebase")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> VerificationAuthenticationFirebase([Required] string idToken)
        {
            var user = await  _accountService.AuthenticateWithFirebaseAsync(idToken);
            var userResponse = _mapper.Map<UserResponse>(user);
            return Ok(new ApiOkResponse(userResponse));
        }

        [HttpGet("users-list")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserList(int? page, int? perPage)
        {
            int pag = page ?? 1;
            int perPag = perPage ?? 10;
            var users = await _accountService.GetUserList(pag, perPag);
            var userResponse = _mapper.Map<List<UserResponse>>(users.ToList());
            Response.AddPagingHeaders(users.GetPaginationData);
            return Ok(new ApiOkResponse(userResponse));
        }

        [Authorize]
        [HttpPost("edit-profile")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditProfile([FromForm]List<IFormFile> pictures, EditProfileRequest request)
        {
            var loggedUser = User.GetUserIdFromToken();
            var result = await _accountService.EditProfile(loggedUser ,pictures, request);
            var user = _mapper.Map<UserResponse>(result);
            return Ok(new ApiOkResponse(user));
        }

    }
}