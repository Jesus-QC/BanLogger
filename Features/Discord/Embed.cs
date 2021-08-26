namespace BanLogger.Features.Discord
{
    public class Embed
    {
        public int? color { get; set; }
        public EmbedAuthor author { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public EmbedImage image { get; set; }
        public EmbedFooter footer { get; set; }
    }
}