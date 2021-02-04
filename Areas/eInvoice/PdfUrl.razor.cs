using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System; 
using System.Threading.Tasks;

namespace eInvoiceApp.Views
{
    public partial class PdfUrl
    {

        [Inject]
        NavigationManager navi { get; set; }
        
        [Inject]
        IJSRuntime jsRuntime { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                var uri = navi.ToAbsoluteUri(navi.Uri);

                string _proformaNo = "";
                string _seller = "";
                string _securityCode = "";

                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("ProformaNo", out var ProformaNo))
                {
                    _proformaNo = ProformaNo.ToString();
                }

                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Seller", out var Seller))
                {
                    _seller = Seller.ToString();
                }

                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("SecurityCode", out var SecurityCode))
                {
                    _securityCode = SecurityCode.ToString();
                }

                var key = SPC.Helper.Extension.SPCHash.CRCToBase36(String.Format("{0}:{1}", _seller.Trim(), _proformaNo.Trim()));

                var thePdfUrl = await SPC.eInvoice.OneInvoice.GetPdfUrlAsync(_seller, _proformaNo, key);

                if (!string.IsNullOrWhiteSpace(thePdfUrl))
                {
                    navi.NavigateTo(thePdfUrl, true);
                    //await jsRuntime.InvokeAsync<object>("open", thePdfUrl, "_blank");
                }
            }
            catch (Exception)
            {
            }


        }
    }
}
