using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class VerificationCodeDoesntMatchBadrequestException : BaseBadRequestException
    {
        public VerificationCodeDoesntMatchBadrequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400014;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}