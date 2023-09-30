using Blazored.SessionStorage;
using BlazorWebAssemblyApp_Core6_Sample.Client.Extensions;
using BlazorWebAssemblyApp_Core6_Sample.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorWebAssemblyApp_Core6_Sample.Client.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                UserSessionModel session = await _sessionStorage.ReadEncryptedItemAsync<UserSessionModel>("UserSession");
                if (session == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }
                else
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,session.UserName),
                        new Claim(ClaimTypes.Role,session.Role)
                    };
                    //别忘了指定AuthenticationType，什么值都可以，但不要缺失。缺少的话，该用户就会被判定为匿名用户。
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,"JwtAuth");
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
                    return await Task.FromResult(new AuthenticationState(principal));
                }
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        //当用户登入或者登出的时候，更新AuthenticationState。
        //登入的时候，传入UserSessionModel。登出的时候，传入null。
        public async Task UpdateAuthenticationState(UserSessionModel? userSession)
        {
            ClaimsPrincipal claimsPrincipal;
            //用户登入
            if(userSession!=null)
            {
                List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,userSession.UserName),
                        new Claim(ClaimTypes.Role,userSession.Role)
                    };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
                claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                userSession.ExpiryTime = DateTime.Now.AddSeconds(userSession.ExpiresIn);
                await _sessionStorage.SaveItemEncryptedAsync("UserSession", userSession);
            }
            //用户登出
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorage.RemoveItemAsync("UserSession");
            }
            //通知客户端更新状态
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        /// <summary>
        /// 用于Blazor组件从Session Storage中获取JWT Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            var result = string.Empty;
            try
            {
                var userSession = await _sessionStorage.ReadEncryptedItemAsync<UserSessionModel>("UserSession");
                if (userSession!=null && DateTime.Now<userSession.ExpiryTime)
                {
                    result = userSession.Token;
                }
            }
            catch
            {

            }
            return result;
        }
    }
}
