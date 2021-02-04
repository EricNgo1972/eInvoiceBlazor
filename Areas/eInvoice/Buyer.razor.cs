using Microsoft.AspNetCore.Components;
using SPC.Cloud.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SPC.Helper.Extension;

namespace eInvoiceApp.Views
{
    public partial class Buyer
    {

        string Status = "";

        string SubscriberId = "";

        List<CompanyUserInfo> BuyersBoundToSubscriberId;

        [Parameter] public string TaxId { get; set; }

        [Parameter] public DateTime FromDate { get; set; }

        [Parameter] public DateTime ToDate { get; set; }

        public SPC.eInvoice.InvoiceInfos Invoices = null;

        /// <summary>
        /// buyer codes bound to this subscriber. Admin user have this list empty
        /// </summary>
        List<string> TaxCodes = new List<string>();

        /// <summary>
        /// Seller bound to this subscriber
        /// </summary>
        List<string> SellerCodes = new List<string>();

        List<SPC.Cloud.Members.CompanyUserInfo> entities = new List<SPC.Cloud.Members.CompanyUserInfo>();

        protected override async Task OnParametersSetAsync()
        {

            FromDate = new SPC.SmartData.SmartDate("1").Date;

            ToDate = new SPC.SmartData.SmartDate("LM").Date;

            await base.OnParametersSetAsync();

            var authState = await authenticationStateTask;

            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                SubscriberId = user.Identity.Name;

                var filter = new Dictionary<string, string>();
                filter.Add("PartitionKey", SubscriberId);
                

                BuyersBoundToSubscriberId = (from info in await SPC.Cloud.Members.CompanyUserInfoList.GetInfoListAsync(filter) where (! info.Suspended.MatchesRegExp("S|C")) select info).ToList();

                TaxCodes = (from info in BuyersBoundToSubscriberId select info.CompanyId).ToList();

                if (TaxCodes != null && TaxCodes.Count > 0)
                {
                    TaxId = TaxCodes[0];
                    await LookupCompanyNameAsync();
                }
            }

        }

        private async Task SearchAsync()
        {
            var accessibleSellers = await GetSellersAccessibleToUser();

            Status = $"Đang tìm các hóa đơn trong khoảng {FromDate.ToString("dd-MM-yyyy")}..{ToDate.ToString("dd-MM-yyyy")}";

            var qr = new SPC.eInvoice.BuyerInvoices();

            var filter = new Dictionary<string, string>();
            filter.Add("PartitionKey", TaxId);

            if (accessibleSellers.Count > 0)
            {
                filter.Add("SellerTaxId", accessibleSellers.ConvertToParameter("<<", "|"));
            }

            filter.Add("InvDate", $"<<{FromDate.ToString("yyyy-MM-dd")}..{ToDate.ToString("yyyy-MM-dd")}");

            qr.SetParameters(filter);

          var theInvoices = (await qr.GetBOListAsync()) as SPC.eInvoice.InvoiceInfos;

            if (theInvoices == null || theInvoices.Count == 0)
            {
                Status = $"Không có hóa đơn nào phát sinh trong khoảng  {FromDate.ToString("dd-MM-yyyy")}..{ToDate.ToString("dd-MM-yyyy")}";
                if (accessibleSellers.Count > 0)
                    Status = $"{Status}. (Chỉ tìm hóa đơn của các đơn vị đã tạo tài khoản tra cứu này: {accessibleSellers.ConvertToParameter()}) ";
            }
            else
            {
                if (accessibleSellers.Count > 0)
                    Status = $"Chỉ hiển thị '{theInvoices.Count}' hóa đơn của các đơn vị đã tạo tài khoản tra cứu này: {accessibleSellers.ConvertToParameter()} ";
                else
                    Status = "";

            }

            await SPC.eInvoice.InvoiceInfos.PopulateTheSellerNameAsync(theInvoices);

            Invoices = theInvoices;

    //        StateHasChanged();
        }

        private async Task<List<string>> GetSellersAccessibleToUser()
        {
            var ret = new List<string>();
            //check if user is admin
            var thecompany = (from info in BuyersBoundToSubscriberId where info.CompanyId == TaxId select info).FirstOrDefault();
            if (thecompany != null)
            {
                if (thecompany.Department.MatchesRegExp("Admin|Accounting"))
                {
                    return ret; // see all invoices
                }
                else // see only invoices of sellers in BuyerContacts
                {
                    var filter = new Dictionary<string, string>();
                    filter.Add("RowKey", SubscriberId);
                    filter.Add("BuyerId", TaxId);
                    //filter.Add("Suspend", "!");

                    var limitedSellers = (from info in await SPC.Cloud.Table.Search.SearchAsync("lavadata.BuyerContacts", filter) select info.Key.GetTextByRegExp(Str.AlphaNumericExt2));

                    ret = limitedSellers.ToList();
                }
            }
            return ret;

        }

        #region Lookup Company Name

        [Parameter] public string CompanyName { get; set; }

        private async Task LookupCompanyNameAsync()
        {
            if (!string.IsNullOrEmpty(TaxId))
            {
                var query = new SPC.eInvoice.Companies();
                query.SetParameters(new Dictionary<string, string>() { { "MST", TaxId } });

                var searchonecompany = await query.GetBOListAsync();

                if (searchonecompany.Count > 0)
                {
                    var onecom = searchonecompany[0] as SPC.Cloud.Members.RegCompanies.RegCompany;

                    CompanyName = onecom.CompanyName;
                }

            }
        }

        #endregion
    }
}
