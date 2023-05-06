using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using BraintreeHttp;
using Xunit;
using PayPal.Test;

namespace PayPal.v1.Invoices.Test
{
    [Collection("Invoices")]
    public class InvoiceListTest
    {

        [Fact]
        public async void TestInvoiceListRequest()
        {
            HttpResponse createResponse = await InvoiceCreateTest.CreateInvoice();
            InvoiceListRequest request = new InvoiceListRequest();

            HttpResponse listResponse = await TestHarness.client().Execute(request);
            Assert.Equal(200, (int) listResponse.StatusCode);
            Assert.NotNull(listResponse.Result<InvoiceList>());
            var invoices = listResponse.Result<InvoiceList>();
            Assert.NotNull(invoices.Invoices);
        }
    }
}
