using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Services.Utils;
using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICore.Services
{
    public interface IStripeService
    {
        Task<Customer> CreateCustomerAsync(string email);
        Task<Product> CreateSubscriptionProduct(string name, int amount, string interval);
        Task<Subscription> SusbcribeCustomer(string priceId, string customerEmail);
    }
}
