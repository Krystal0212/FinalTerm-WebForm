using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using static PayPal.Test.TestHarness;
using static PayPal.v1.Payments.Test.PaymentCreateTest;

namespace PayPal.v1.Payments.Test
{
    [Collection("Payments")]
    public class AuthorizationReauthorizeTest
    {
        private Authorization buildRequestBody()
        {
            return new Authorization()
            {
                Amount = new Amount()
                {
                    Total = "10",
                    Currency = "USD"
                }
            };
        }

        [Fact]
        public async void TestAuthorizationReauthorizeRequest()
        {
            var paymentCreateResponse = await PaymentCreateTest.CreatePayment("authorize");
            var authId = paymentCreateResponse.Result<Payment>()
                .Transactions[0]
                .RelatedResources[0]
                .Authorization.Id;

            AuthorizationReauthorizeRequest request = new AuthorizationReauthorizeRequest(authId);

            request.RequestBody(buildRequestBody());

            try 
            {
                HttpResponse response = await TestHarness.client().Execute(request);
                Assert.False(true, "This test relies on User interaction");
            }
            catch (HttpException e)
            {
                Assert.Contains("DCC_REAUTHORIZATION_NOT_ALLOWED", e.Message);
            }
        }
    }
}
