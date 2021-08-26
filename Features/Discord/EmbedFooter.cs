namespace BanLogger.Features.Discord
{
    public class EmbedFooter
    {
        public EmbedFooter(string text, string iconUrl)
        {
            this.text = text;
            icon_url = iconUrl;
        }
            
        public string text { get; set; }
        public string icon_url { get; set; }
    }
}