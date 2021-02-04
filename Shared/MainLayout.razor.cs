

using BlazorPlus;
using eInvoiceApp.Services;
using SPC.Services.UI;
using System;
using System.Threading.Tasks;

namespace eInvoiceApp.Shared
{
    public partial class MainLayout  : IAlert  , SPC.Services.UI.IConfirm
    {

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var usr = Csla.ApplicationContext.User;

        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SPC.Services.UI.AlertService.RegisterUIService(this);
            SPC.Services.UI.ConfirmService.RegisterUIService(this);
            
        }

        #region Alert

        void IAlert.Alert(string pFormatString, params object[] ParamArray)
        {
            bls.Alert("Info", string.Format(pFormatString, ParamArray));
        }

        void IAlert.ShowError(Exception ex)
        {
            bls.ConsoleError("Error", ex.ToString());
            bls.Alert("Error", ex.Message);
        }

        void IAlert.Toast(string pFormatString, params object[] ParamArray)
        {
            try
            {
                bls.Toast(string.Format(pFormatString, ParamArray), 15);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }


        }

        #endregion

        #region Confirm Service

        async Task<bool> IConfirm.ConfirmAsync(string pFormatString, params object[] ParamArray)
        {
            var option = new UIDialogOption() { Title = "Confirmation", Message = string.Format(pFormatString, ParamArray) };

            var ret = await bls.ConfirmAsync(option);

            return (ret == true);

        }

        void IConfirm.ShowError(Exception ex)
        {
            bls.ConfirmYes("Error", ex.Message, null);
        }

        #endregion
    }
}
