using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.Unauthorized
{
    public class IncorrectVerificationCodeUnauthorizedException : BaseUnauthorizedException
    {
        public IncorrectVerificationCodeUnauthorizedException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 401002;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}