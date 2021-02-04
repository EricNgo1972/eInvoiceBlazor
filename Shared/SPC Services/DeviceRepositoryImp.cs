using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SPC.Services.Storage
{
    public class DeviceRepositoryImp : SPC.Interfaces.IKeyValueRepository
    {

        private static ILocalStorageService localStorage;

        public DeviceRepositoryImp(ILocalStorageService pService)
        {
            localStorage = pService;
        }

        public async Task<bool> IsKeyExistsAsync(string key)
        {
            try
            {

                var val = await localStorage.ContainKeyAsync(key);

                return val;
            }
            catch (Exception)

            {
                return false;

            }

        }

        public async Task<string> LoadFromRepositoryAsync(string key, bool Decrypt = false)
        {
            try
            {                  
                return await localStorage.GetItemAsStringAsync(key);
                                
            }
            catch (Exception)

            {

                return string.Empty;

                // Possible that device doesn't support secure storage on device.
            }

        }

        public async Task SaveToRepositoryAsync(string Key, string value, bool Encypt = false)
        {
            try
            {
                await localStorage.SetItemAsync(Key, value);
                // var test = await SecureStorage.GetAsync(Key);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                TextLogger.Log(ex);

            }

        }

    }

}