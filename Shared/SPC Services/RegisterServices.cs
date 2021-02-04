using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SPC.AppServer;
using SPC.CORE;
using SPC.Security;
using SPC.Services.Storage;
using System.Threading.Tasks;

namespace SPC

{
    public class SystemServicesRegister

    {

        public static void RegisterAll(IConfiguration Config)

        {

            SPC.BO.ModuleCheckin.CheckIn();

            //SPC.BO.LA.ModuleCheckin.CheckIn();
            //SPC.BO.HR.ModuleCheckin.CheckIn();
            //SPC.BO.DM.ModuleCheckin.CheckIn();
            //SPC.BO.PO.ModuleCheckin.CheckIn();
            //SPC.BO.RE.ModuleCheckin.CheckIn();
            //SPC.BO.PI.ModuleCheckin.CheckIn();
            //SPC.BO.PM.ModuleCheckin.CheckIn();
            //SPC.BO.CRM.ModuleCheckin.CheckIn();
            //SPC.BO.eInvoice.ModuleCheckin.CheckIn();

           // SPC.Services.LocalizationCheckIn.CheckIn();
            SPC.BO.COM.ModuleCheckin.CheckIn();
            SPC.Cloud.ModuleCheckin.CheckIn();


            //SPC.Services.UI.WaitingService.RegisterUIService(new Services.UI.WaitingPanel());

            //SPC.Services.UI.ConfirmService.RegisterUIService(new Services.UI.ConfirmPanel());

            //SPC.Services.UI.AlertService.RegisterUIService(new Services.UI.AlertPanel());

            //SPC.Services.UI.ValueSelector.RegisterUIService(new SPC.Views.Dialog.ValueSelectionService_Imp());

            //SPC.Services.UI.SurveyService.RegisterUIService(new SPC.Views.Dialog.SurveyService_Imp());

            //SPC.Services.UI.MemoEditService.RegisterUIService(new SPC.Views.Dialog.MemoEditServiceImp());

            //SPC.Services.UI.ApprovalService.RegisterUIService(new SPC.Views.Dialog.ApprovalDialogService_Imp());

            //SPC.Services.UI.ObjectEditService.RegisterUIService(new SPC.Views.Dialog.ObjectEditServiceImp());

            //SPC.Services.UI.DictionaryEditService.RegisterUIService(new SPC.Views.Dialog.DictionaryEditServiceImp());



            //SPC.Services.UI.UserInputService.RegisterUIService(new Services.UI.TextInputService());
            //  SPC.Services.UI.TableViewService.RegisterUIService(new TableService());

            SPC.Services.UI.RunURLService.RegisterUIService(new Services.RunUrlService_Imp());

            SPC.Services.FormulaService.RegisterService(new SPC.Services.FormulaServiceImp());

            SPC.Services.COM.MessageServices.RegisterService(new SPC.COM.Mail.SendingService());

            SPC.Services.Cloud.Blob.RegisterService(new SPC.Cloud.Blob.BlobService());

            SPC.Services.Cloud.Table.RegisterService(new SPC.Cloud.Table.TableService());

            SPC.Services.LogService.RegisterService(new SPC.Cloud.LogService());


            //SPC.Services.Device.MediaService.RegisterService(new SPC.Services.Device.MediaService_Imp());

            //SPC.Services.Storage.FileStorageService.RegisterService(new SPC.Services.Storage.SQLFileStreamService());

            SPC.Services.Storage.FileStorageService.RegisterService(new SPC.Services.Storage.DocLinkBlobService());

            //SPC.Services.Storage.DeviceRepositoryService.RegisterService(new SPC.Services.Storage.DeviceRepositoryImp());

            //SPC.Services.Storage.UserRepositoryService.RegisterService(new SPC.Services.Storage.UserRepositoryImp());

            //SPC.Services.Database.LocalDBService.RegisterService(new SPC.Services.Database.LocalDBService_Imp());

            //fix problem with serialization object with children
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,

                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore

            };

            //AppTheme appTheme = AppInfo.RequestedTheme;
            //if (appTheme == AppTheme.Dark)
            //    SPC.UI.Themes.Theme.SetTheme(new UI.Themes.DarkTheme());
            //else
            //    SPC.UI.Themes.Theme.SetTheme(new UI.Themes.LightTheme());

            SPC.Ctx.Init();

            SPC.Ctx.AppConfig.RegisterService(new ServerConfigProvider(Config));

            SPC.Ctx.DeviceInfo.RegisterService(new SPC.Services.UI.DeviceInfo());

            SPC.Helper.Voca.Register(new SPC.Services.Localization());

            //Task.Run(() => SPCTypes.GetClassType("SPC.BO.PS.DB"));

        }


        /// <summary>
        /// those serices is depended only from connection string. We should register it right after successfully testing DB connection
        /// 1. Fetch User info from OD
        /// 2. Clear DAG cache
        /// </summary>

        public async static Task RegisterDBDependentServicesAsync()
        {
            SPC.BO.PS.DBInfoList.InvalidateCache();

            await SPC.Security.Identity.LoadUserInfoFromDBAsync();

            SPC.Security.DataAccessGroup.RegisterService(new DataAccessGroup_Imp());

        }



        /// <summary>
        /// Call when logout
        /// </summary>
        /// <returns></returns>

        public static void UnRegisterDBDependentServices()
        {
            SPC.BO.PS.DBInfoList.InvalidateCache();
            SPC.UsrMan.ODInfoList.InvalidateCache();
            SPC.UsrMan.DAInfoList.InvalidateCache();
            SPC.UsrMan.DAG.InvalidateCache();
            SPC.UsrMan.OGInfoList.InvalidateCache();

        }



    }

}