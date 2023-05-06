using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using System.Threading.Tasks;

namespace PayPal.v1.Webhooks.Test
{
    [Collection("Webhooks")]
    public class AvailableEventTypesTest
    {
        public static async Task<HttpResponse> getAvailableEventTypes() {
            AvailableEventTypeListRequest request = new AvailableEventTypeListRequest();
            return await TestHarness.client().Execute(request);
        }

        [Fact]
        public async void TestAvailableEventTypeListRequest()
        {
            HttpResponse response = await getAvailableEventTypes();
            Assert.Equal(200, (int) response.StatusCode);
            Assert.NotNull(response.Result<EventTypeList>());
        }
    }
}
