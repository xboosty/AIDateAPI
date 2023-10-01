using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Common.DTO.Request
{
    public class VerificationCodeRequest
    {
        public PhoneDTO Phone { get; set; }
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }
    }
}