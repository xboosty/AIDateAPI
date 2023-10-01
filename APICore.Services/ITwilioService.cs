using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace APICore.Services
{
    public interface ITwilioService
    {
        public Task<VerificationCheckResource> CheckSentVerificationCode(string PhoneNumber, string TwilioCode);

        public Task<bool> SendVerificationCodeAsync(string PhoneNumber);

        public Task<MessageResource> SendSMSAsync(string ToPhoneNumber, string Message);
    }
}