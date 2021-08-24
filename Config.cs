using System.Collections.Generic;
using BanLogger.Features.Enums;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace BanLogger
{
    public sealed class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Public Webhooks (Mute/Kick/Ban/OBan)")]
        public Dictionary<MessageType, string> PublicWebhooks { get; set; } = new Dictionary<MessageType, string>()
        {
            {
                MessageType.Kick, "https://discord.com/api/webhooks/webhook.id/webhook.token"
            },
            {
                MessageType.OBan, "https://discord.com/api/webhooks/webhook.id/webhook.token"
            }
        };
        [Description("Private Webhooks (Mute/Kick/Ban/OBan) Contains IDs and more info")]
        public Dictionary<MessageType, string> PrivateWebhooks { get; set; } = new Dictionary<MessageType, string>()
        {
            {
                MessageType.Mute, "https://discord.com/api/webhooks/webhook.id/webhook.token"
            },
            {
                MessageType.Ban, "https://discord.com/api/webhooks/webhook.id/webhook.token"
            }
        };

        [Description("Steam Api key to get the nickname of obanned users (Get your api key in https://steamcommunity.com/dev/apikey)")]
        public string SteamApiKey { get; set; } = "00000000000000000000000000000000";
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
        public string AuthorName { get; set; } = "MyServer #1 | Security";
        [Description("Webhook author image URL (server image)")]
        public string ServerImgUrl { get; set; } = "https://imgur.com/FpPpR90.png";
        [Description("Title of the embed")]
        public string Title { get; set; } = "BAN & KICK LOGGER";
        [Description("Default: User banned:")]
        public string UserBannedText { get; set; } = "User:";
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