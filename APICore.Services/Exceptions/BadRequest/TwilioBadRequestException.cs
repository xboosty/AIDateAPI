using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class TwilioBadRequestException : BaseBadRequestException
    {
        public TwilioBadRequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400015;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}