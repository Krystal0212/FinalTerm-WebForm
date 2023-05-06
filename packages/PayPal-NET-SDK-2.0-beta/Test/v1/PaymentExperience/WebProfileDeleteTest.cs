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
    [Collection("Web Profile")]
    public class WebProfileDeleteTest
    {

        [Fact]
        public async void TestWebProfileDeleteRequest()
        {
            HttpResponse createResponse = await WebProfileCreateTest.createWebProfile();
            var expected = createResponse.Result<WebProfile>();
            WebProfileDeleteRequest request = new WebProfileDeleteRequest(expected.Id);

            HttpResponse response = await TestHarness.client().Execute(request);
            Assert.Equal(204, (int) response.StatusCode);
        }
    }
}
