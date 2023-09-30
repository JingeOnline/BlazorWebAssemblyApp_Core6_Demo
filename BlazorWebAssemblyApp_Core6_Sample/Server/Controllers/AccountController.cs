using BlazorWebAssemblyApp_Core6_Sample.Server.Authentication;
using BlazorWebAssemblyApp_Core6_Sample.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebAssemblyApp_Core6_Sample.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private UserAccountService _userAccountService;

        public AccountController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<UserSessionModel> Login([FromBody] LoginRequestModel loginRequest)
        {
            JwtAuthManager manager = new JwtAuthManager(_userAccountService);
            UserSessionModel session = manager.GetUserSession(loginRequest.UserName,loginRequest.Password);
            if(session is null)
            {
                return Unauthorized();
            }
            else
            {
                return session;
            }
        }
    }
}
