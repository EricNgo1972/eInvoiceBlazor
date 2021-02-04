
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPC.Services.UI

{

    public class DeviceInfo : Ctx.IDeviceInfo
    {
        
        [NonSerialized] private static Dictionary<string, string> _infos;

        private Task<string> GetDeviceIdAsync()
        {
            var theDeviceID = "NA";
            //var theDeviceID = await _repo.GetItemAsStringAsync("DeviceId");

            //if (string.IsNullOrEmpty(theDeviceID))
            //{
            //    //no device id , first time running

            //    theDeviceID = Guid.NewGuid().ToString();

            //    await _repo.SetItemAsync("DeviceId", theDeviceID);

            //}


            return Task.FromResult(theDeviceID);

        }

        public static string GetDeviceFullInfo()
        {
            return JsonConvert.SerializeObject(_infos);

        }


        Dictionary<string, string> Ctx.IDeviceInfo.GetDeviceInfos()
        {

            if (_infos == null)

            {
                _infos = Task.Run(async () => await this.LoadDeviceInfoAsync()).Result;

            }



            return _infos;

        }



        bool Ctx.IDeviceInfo.IsConnectedToInternet()
        {
            return true;
        }



        private async Task<Dictionary<string, string>> LoadDeviceInfoAsync()
        {
            _infos = new Dictionary<string, string>();


            _infos.Add("DeviceType", "BlazorClient");

            _infos.Add("Manufacturer", "SPC");

            _infos.Add("DeviceName", "Browser");

            _infos.Add("Platform", "AspNetCore");

            _infos.Add("Version", "");
                        
            var theId = await GetDeviceIdAsync();

            _infos.Add("DeviceId", theId);

            return _infos;

        }





    }

}