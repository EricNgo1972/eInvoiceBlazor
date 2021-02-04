using BlazorPlus;
using SPC.Services.UI;
using System;


namespace eInvoiceApp.Services
{
    public class AlertServiceImp : SPC.Services.UI.IAlert
    {
        
        void IAlert.Alert(string pFormatString, params object[] ParamArray)
        {
            BlazorSession.Current.Alert("Info", string.Format(pFormatString, ParamArray));
        }

        void IAlert.ShowError(Exception ex)
        {
            BlazorSession.Current.ConsoleError("Error", ex.ToString());
            BlazorSession.Current.Alert("Error", ex.Message);
        }

        void IAlert.Toast(string pFormatString, params object[] ParamArray)
        {
            try
            {
                BlazorSession.Current.Toast(string.Format(pFormatString, ParamArray), 5);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }


        }
    }
}
