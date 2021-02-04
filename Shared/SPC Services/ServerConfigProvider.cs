using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SPC.Helper.Extension;

namespace SPC.AppServer
{
    public class ServerConfigProvider : Ctx.IAppConfigInfo
    {
        private const string ProductName = "Lava Invoice";

        public ServerConfigProvider(IConfiguration pConfig)
        {

            _config = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            try
            {
                string cs = Str.DNz(pConfig.GetConnectionString("C1"), "");

                _config.Add(Ctx.AppConfig.SQLConnectionString, cs);

                DbConnectionStringBuilder csb = new DbConnectionStringBuilder();
                csb.ConnectionString = cs;

                if (csb.TryGetValue("server", out object servername))
                    _config.Add(Ctx.AppConfig.ServerName, Str.DNz(servername, ""));


            }
            catch (Exception)
            {

            }

            _config.Add("Entity", pConfig["PhoebusSettings:Entity"]);

            _config.Add(nameof(Ctx.AppConfig.IsMobileApp), "N");

            _config.Add(nameof(Ctx.AppConfig.IsWebClient), "Y");

            _config.Add(nameof(Ctx.AppConfig.ProductName), ProductName);

            _config.Add(nameof(Ctx.AppConfig.ProductVersion), Str.DNz(pConfig["PhoebusSettings:Version"], "No Version Defined"));

            _config.Add("PackageName", ProductName);

            _config.Add(nameof(Ctx.AppConfig.UseAPIDataPortal), "N");

            _config.Add(nameof(Ctx.AppConfig.DataPortalEndPoint), "");// await SPC.Cloud.KeyMan.GetSecretAsync("API-LeaveApp"));

            _config.Add(nameof(Ctx.AppConfig.DBEngine), "SQL");

            //_config.Add(nameof(Ctx.AppConfig.SendGridAPIKey), await SPC.Cloud.KeyMan.GetSecretAsync("SendGridKey"));


        }

        private Dictionary<string, string> _config = null;

        /// <summary>
        /// Loading application info before login. We don't know about server and about user id at this moment
        /// </summary>
        /// <returns></returns>
        async Task Ctx.IAppConfigInfo.LoadAppInfosAsync()
        {

            _config.Add(nameof(Ctx.AppConfig.SendGridAPIKey), await SPC.Cloud.KeyMan.GetSecretAsync("SendGridKey"));

            //_config.Add(nameof(Ctx.AppConfig.UseTestServer), "N");
        }

        public ServerConfigProvider()
        {
        }

        Dictionary<string, string> Ctx.IAppConfigInfo.GetApplicationConfigInfos()
        {
            if (_config == null)
            {
                _config = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                _config.Add(nameof(Ctx.AppConfig.ProductName), ProductName);
                _config.Add(nameof(Ctx.AppConfig.ProductVersion), "");
                _config.Add("PackageName", ProductName);
            }

            return _config;
        }


        Task Ctx.IAppConfigInfo.SetDefaultConnectionAsync(string ConnnectionCode)
        {
            return Task.CompletedTask;

        }
    }
}