using Blazored.SessionStorage;
using System.Text;
using System.Text.Json;

namespace BlazorWebAssemblyApp_Core6_Sample.Client.Extensions
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class SessionStorageServiceExtension
    {
        /// <summary>
        /// 储存信息到Session Storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionStorageService"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task SaveItemEncryptedAsync<T>(this ISessionStorageService sessionStorageService,string key,T item)
        {
            string itemJson=JsonSerializer.Serialize(item);
            byte[] itemJsonBytes=Encoding.UTF8.GetBytes(itemJson);
            string base64Json=Convert.ToBase64String(itemJsonBytes);
            //以键值对的形式储存到Session Storage中
            await sessionStorageService.SetItemAsync(key, base64Json);
        }

        /// <summary>
        /// 从Session Storage中读取信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionStorageService"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> ReadEncryptedItemAsync<T>(this ISessionStorageService sessionStorageService, string key)
        {
            string base64Json = await sessionStorageService.GetItemAsync<string>(key);
            byte[] itemJsonBytes=Convert.FromBase64String(base64Json);
            string itemJson = Encoding.UTF8.GetString(itemJsonBytes);
            var item=JsonSerializer.Deserialize<T>(itemJson);
            return item;
        }
    }
}
