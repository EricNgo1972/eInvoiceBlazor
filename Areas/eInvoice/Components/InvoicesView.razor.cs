using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;



namespace eInvoiceApp.Views
{
    public  partial class InvoicesView
    {
        [Inject]
        IJSRuntime jsRuntime { get; set; }

        async Task DownloadPdf(object pContext)
        {
            SPC.eInvoice.InvoiceInfo info = pContext as SPC.eInvoice.InvoiceInfo;

            if (info != null)
            {
                var key = SPC.Helper.Extension.SPCHash.CRCToBase36(String.Format("{0}:{1}", info.SellerTaxCode, info.ProformaNo));
                var thePdfUrl = await SPC.eInvoice.OneInvoice.GetPdfUrlAsync(info.SellerTaxCode, info.ProformaNo, key);

                if (!string.IsNullOrWhiteSpace(thePdfUrl))
                {
                    //navi.NavigateTo(thePdfUrl, true);
                    await jsRuntime.InvokeAsync<object>("open", thePdfUrl, "_blank");
                }

            }
        }

        async Task DownloadXml(object pContext)
        {
            SPC.eInvoice.InvoiceInfo info = pContext as SPC.eInvoice.InvoiceInfo;

            if (info != null)
            {
                var key = SPC.Helper.Extension.SPCHash.CRCToBase36(String.Format("{0}:{1}", info.SellerTaxCode, info.ProformaNo));
                var theXmlUrl = await SPC.eInvoice.OneInvoice.GetXmlUrlAsync(info.SellerTaxCode, info.ProformaNo, key);


                if (!string.IsNullOrWhiteSpace(theXmlUrl))
                {
                    await SPC.Cloud.Blob.Storage.SetDownloadContentTypeForBlobAsync(theXmlUrl);

                    await jsRuntime.InvokeAsync<object>("open", theXmlUrl, "_blank");
                }

            }
        }

    }
}
