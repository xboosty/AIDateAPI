using Microsoft.Extensions.Localization;

namespace APICore.Services.Exceptions
{
    public class MessageNotFoundException : BaseNotFoundException
    {
        public MessageNotFoundException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 404005;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}