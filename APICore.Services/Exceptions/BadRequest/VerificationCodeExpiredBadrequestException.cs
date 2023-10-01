using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class VerificationCodeExpiredBadrequestException : BaseBadRequestException
    {
        public VerificationCodeExpiredBadrequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400019;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}