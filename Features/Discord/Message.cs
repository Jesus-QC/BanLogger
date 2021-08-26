using System.Collections.Generic;

namespace BanLogger.Features.Discord
{
    public class Message
    {
        public Message()
        {
            username = Plugin.Instance.Config.Username;
            avatar_url = Plugin.Instance.Config.AvatarUrl;
            content = Plugin.Instance.Config.Content;
            tts = Plugin.Instance.Config.IsTtsEnabled;
            embeds = new List<Embed>();
        }
            
        public string username { get; set; }
        public  string avatar_url { get; set; }
        public  string content { get; set; }
        public  bool tts { get; set; }
        public List<Embed> embeds { get; set; }
    }
}