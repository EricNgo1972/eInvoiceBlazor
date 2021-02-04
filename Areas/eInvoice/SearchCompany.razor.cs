using SPC.Cloud.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eInvoiceApp.Views
{
    public partial class SearchCompany
    {
        //[CascadingParameter]
        //public MainLayout Layout { get; set; }

        bool? IsSearching { get; set; }

        string TaxId { get; set; }

        RegCompanies.RegCompany SPCRegCompany { get; set; }

        async Task SearchAsync()
        {

            //if (! await SPC.Services.UI.ConfirmService.ConfirmAsync("Search for company?"))
            //{
            //    SPC.Services.UI.AlertService.Alert("Search cancelled");
            //    return;
            //}

            if (!string.IsNullOrWhiteSpace(TaxId))
            {

                IsSearching = true;

                // StateHasChanged();

                var _regCompanies = new SPC.Cloud.Members.RegCompanies();

                _regCompanies.SetParameters(new Dictionary<string, string>() { { "RowKey", TaxId.Trim() } });

                var ret = await _regCompanies.GetBOListAsync();


                if (ret != null && ret.Count > 0)
                {
                    IsSearching = false;
                    //found company. it is registered with SPC
                    SPCRegCompany = ret[0] as RegCompanies.RegCompany;
                }
                else
                {
                    //unregistered companies


                    var _companies = new SPC.eInvoice.Companies();

                    _companies.SetParameters(new Dictionary<string, string>() { { "PartitionKey", "TTCT" }, { "RowKey", TaxId.Trim() } });

                    ret = await _companies.GetBOListAsync();

                    if (ret != null && ret.Count > 0)
                    {
                        IsSearching = false;
                        //found company. it is registered with SPC
                        SPCRegCompany = ret[0] as RegCompanies.RegCompany;
                    }
                    else
                        SPCRegCompany = null;



                }

                IsSearching = false;

            }
        }


    }
}
