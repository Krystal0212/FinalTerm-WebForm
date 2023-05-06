using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;
using static PayPal.Test.TestHarness;


namespace PayPal.v1.CustomerDisputes.Test
{
    [Collection("Customer Disputes")]
    public class DisputeAdjudicateTest
    {
        [Fact(Skip = "This test requires a dispute in the correct state.")]
        public async void TestDisputeAdjudicateRequest()
        {
            AdjudicateRequest adjudicateRequest = new AdjudicateRequest() {
                AdjudicationOutcome = "BUYER_FAVOR"
            };
            DisputeAdjudicateRequest request = new DisputeAdjudicateRequest("PP-000-042-635-209");
            request.RequestBody(adjudicateRequest);

            HttpResponse response = await TestHarness.client().Execute(request);
            Assert.Equal((int) response.StatusCode, 200);
            Assert.NotNull(response.Result<SubsequentActions>());
        }
    }
}
