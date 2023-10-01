using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Common.DTO.Request
{
    public class ForgotPasswordRequest
    {
        [Required]
        public PhoneDTO Phone { get; set; }
    }
}