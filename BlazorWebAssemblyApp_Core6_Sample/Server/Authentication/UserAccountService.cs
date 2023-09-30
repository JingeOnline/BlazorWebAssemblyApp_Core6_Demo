namespace BlazorWebAssemblyApp_Core6_Sample.Server.Authentication
{
    public class UserAccountService
    {
        private List<UserAccountModel> _users;

        public UserAccountService()
        {
            _users = new List<UserAccountModel>()
            {
                new UserAccountModel{UserName="admin",Password="admin",Role="Administrator"},
                new UserAccountModel{UserName="user",Password="user", Role="User"}
            };

        }

        public UserAccountModel? GetAccountByUsername(string username)
        {
            return _users.FirstOrDefault(x => x.UserName == username);
        }
        public UserAccountModel? GetAccountByUsernameAndPassword(string username,string password)
        {
            return _users.FirstOrDefault(x => x.UserName == username && x.Password==password);
        }
    }
}
