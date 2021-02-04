using BlazorPlus;
using Microsoft.AspNetCore.Components;
using SPC.Services.UI;
using System;
using System.Threading.Tasks;

namespace eInvoiceApp.Services
{
    public class ConfirmServiceImp : SPC.Services.UI.IConfirm
    {
        
        async Task<bool> IConfirm.ConfirmAsync(string pFormatString, params object[] ParamArray)
        {
            var option = new UIDialogOption() { Title = "Confirmation", Message = string.Format(pFormatString, ParamArray) };

            var ret = await BlazorSession.Current.ConfirmAsync(option);

            

            return (ret == true);

        }

        void IConfirm.ShowError(Exception ex)
        {
            BlazorSession.Current.ConfirmYes("Error", ex.Message, null);
        }
    }
}
