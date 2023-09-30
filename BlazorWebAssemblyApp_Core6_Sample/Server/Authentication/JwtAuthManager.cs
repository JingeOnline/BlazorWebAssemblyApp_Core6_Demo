using BlazorWebAssemblyApp_Core6_Sample.Shared;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorWebAssemblyApp_Core6_Sample.Server.Authentication
{
    public class JwtAuthManager
    {
        public const string JWT_SECURITY_KEY = "123456789012345678901234567890123456789012345678901234567890";
        private const int JWT_TOKEN_VALID_MINS = 10;
        private UserAccountService _userAccountService;

        public JwtAuthManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public UserSessionModel? GetUserSession(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)||string.IsNullOrEmpty(password))
            {
                return null;
            }
            else
            {
                var userAccount=_userAccountService.GetAccountByUsernameAndPassword(userName,password);
                if(userAccount==null)
                {
                    return null;
                }
                else
                {
                    string jwtToken= generateJwtToken(userAccount);
                    UserSessionModel session = new UserSessionModel()
                    {
                        UserName=userAccount.UserName,
                        Role=userAccount.Role,
                        Token=jwtToken,
                        ExpiresIn=JWT_TOKEN_VALID_MINS*60
                    };
                    return session;
                }
            }
        }

        private string generateJwtToken(UserAccountModel userAccount)
        {
            DateTime tokenExpiryTime = DateTime.Now.AddMinutes(JWT_TOKEN_VALID_MINS);
            byte[] tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name,userAccount.UserName),
                        new Claim(ClaimTypes.Role,userAccount.Role)
                    };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            //Token的加密（签署）方法
            SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey)
                                            , SecurityAlgorithms.HmacSha256Signature);

            //构建Jwt Token
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTime,
                SigningCredentials = signingCredentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(securityTokenDescriptor);
            //把jwt token格式化成字符串
            string? token = tokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}
