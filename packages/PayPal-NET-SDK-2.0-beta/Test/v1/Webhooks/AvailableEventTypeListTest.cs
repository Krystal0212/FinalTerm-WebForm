using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using static PayPal.Test.TestHarness;


namespace PayPal.v1.Webhooks.Test
{
    [Collection("Webhooks")]
    public class AvailableEventTypeListTest
    {

        [Fact]
        public async void TestAvailableEventTypeListRequest()
        {
            AvailableEventTypeListRequest request = new AvailableEventTypeListRequest();

            HttpResponse response = await TestHarness.client().Execute(request);
            Assert.Equal(200, (int) response.StatusCode);
            Assert.NotNull(response.Result<EventTypeList>());

            // Add your own checks here
        }
    }
}
