using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SPC.CORE;
using SPC.Helper;
using SPC.Helper.Exceptions;
using SPC.Helper.Extension;
using SPC.Interfaces;
using SPC.Services.UI;


namespace SPC.Services
{
    public class RunUrlService_Imp: IRunURLService
    {

        [Inject] NavigationManager nav { get; set; }

      

        Task IRunURLService.RunAsync(CmdArg Arg)
        {
            return RunTaskAsync(Arg);
        }

        private async Task RunTaskAsync(CmdArg Arg)
        {
            try
            {


                if (Arg.Action == Helper.Action._Help)
                {
                    SPC.Services.InternetChecker.OnlineGuard();

                    string Topic = Arg.ShortCut;
                    if (!string.IsNullOrEmpty(Topic))
                    {
                        WaitingService.Wait("Openning Online Help".Translate(), Arg.Url);

                        var theHelpUrl = await SPC.Cloud.HelpService.GetHelpTopicUrlAsync(Topic);

                        nav.NavigateTo(theHelpUrl);

                        WaitingService.Done();
                    }
                }

                else if (Arg.GetShortCutSegment(0).Equals("Debug", StringComparison.OrdinalIgnoreCase))
                {
                    var onoff = Arg.GetDefaultParameter();
                    if (string.IsNullOrEmpty(onoff))
                        Ctx.AppConfig.DebugMode = !Ctx.AppConfig.DebugMode;
                    else
                        Ctx.AppConfig.DebugMode = onoff.ToBoolean();

                    AlertService.Toast($"Debug Mode is : {(Ctx.AppConfig.DebugMode ? "ON" : "OFF")}");
                }

                else if (Arg.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase)) //Open online resources
                {

                    WaitingService.Wait("Openning browser", Arg.Url);

                    nav.NavigateTo(Arg.Url);

                    WaitingService.Done();
                }

                else if (Arg.Url.StartsWith(@"/", StringComparison.OrdinalIgnoreCase)) //Open Local files
                {

                    WaitingService.Done();
                }

                else if (Arg.Url.StartsWith(@"mailto://", StringComparison.OrdinalIgnoreCase))
                {

                    WaitingService.Wait("Composing mail ...", "");

                    nav.NavigateTo(Arg.Url);

                    WaitingService.Done();

                }

                else if (Arg.Url.MatchesRegExp(@"^[A-Za-z]+://")) //Open other apps "lyft://
                {
                    WaitingService.Done();

                }

                else if (Arg.Action.MatchesRegExp("^Insert$|^Update$|^InsertUpdate$|^UpdateAll$|^Delete$|^DeleteAll$"))
                //NON-UI Commands
                {
                    WaitingService.Wait(Arg.Action, Arg.Url);

                    await SPC.Commands.NonUIActionRunner.RunURLCommandAsync(Arg);

                    WaitingService.Done();
                }

                else
                {

                    var pClass = SPCTypes.GetClassType(Arg.GetShortCutSegment(0), true);

                    if (pClass != null)
                    {
                        //Runable non UI command if Action = Run or the command is not a mix of Editable and Runnable
                        if (pClass.GetInterfaces().Contains(typeof(IRunable)) && (Arg.Action == Helper.Action._Run || !pClass.GetInterfaces().Contains(typeof(IEditable))))
                        {
                            var Sample = BOFactory.CreateSample(pClass) as IRunable;

                            await Sample.RunAsync(Arg);

                        }
                        //Class is ISupportQueryInfoList and the Action = Select
                        else if (Arg.Action == Helper.Action._Select && pClass.GetInterfaces().Contains(typeof(SPC.Interfaces.ISupportQueryInfoList))) //Query BO
                        {
                            if (BOFactory.CreateSample(pClass) is ISupportQueryInfoList sample)
                            {
                                var theList = await sample.GetInfoListAsync(Arg.GetDictionary()) as IList;
                                await ValueSelector.SelectAsync(theList, "Info List");
                            }

                        }

                        else

                        {
                            AlertService.Toast($"Could not find any suitable Page for class : {Arg.ShortCut}");
                            WaitingService.Done();

                        }

                    }


                    else //Short Command is not a class
                    {

                       
                            WaitingService.Done();
                            AlertService.Toast($"Could not find the class {Arg.GetShortCutSegment(0)}");
 
                    }

                }


            }
            catch (Exception ex)
            {
                WaitingService.Done();
                if (ex.GetType() == typeof(NullReferenceException)) //file extension is not support by ios
                {
                    try
                    {
                        AlertService.ShowError(ex);
                    }
                    catch (Exception ex1)
                    {
                        //ExceptionHandler.LogAndAlert(ex1);
                        // AlertService.ShowError(ex1);
                    }
                }
                //if (ex.GetType() == typeof(ExitScriptException)) //file extension is not support by ios
                    //ExceptionHandler.LogAndToast(ex);
                //else
                    //ExceptionHandler.LogAndAlert(ex);
                //AlertService.ShowError(ex);

            }


        }

        /// <summary>
        /// Throw exception if user did not in any entity
        /// </summary>
        /// <param name="Arg"></param>
        internal static void GuardCurrentEntity(CmdArg Arg)
        {
            var theClass = Arg.GetShortCutSegment(0);
            if (theClass.StartsWith("SPC.UsrMan", StringComparison.OrdinalIgnoreCase) || theClass.MatchesRegExp(@"^SPC.Commands$|^SPC\.BO\.CD$|^SPC\.BO\.PS\.CD$"))
            {
                //system menu is ok
            }
            else
            {
                if (string.IsNullOrEmpty(Ctx.EntityInfo.CurrentBECode))
                {
                    ExceptionThower.BusinessRuleStop("Please select an Entity first.".Translate());
                }
            }
        }



    }



}