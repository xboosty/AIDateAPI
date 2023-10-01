using System.ComponentModel.DataAnnotations;

namespace APICore.Common.DTO.Request
{
    public class ChangePasswordRequest
    {
        [Required]
        [MinLength(6)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmationPassword { get; set; }
    }
}