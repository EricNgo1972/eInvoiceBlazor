using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eInvoiceApp.Account
{
   
    public class ChangePasswordModel
    {
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập email đăng nhập")]
        public string Email { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập mật khẩu cũ")]
        public string OldPassword { get; set; } = "";
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập mật khẩu tài khoản người mua")]
        public string NewPassword { get; set; } = "";
       
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Nhập lại mật khẩu")]
        public string ReTypePassword { get; set; } = "";

        internal Dictionary<string, List<string>> GetValidationData()
        {
            var ret = new Dictionary<string, List<string>>();

            if ( NewPassword != ReTypePassword )
            {
                ret.Add(nameof(ReTypePassword), new List<string> { "Mật khẩu nhập lại không khớp." });
            }
            else
            {
                return null;
            }

            return ret;
        }

        
    }
}
