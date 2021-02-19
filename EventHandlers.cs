using System;
using System.IO;
using System.Net;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Newtonsoft.Json;

namespace BanLogger
{
    public partial class EventHandlers
    {
        public Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnBanning(BanningEventArgs ev)
        {
            if (string.IsNullOrEmpty(ev.Reason))
                ev.Reason = " ";
            if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
            {
                SendWebhook(ev.Target.Nickname, ev.Issuer.Nickname, ev.Reason, TimeFormatter(ev.Duration));
            }
            if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer.Nickname, ev.Reason, TimeFormatter(ev.Duration));
            }
        }
        public void OnKicking(KickingEventArgs ev)
        {
            if (string.IsNullOrEmpty(ev.Reason))
                ev.Reason = " ";
            if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
            {
                SendWebhook(ev.Target.Nickname, ev.Issuer.Nickname, ev.Reason, "Kick");
            }
            if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer.Nickname, ev.Reason, "Kick");
            }
        }

        public void SendWebhook(string bannedNickname, string issuerNickname, string reason, string Expire)
        {
            try
            {
                string name;
                if (!plugin.Config.ServerName.ContainsKey(Server.Port))
                {
                    name = "Ban Logger";
                }
                else
                {
                    name = plugin.Config.ServerName[Server.Port];
                }
                var message = new Message()
                {
                    Username = plugin.Config.Username,
                    AvatarUrl = plugin.Config.AvatarUrl,
                    Content = plugin.Config.Content,
                    Tts = plugin.Config.IsTtsEnabled,
                    Embeds = new[]{
                    new DiscordMessageEmbed()
                    {
                        Color = int.Parse(plugin.Config.HexColor.Replace("#", ""), System.Globalization.NumberStyles.HexNumber),
                        Author = new DiscordMessageEmbedAuthor()
                        {

                            Name = name, IconUrl= plugin.Config.ServerImgUrl
                        },
                        Title = plugin.Config.Title,
                        Description = $"{plugin.Config.UserBannedText}```{bannedNickname}```\n{plugin.Config.IssuingStaffText}```{issuerNickname}```\n{plugin.Config.ReasonText}```{reason}```\n{plugin.Config.TimeBannedText}```{Expire}```",
                        Image = new DiscordMessageEmbedImage()
                        {
                            Url = plugin.Config.ImageUrl
                        },
                        Footer = new DiscordMessageEmbedFooter()
                        {
                            IconUrl = plugin.Config.FooterIconUrl, Text = plugin.Config.FooterTxt,
                        }
                    }
                }
                };

                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(plugin.Config.PublicWebhookUrl);
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                using (var sendWebhook = new StreamWriter(webRequest.GetRequestStream()))
                {
                    string webhook = JsonConvert.SerializeObject(message);
                    sendWebhook.Write(webhook);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
        public void SendWebhook(string bannedNickname, string bannedUserID, string issuerNickname, string reason, string Expire)
        {
            try
            {
                string name;
                if (!plugin.Config.ServerName.ContainsKey(Server.Port))
                {
                    name = "Ban Logger";
                }
                else
                {
                    name = plugin.Config.ServerName[Server.Port];
                }
                var message = new Message()
                {

                    Username = plugin.Config.Username,
                    AvatarUrl = plugin.Config.AvatarUrl,
                    Content = plugin.Config.Content,
                    Tts = plugin.Config.IsTtsEnabled,
                    Embeds = new[]{
                    new DiscordMessageEmbed()
                    {
                        Color = int.Parse(plugin.Config.HexColor.Replace("#", ""), System.Globalization.NumberStyles.HexNumber),
                        Author = new DiscordMessageEmbedAuthor()
                        {
                            Name = plugin.Config.ServerName[Server.Port], IconUrl= plugin.Config.ServerImgUrl
                        },
                        Title = plugin.Config.Title,
                        Description = $"{plugin.Config.UserBannedText}```{bannedNickname} [{bannedUserID}]```\n{plugin.Config.IssuingStaffText}```{issuerNickname}```\n{plugin.Config.ReasonText}```{reason}```\n{plugin.Config.TimeBannedText}```{Expire}```",
                        Image = new DiscordMessageEmbedImage()
                        {
                            Url = plugin.Config.ImageUrl
                        },
                        Footer = new DiscordMessageEmbedFooter()
                        {
                            IconUrl = plugin.Config.FooterIconUrl, Text = plugin.Config.FooterTxt,
                        }
                    }
                }
                };

                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(plugin.Config.SecurityWebhookUrl);
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                using (var sendWebhook = new StreamWriter(webRequest.GetRequestStream()))
                {
                    string webhook = JsonConvert.SerializeObject(message);
                    sendWebhook.Write(webhook);
                }
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Message
        {
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }
            [JsonProperty("content")]
            public string Content { get; set; }
            [JsonProperty("tts")]
            public bool Tts { get; set; }
            [JsonProperty("embeds")]
            public DiscordMessageEmbed[] Embeds { get; set; }
        }
        [JsonObject(MemberSerialization.OptIn)]
        public class DiscordMessageEmbed
        {
            [JsonProperty("color")]
            public int? Color { get; set; }
            [JsonProperty("author")]
            public DiscordMessageEmbedAuthor Author { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("image")]
            public DiscordMessageEmbedImage Image { get; set; }
            [JsonProperty("footer")]
            public DiscordMessageEmbedFooter Footer { get; set; }

            [JsonConstructor]
            private DiscordMessageEmbed()
            {

            }

            public DiscordMessageEmbed(
                string title = null,
                int? color = null,
                DiscordMessageEmbedAuthor author = null,
                string url = null,
                string description = null,
                DiscordMessageEmbedImage image = null,
                DiscordMessageEmbedFooter footer = null)
            {
                Color = color;
                Author = author;
                Title = title;
                Url = url?.ToLower();
                Description = description;
                Image = image;
                Footer = footer;
            }

        }
        [JsonObject(MemberSerialization.OptIn)]
        public class DiscordMessageEmbedAuthor
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }

            [JsonConstructor]
            private DiscordMessageEmbedAuthor()
            {

            }

            public DiscordMessageEmbedAuthor(string name = null, string url = null, string iconUrl = null)
            {
                Name = name;
                Url = url;
                IconUrl = iconUrl;
            }
        }
        [JsonObject(MemberSerialization.OptIn)]
        public class DiscordMessageEmbedImage
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonConstructor]
            private DiscordMessageEmbedImage()
            {

            }

            public DiscordMessageEmbedImage(string url = null)
            {
                Url = url;
            }
        }
        [JsonObject(MemberSerialization.OptIn)]
        public class DiscordMessageEmbedFooter
        {
            [JsonProperty("text")]
            public string Text { get; set; }
            [JsonProperty("icon_url")]
            public string IconUrl { get; set; }

            [JsonConstructor]
            private DiscordMessageEmbedFooter()
            {

            }

            public DiscordMessageEmbedFooter(string text = null, string iconUrl = null)
            {
                Text = text;
                IconUrl = iconUrl;
            }
        }
        // This code is from @Sinsa's ScpAdminReports
        private string TimeFormatter(int duration)
        {
            if (duration < 60)
            {
                return ($"{duration}s");
            }
            else if (duration < 7200)
            {
                int newtime = (duration + 59) / 60;
                return ($"{newtime}min");
            }
            else if (duration < 129600)
            {
                int newtime = (duration + 3599) / 3600;
                string newtimestring;
                if (newtime.ToString().Length > 2)
                {
                    newtimestring = newtime.ToString().Substring(0, 3);
                }
                else
                {
                    newtimestring = newtime.ToString();
                }
                return ($"{newtimestring}h");
            }
            else if (duration < 2678400)
            {
                int newtime = duration / 86400;
                string newtimestring;
                if (newtime.ToString().Length > 2)
                {
                    newtimestring = newtime.ToString().Substring(0, 3);
                }
                else
                {
                    newtimestring = newtime.ToString();
                }
                return ($"{newtimestring}d");
            }
            else if (duration < 31622400)
            {
                int newtime = duration / 2592000;
                string newtimestring;
                if (newtime.ToString().Length > 2)
                {
                    newtimestring = newtime.ToString().Substring(0, 3);
                }
                else
                {
                    newtimestring = newtime.ToString();
                }

                return ($"{newtimestring}mon");
            }
            int newduration = duration / 31536000;
            string newdurationstring;
            if (newduration.ToString().Length > 2)
            {
                newdurationstring = newduration.ToString().Substring(0, 3);
            }
            else
            {
                newdurationstring = newduration.ToString();
            }

            return ($"{newdurationstring}y");
        }
    }
}
