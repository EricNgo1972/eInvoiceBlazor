using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using SPC.Helper;
using System.ComponentModel.DataAnnotations;
using Csla.Security;
using SPC.Cloud.Members;
using System;
using System.Collections.Generic;
using SPC.Helper.Extension;

namespace eInvoiceApp.Account.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ErrorText { get; set; }

        [BindProperty]
        public string Caller { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorText = "Form has validation errors.";
                return Page();
            }


            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                //var userInfos = await CloudLoginAsync(Username.Trim(), Password.Trim());
                var principal = await SPC.Security.AzureLogin.LoginUsingClaimIdentity(Username.Trim(), Password.Trim());

                if (principal != null && principal.Identity.IsAuthenticated)
                {
                    var theUserClaims = new CslaClaimsPrincipal(principal);

                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, theUserClaims, authProperties);

                    var ctx = new Csla.Blazor.ApplicationContextManager();

                    ctx.SetUser(theUserClaims);

                    Csla.ApplicationContext.User = theUserClaims;

                  // CreateMappingUserTaxCodeIfNeededAsync(Username,true).TaskFireAndForget();

                    return LocalRedirect(Url.Content($"~/{Caller}"));
                }
                else
                {
                    Password = "";

                    ErrorText = "Cannot login with provided user name and password".Translate();
                    return Page();
                }

            }

            ErrorText = "Invalid credentials";

            return Page();
        }

       

        //private async static Task<SPC.UsrMan.IUserInfo> CloudLoginAsync(string pUser, string pPassword)
        //{

        //    Ctx.DeleteOldUserContext();

        //    Csla.ApplicationContext.User = new UnauthenticatedPrincipal();

        //    await SPC.Ctx.AppConfig.LoadAppInfosAsync();

        //    var loginInfo = await SPC.Security.AzureLogin.LoginWithSubscriberId(pUser, pPassword);

        //    //if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
        //    if (loginInfo != null)
        //    {
        //        DeviceRepositoryService.SaveToRepositoryAsync("username", pUser).TaskFireAndForget();
        //        DeviceRepositoryService.SaveToRepositoryAsync("password", pPassword).TaskFireAndForget();
        //    }
        //    else
        //        AlertService.Alert("Sorry, You enter a wrong user Id and /or password".Translate());

        //    return loginInfo;

        //}
    }
}