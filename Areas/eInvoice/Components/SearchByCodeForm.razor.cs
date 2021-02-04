using SPC.eInvoice;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace eInvoiceApp.Views
{

    public class SearchOneModel
    {
        public string BuyerCode { get; set; }

        public string SellerCode { get; set; }

        public Dictionary<string, List<string>> GetValidationDataNoIndex()
        {
            var ret = new Dictionary<string, List<string>>();
            ret.Add(nameof(BuyerCode), new List<string> { "Xin nhập thêm MST của người mua hoặc người bán" });
            return ret;
        }

        //public Dictionary<string, List<string>> GetValidationData()
        //{
        //    var ret = new Dictionary<string, List<string>>();
        //    if (FoundSingleInvoice == null)
        //    {
        //        return null;
        //    }
        //    if (!FoundSingleInvoice.Value && string.IsNullOrWhiteSpace(BuyerCode) && string.IsNullOrWhiteSpace(SellerCode))
        //    {
        //        ret.Add(nameof(BuyerCode), new List<string> { "Xin nhập thêm MST của người mua hoặc người bán" });
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //    return ret;
        //}

        [Required(ErrorMessage = "Xin hãy nhập mã bảo mật")]
        public string SecurityCode { get; set; }

        private bool? FoundSingleInvoice = null;

        /// <summary>
        /// return null if search without buyer or seller code return nothing
        /// </summary>
        /// <returns></returns>
        internal async Task<InvoiceInfos> SearchAsync()
        {

            // AzureInvoice:=> Index.Indexing 1846 / 1851: 1A58MN3.Create.Data = .:| Indexing 1846 / 1851: 1A58MN3: Seller: 3702670226 - Proforma: IV - CVN - 20100034
            //AzureInvoice:=> Index.Indexing 1847 / 1851: 14BCBWW.Create.Data = .:| Indexing 1847 / 1851: 14BCBWW: Seller: 3702670226 - Proforma: IV - CVN - 20100035

            InvoiceInfos ret = null;
            ret = await InvoiceInfos.GetInvoiceInfoBySecurityCodeAsync(SecurityCode);

            if (ret != null && ret.Count == 1)
            {
                FoundSingleInvoice = true;
                return ret;
            }
            else
            {
                if (!string.IsNullOrEmpty(BuyerCode))
                {
                    if (ret != null && ret.Count > 1)
                    {
                        //found more than one invoices with one security code. this is rarely, but in case it happens
                        foreach (var item in ret)
                        {
                            if (item.BuyerTaxCode == BuyerCode)
                            {
                                var buyerinvoice = new InvoiceInfos();
                                buyerinvoice.Add(item);
                                return buyerinvoice;
                            }
                        }
                    }
                    else
                        //search in the InvoiceClientIndex table. this is the old algorithm.
                        ret = await InvoiceInfos.GetBuyerInvoiceInfoBySecurityCodeAsync(BuyerCode, SecurityCode);

                    return ret;
                }
                else if (!string.IsNullOrEmpty(SellerCode))
                {
                    if (ret != null && ret.Count > 1)
                    {
                        //found more than one invoices with one security code. this is rarely, but in case it happens
                        foreach (var item in ret)
                        {
                            if (item.SellerTaxCode == SellerCode)
                            {
                                var sellerinvoice = new InvoiceInfos();
                                sellerinvoice.Add(item);
                                return sellerinvoice;
                            }
                        }
                    }
                    else
                        //search in the main table. this may takes a long time to get the result
                        ret = await InvoiceInfos.GetSellerInvoiceInfoBySecurityCodeAsync(SellerCode, SecurityCode);

                    return ret;
                }
                else
                {
                    //no data in the security index table/ no buyer code and no seller code. ask user to fill more searching info
                    return null;
                }
            }

        }

    }
}

