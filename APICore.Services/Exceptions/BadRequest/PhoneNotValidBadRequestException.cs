using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class PhoneNotValidBadRequestException : BaseBadRequestException
    {
        public PhoneNotValidBadRequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400012;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}