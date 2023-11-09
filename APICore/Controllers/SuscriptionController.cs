using APICore.BasicResponses;
using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APICore.Controllers
{
    [Route("api/subscription")]
    public class SuscriptionController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly IMapper _mapper;

        public SuscriptionController(IStripeService stripeService, IMapper mapper)
        {
            _stripeService = stripeService ?? throw new ArgumentNullException(nameof(Stripe));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("create-customer")]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateCustomer([FromBody] string name)
        {
            var customer = await _stripeService.CreateCustomerAsync(name);
            return Ok(new ApiOkResponse(customer));
        }

        [HttpPost("create-subscription-product")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProductSubscription([FromBody] int amount, string interval, string productName = "Basic")
        {
            var product = await _stripeService.CreateSubscriptionProduct(productName, amount, interval); 
            return Ok(new ApiOkResponse(product));
        }

        [HttpPost("subscribe-customer")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SubscribeCustomer([FromBody] string customerEmail, string priceId)
        {
            var subscription = await _stripeService.SusbcribeCustomer(priceId, customerEmail);
            return Ok(new ApiOkResponse(subscription));
        }

    }
}