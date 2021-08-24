namespace BanLogger.Features.Structs
{
    public struct UserInfo
    {
        public UserInfo(string username, string userId)
        {
            Username = username;
            UserId = userId;
        }
        
        public string Username;
        public string UserId;
        
        public string PublicInfo => string.IsNullOrEmpty(Username) ? "n/a" : Username;
        public string PrivateInfo => $"{(string.IsNullOrEmpty(Username) ? "n/a" : Username)} ({(string.IsNullOrEmpty(UserId) ? "n/a" : UserId)})";
    }
}