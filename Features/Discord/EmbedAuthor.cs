namespace BanLogger.Features.Discord
{
    public class EmbedAuthor
    {
        public EmbedAuthor(string name, string url, string iconUrl)
        {
            this.name = name;
            this.url = url;
            icon_url = iconUrl;
        }
            
        public string name { get; set; }
        public string url { get; set; }
        public string icon_url { get; set; }
    }
}