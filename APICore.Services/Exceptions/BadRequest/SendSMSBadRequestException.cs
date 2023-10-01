using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class SendSMSBadRequestException : BaseBadRequestException
    {
        public SendSMSBadRequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400016;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}