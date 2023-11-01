using System;
using System.ComponentModel.DataAnnotations;

namespace APICore.Common.DTO.Request
{
    public class SignUpFirebaseRequest
    {
        [Required]
        public string TokenId { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        public int SexualOrientation { get; set; }
        public PhoneDTO Phone { get; set; }

    }
}