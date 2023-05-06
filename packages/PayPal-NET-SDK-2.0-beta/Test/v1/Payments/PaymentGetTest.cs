using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using static PayPal.Test.TestHarness;

namespace PayPal.v1.Payments.Test
{
    [Collection("Payments")]
    public class PaymentGetTest
    {

        [Fact]
        public async void TestPaymentGetRequest()
        {
            var paymentResponse = await PaymentCreateTest.CreatePayment("sale");
            PaymentGetRequest request = new PaymentGetRequest(paymentResponse.Result<Payment>().Id);

            HttpResponse response = await TestHarness.client().Execute(request);
            Assert.Equal(200, (int) response.StatusCode);
            Assert.NotNull(response.Result<Payment>());
        }
    }
}
