using APICore.Services.Exceptions.BadRequest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace APICore.Services.Impls
{
    public class TwilioService : ITwilioService
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<ITwilioService> _localizer;

        public TwilioService(IConfiguration configuration, IStringLocalizer<ITwilioService> localizer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<VerificationCheckResource> CheckSentVerificationCode(string PhoneNumber, string TwilioCode)
        {
            string serviceSid = _configuration.GetValue<string>("Twilio:VerificationServiceSid");
            VerificationCheckResource verification;
            try
            {
                verification = await VerificationCheckResource.CreateAsync(
                    to: PhoneNumber,
                    code: TwilioCode,
                    pathServiceSid: serviceSid
                    );
            }
            catch (Exception)
            {
                throw new VerificationCodeDoesntMatchBadrequestException(_localizer);
            }

            return verification;
        }

        public async Task<bool> SendVerificationCodeAsync(string PhoneNumber)
        {
            TwilioClient.Init(_configuration.GetValue<string>("Twilio:AccountSid"), _configuration.GetValue<string>("Twilio:AuthToken"));
            string serviceSid = _configuration.GetValue<string>("Twilio:VerificationServiceSid");

            try
            {
                await VerificationResource.CreateAsync(
                to: PhoneNumber,
                channel: "sms",
                pathServiceSid: serviceSid
                );
            }
            catch (Exception e)
            {
                throw new TwilioBadRequestException(_localizer);
            }

            return true;
        }

        public async Task<MessageResource> SendSMSAsync(string ToPhoneNumber, string Message)
        {
            string PhoneNumber = _configuration.GetValue<string>("Twilio:PhoneNumber");

            return await MessageResource.CreateAsync(
                   body: Message,
                   from: new Twilio.Types.PhoneNumber(PhoneNumber),
                   to: new Twilio.Types.PhoneNumber(ToPhoneNumber)
                   );
        }
    }
}