using Microsoft.Extensions.Localization;

namespace APICore.Services.Exceptions
{
    public class BlockedUserNotFoundException : BaseNotFoundException
    {
        public BlockedUserNotFoundException(IStringLocalizer<object> localizer) : base()
        {
            CustomCode = 404004;
            CustomMessage = localizer.GetString(CustomCode.ToString());
        }
    }
}