using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using APICore.Services.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Stripe;
using Stripe.FinancialConnections;

namespace APICore.Services.Impls
{
    public class StripeService : IStripeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly IOptions<StripeOptions> options;

        public StripeService(IUnitOfWork uow, IConfiguration configuration, IOptions<StripeOptions> options)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.options = options;
            StripeConfiguration.ApiKey = options.Value.SecretKey;
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<Customer> CreateCustomerAsync(string email)
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
            };
            var service = new CustomerService();
            var customer = service.Create(options);
            return customer;
        }

        public async Task<Product> CreateSubscriptionProduct(string name,int amount, string interval)
        {
            var productOptions = new ProductCreateOptions { Name = name };
            var productService = new ProductService();
            var product = await  productService.CreateAsync(productOptions);

            var priceOptions = new PriceCreateOptions
            {
                Product = product.Id,
                UnitAmount = amount,
                Currency = "usd",
                Recurring = new PriceRecurringOptions { Interval = interval },
            };
            var priceService = new PriceService();
            var price = await  priceService.CreateAsync(priceOptions);
            
            return product;
            }

public async Task<Subscription> SusbcribeCustomer(string priceId, string customerEmail)
        {
            var customerService = new CustomerService();
  var customer = (await customerService.SearchAsync(new CustomerSearchOptions{Query = $"email:'{customerEmail}'" })).First();
            var paymentOptions = new PaymentMethodAttachOptions
            {
                Customer = customer.Id,
            };
            var paymentMethodService = new PaymentMethodService();
            var paymentMethod = paymentMethodService.Attach("card", paymentOptions);
            var customerOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethod.Id,
                },
            };
          await   customerService.UpdateAsync(customer.Id, customerOptions);

            var options = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
    {
        new SubscriptionItemOptions { Price = priceId },
    },
            };
            var service = new SubscriptionService();
            var subscription = service.Create(options);

            return subscription;
        }
        }
}
