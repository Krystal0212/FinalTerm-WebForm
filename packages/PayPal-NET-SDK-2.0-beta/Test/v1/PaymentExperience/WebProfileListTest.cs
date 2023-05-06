using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using static PayPal.Test.TestHarness;


namespace PayPal.v1.PaymentExperience.Test
{
    [Collection("Payment Experience")]
    public class WebProfileListTest
    {

        [Fact]
        public async void TestWebProfileListRequest()
        {
            WebProfileListRequest request = new WebProfileListRequest();

            HttpResponse response = await TestHarness.client().Execute(request);
            Assert.Equal(200, (int) response.StatusCode);
            Assert.NotNull(response.Result<List<WebProfile>>());

            // Add your own checks here
        }
    }
}
