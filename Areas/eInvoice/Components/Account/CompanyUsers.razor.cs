using DevExpress.Blazor;

using SPC;
using SPC.Cloud.Members;
using SPC.Helper.Extension;
using SPC.Services.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eInvoiceApp.Areas.Account
{
    public partial class CompanyUsers
    {
        bool IsConsultant { get; set; }
        bool CanAccess { get; set; }

        protected async override Task OnParametersSetAsync()
        {

            await base.OnParametersSetAsync();

            var authState = await authenticationStateTask;

            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {

                IsConsultant = user.GetClaim("IsConsultant").ToBoolean() && user.GetClaim("RegCompany") == "SPC";

                CanAccess = IsConsultant || (user.GetClaim("Department") == "Admin");


                var filter = new Dictionary<string, string>();
                filter.Add("PartitionKey", user.Identity.Name);

                var theList = await CompanyUserInfoList.GetInfoListAsync(filter);

                TaxCodes = (from info in theList where info.Suspended == "" select info.CompanyId).ToList();

                if (TaxCodes != null && TaxCodes.Count > 0)
                {
                    TaxId = TaxCodes[0];
                }

            }


        }

        List<string> TaxCodes = new List<string>();

        string Status { get; set; }

        bool? IsSearching { get; set; }

        RegCompanies.RegCompany SPCRegCompany { get; set; }

        CompanyUserInfoList Users { get; set; }

        async Task SearchAsync()
        {


            IsSearching = true;

            StateHasChanged();

            if (string.IsNullOrWhiteSpace(TaxId) && !string.IsNullOrWhiteSpace(TaxText))
            {
                TaxId = TaxText;
            }

            if (!string.IsNullOrWhiteSpace(TaxId))
            {
                await SearchCompanyInfoAsync();

                Users = await CompanyUserInfoList.GetUsersOfACompanyAsync(TaxId);

            }


            Status = "";

            IsSearching = false;

            StateHasChanged();

        }

        private async Task SearchCompanyInfoAsync()
        {

            // StateHasChanged();

            var _regCompanies = new SPC.Cloud.Members.RegCompanies();

            _regCompanies.SetParameters(new Dictionary<string, string>() { { "RowKey", TaxId.Trim() } });

            var ret = await _regCompanies.GetBOListAsync();

            if (ret != null && ret.Count > 0)
            {
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
                    //found company. it is registered with SPC
                    SPCRegCompany = ret[0] as RegCompanies.RegCompany;
                }
                else
                    SPCRegCompany = null;
            }
        }


        #region Actions

        async Task OnRowRemoving(CompanyUserInfo dataItem)
        {
            if (dataItem != null)
            {

                //Status = $"Đang xóa tài khoản {dataItem.UserId} ...";
                //StateHasChanged();

                string ret = await SPC.Cloud.Members.CompanyUser.DeleteCompanyUserAsync(dataItem.UserId, dataItem.CompanyId);
                if (!string.IsNullOrEmpty(ret))
                    await SearchAsync();
            }
            //await ForecastService.Remove(dataItem);
        }
        async Task OnRowUpdating(CompanyUserInfo dataItem, IDictionary<string, object> newValue)
        {
            if (dataItem != null && newValue != null && newValue.Count > 0)
            {
                Status = $"Cập nhật tài khoản {dataItem.UserId}";

                StateHasChanged();

                var itm = await SPC.Cloud.Members.CompanyUser.GetCompanyUserAsync(dataItem.UserId, dataItem.CompanyId);

                Dictionary<string, string> dic = CreateStringDic(newValue);

                BOFactory.ApplyPreset(itm, dic);

                await itm.SaveAndMergeAsync();

                await CompanyUser.CreateSubscriberIfNotExistsAsync(itm.CompanyId, itm.UserId, dic);

                await SearchAsync();

            }
            //await ForecastService.Update(dataItem, newValue);
        }
        async Task OnRowInserting(IDictionary<string, object> newValue)
        {
            if (newValue != null && newValue.Count > 0)
            {

                Dictionary<string, string> dic = CreateStringDic(newValue);

                var pUserId = dic.GetValueByKey(nameof(CompanyUser.UserId), "");

                Status = $"Thêm tài khoản {pUserId}";

                StateHasChanged();


                foreach (var item in Users)
                {
                    if (item.UserId == pUserId)
                    {
                        AlertService.Alert($"Tài khoản {pUserId} đã tồn tại");
                        return;
                    }
                }

                await CompanyUser.RegisterANewUserAsync(TaxId, dic);

                await SearchAsync();
            }
        }

        private static Dictionary<string, string> CreateStringDic(IDictionary<string, object> newValue)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in newValue)
            {
                if (!dic.ContainsKey(item.Key))
                {
                    dic.Add(item.Key, Str.DNz(item.Value, ""));
                }
            }

            return dic;
        }

        Task OnInitNewRow(Dictionary<string, object> values)
        {
            values.Add(nameof(CompanyUserInfo.CompanyId), TaxId);
            values.Add(nameof(CompanyUserInfo.UserId), "");
            values.Add(nameof(CompanyUserInfo.Name), "");

            return Task.CompletedTask;
        }

        async Task ResendPasswordAsync(ToolbarItemClickEventArgs e)
        {
            if (SelectedRow != null)
            {

                var cmd = new CmdArg($"SPC.Cloud.Commands.Subscriber.ResetPassword?SubscriberId={SelectedRow.UserId}");

                await SPC.Services.UI.RunURLService.RunAsync(cmd);

                bs.Toast($"Đã gửi mật khẩu mới cho user {SelectedRow.UserId}");

            }
        }

        #endregion


    }
}
