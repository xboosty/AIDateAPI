using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services.Exceptions.BadRequest
{
    public class IdentityFieldBadRequestException : BaseBadRequestException
    {
        public IdentityFieldBadRequestException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 400020;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}