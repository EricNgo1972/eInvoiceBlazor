using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPC.Helper;
using SPC.Helper.Extension;

namespace eInvoiceApp.Views
{


    public partial class SellerSecurityCode
    {
        string Seller { get; set; } = "";

        string SecurityCode { get; set; } = "";

        string Period { get; set; } = new SPC.SmartData.SmartPeriod("T").Text;

        SPC.eInvoice.InvoiceInfos SearchResult { get; set; } = null;

        string Status { get; set; } = "";

        bool IsSearching = false;

        async Task SearchAsync()
        {
            try
            {
                var thePeriod = new SPC.SmartData.SmartPeriod(Period).Text;
                if (!IsSearching)
                {
                    IsSearching = true;

                    if (string.IsNullOrEmpty(Seller))
                    {
                        Status = "Xin nhập mã số thuế người bán";
                    }
                    else if (string.IsNullOrEmpty(SecurityCode))
                    {
                        Status = "Xin nhập mã kiểm soát";
                    }
                    else if (!SecurityCode.Trim().Equals(SPCHash.CRCToBase36(Seller)))
                    {
                        //LBFFXI
                        Status = "Mã kiểm soát không hợp lệ. Xin liên hệ với đơn vị cung cấp dịch vụ để lấy mã kiểm soát";
                    }
                    else
                    {
                        Status = $"Trích xuất hóa đơn trong kỳ {thePeriod} ...";

                        SearchResult = await SPC.eInvoice.InvoiceInfos.GetSellerInvoiceInfosAsync(Seller, thePeriod);

                        Status = "";
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsSearching = false;
            }
        }
    }


}

 
