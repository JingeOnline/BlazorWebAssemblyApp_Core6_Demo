using Blazored.SessionStorage;
using BlazorWebAssemblyApp_Core6_Sample.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorWebAssemblyApp_Core6_Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            //�ڿͻ�����ָ��server api�� root URL������builder.HostEnvironment.BaseAddress��ʾapi server��URL���ǵ�ǰServer��Ŀ��URL��
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //��ӵ�������
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddAuthorizationCore();
            //�÷�������ͻ��˶�ȡSession storage��ʡȥ����JS��ȡ�Ĳ��㡣
            builder.Services.AddBlazoredSessionStorage();

            await builder.Build().RunAsync();
        }
    }
}