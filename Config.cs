using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace BanLogger
{
    public sealed class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        public string PublicWebhookUrl { get; set; } = "https://discord.com/api/webhooks/webhook.id/webhook.token";
        public string SecurityWebhookUrl { get; set; } = "https://discord.com/api/webhooks/webhook.id/webhook.token";
        [Description("Webhook Username")]
        public string Username { get; set; } = "MyServer Bans | Security";
        [Description("Webhook avatar image")]
        public string AvatarUrl { get; set; } = "https://imgur.com/FpPpR90.png";
        [Description("Content out of the embed, you can for example ping a role")]
        public string Content { get; set; } = "<@&811252527398912059>";
        [Description("Is TTS enabled?")]
        public bool IsTtsEnabled { get; set; } = true;
        [Description("Hex Color of the webhook")]
        public string HexColor { get; set; } = "#D10E11";
        [Description("Webhook author name depending of the port of the server")]
        public Dictionary<int, string> ServerName { get; set; } = new Dictionary<int, string>()
        {
            {
                7777, "MyServer #1 | Security"
            },
            {
                7778, "MyServer #2 | Security"
            },
            {
                7779, "MyServer #3 | Security"
            },
        };
        [Description("Webhook author image URL (server image)")]
        public string ServerImgUrl { get; set; } = "https://imgur.com/FpPpR90.png";
        [Description("Title of the embed")]
        public string Title { get; set; } = "BAN & KICK LOGGER";
        [Description("Default: User banned:")]
        public string UserBannedText { get; set; } = "User banned:";
        [Description("Default: Issuing staff:")]
        public string IssuingStaffText { get; set; } = "Issuing staff:";
        [Description("Default: Reason:")]
        public string ReasonText { get; set; } = "Reason:";
        [Description("Default: Time banned:")]
        public string TimeBannedText { get; set; } = "Time banned:";
        [Description("Webhook image URL")]
        public string ImageUrl { get; set; } = "https://imgur.com/spXs9Bq.png";
        [Description("Footer Text")]
        public string FooterTxt { get; set; } = "";
        [Description("Footer IconUrl")]
        public string FooterIconUrl { get; set; } = "";
    }
}