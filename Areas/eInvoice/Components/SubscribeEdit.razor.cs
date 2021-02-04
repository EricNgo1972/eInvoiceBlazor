using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SPC.Helper.Extension;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eInvoiceApp.Views
{
    public partial class SubscribeEdit
    {

        [Parameter]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập mã số thuế người mua")]
        public string TaxCode { get; set; } = "";

        [Parameter]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Là mã số tra cứu hóa đơn khi lần đầu đăng ký tài khoản người mua")]
        public string SecurityCode { get; set; } = "";


        [Parameter]
        public string Emails { get; set; } = "";

        private SPC.eInvoice.InvoiceInfo oneInvoice;

        protected async override Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();

            TaxCode = Str.Nz(TaxCode, "");

            var state = await auth.GetAuthenticationStateAsync();

        }

        private async Task CheckSecurityAsync()
        {
            try
            {


                if (!string.IsNullOrEmpty(TaxCode) && !string.IsNullOrEmpty(SecurityCode))
                {
                    var ret = await SPC.eInvoice.InvoiceInfos.GetInvoiceInfoBySecurityCodeAsync(SecurityCode);
                    if (ret == null || ret.Count == 0)
                    {
                        ret = await SPC.eInvoice.InvoiceInfos.GetBuyerInvoiceInfoBySecurityCodeAsync(TaxCode, SecurityCode);
                    }

                    if (ret != null && ret.Count > 0)
                    {

                        oneInvoice = ret[0];

                        Emails = Str.Nz(oneInvoice.Notes.Trim(), "-");

                    }
                    else
                    {
                        Emails = "!";
                    }

                }

            }
            catch (Exception ex)
            {
                await JsRuntime.InvokeVoidAsync("confirm", ex.Message);
            }

        }


        private bool IsCreatingAccount;

        private async Task CreateAccountAsync()
        {
            try
            {
                if (IsCreatingAccount)
                {
                    bs.Toast($"Đang tạo tài khoản: {Emails} cho công ty {TaxCode}.".Trim());
                }
                else
                {
                    IsCreatingAccount = true;

                    var users = Emails.Split(new char[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                    var added = new List<string>();

                    foreach (string item in users)
                    {
                        string sendpassMsg = "";
                        if (!await SPC.Cloud.Members.Subscriber.ExistsAsync(item))
                        {
                            var subs = await SPC.Cloud.Members.Subscriber.NewSubscriberAsync(item);

                            subs.RegCompany = TaxCode;

                            subs.Name = $"{subs.SubscriberId} ({TaxCode})";

                            subs = await subs.SaveBOAsync() as SPC.Cloud.Members.Subscriber;

                            await subs.SendPasswordAsync();

                            sendpassMsg = $"Đã tạo tài khoản đăng nhập và gửi mật khẩu đến {item}.";
                        }

                        var mapping = await SPC.Cloud.Members.CompanyUser.NewCompanyUserAsync(item, TaxCode);

                        await mapping.SaveBOAsync();

                        bs.Toast($"{sendpassMsg} Đã liên kết tài khoản: {item} với công ty {TaxCode}.".Trim());

                    }

                }

            }
            catch (Exception ex)
            {
                bs.ConfirmYes("Error", ex.Message, null);

            }
            finally
            {
                IsCreatingAccount = false;
            }
        }
    }
}
