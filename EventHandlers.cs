using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
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
            if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
            {
                SendWebhook(ev.Target, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, TimeFormatter(ev.Duration));
            }
            if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
            {
                SendWebhook(ev.Target, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, TimeFormatter(ev.Duration), false);
            }
        }

        public void OnKicking(KickingEventArgs ev)
        {
            if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
            {
                SendWebhook(ev.Target, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, "Kick");
            }
            if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
            {
                SendWebhook(ev.Target, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, "Kick", false);
            }
        }

        public void OnPlayerOban(BannedEventArgs ev)
        {
            if (ev.Type != BanHandler.BanType.UserId) return;
            if (ev.Details.OriginalName == "Unknown - offline ban")
            {
                string ticksintime = TimeSpan.FromTicks(ev.Details.Expires - ev.Details.IssuanceTime).TotalSeconds.ToString();
                string time;
                if(int.TryParse(ticksintime, out int timeInt))
                {
                    time = TimeFormatter(timeInt);
                }
                else
                {
                    time = "Unknown";
                }
                if (ev.Details.Id.Contains("@steam") && !string.IsNullOrEmpty(plugin.Config.SteamApiKey))
                {
                    string nickname;
                    string pattern = @"(\d+)";
                    var match = Regex.Match(ev.Details.Id, pattern);

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + plugin.Config.SteamApiKey + "&steamids=" + match.Groups[0].Value);
                    httpWebRequest.Method = "GET";

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        string getname = @"\x22personaname\x22:\x22(.+?)\x22";
                        nickname = Regex.Match(result, getname).Groups[1].Value;
                        Log.Info(nickname);
                    }

                    if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
                    {
                        SendObanWebhook(nickname, ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time);
                    }
                    if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
                    {
                        SendObanWebhook(nickname, ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time, false);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(plugin.Config.PublicWebhookUrl))
                    {
                        SendObanWebhook("UNKNOWN", "oban", ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time);
                    }
                    if (!string.IsNullOrEmpty(plugin.Config.SecurityWebhookUrl))
                    {
                        SendObanWebhook("UNKNOWN", "oban", ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time, false);
                    }
                }
            }
        }

        public void SendWebhook(Player bannedPly, string issuerStaffNickname, string reason, string time, bool IsPublic = true, bool IsOban = false)
        {
            if (string.IsNullOrEmpty(reason))
                reason = " ";

            string name;
            if (!plugin.Config.ServerName.ContainsKey(Server.Port))
            {
                name = "Server #1 | Security";
            }
            else
            {
                name = plugin.Config.ServerName[Server.Port];
            }

            string desc;
            string finalurl;
            if (IsPublic)
            {
                finalurl = plugin.Config.PublicWebhookUrl;
                desc = $"{plugin.Config.UserBannedText}\n```{bannedPly.Nickname}```\n{plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{plugin.Config.ReasonText}\n```{reason}```\n{plugin.Config.TimeBannedText}\n```{time}```";
            }
            else
            {
                finalurl = plugin.Config.SecurityWebhookUrl;
                desc = $"{plugin.Config.UserBannedText}\n```{bannedPly.Nickname} ({bannedPly.UserId})```\n{plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{plugin.Config.ReasonText}\n```{reason}```\n{plugin.Config.TimeBannedText}\n```{time}```";
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
                            Name = name, IconUrl= plugin.Config.ServerImgUrl,
                        },
                        Title = plugin.Config.Title,
                        Description = desc,
                        Image = new DiscordMessageEmbedImage()
                        {
                            Url = plugin.Config.ImageUrl,
                        },
                        Footer = new DiscordMessageEmbedFooter()
                        {
                            IconUrl = plugin.Config.FooterIconUrl, Text = plugin.Config.FooterTxt,
                        }
                    }
                }
            };

            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(finalurl);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

            using (var sendWebhook = new StreamWriter(webRequest.GetRequestStream()))
            {
                string webhook = JsonConvert.SerializeObject(message);
                sendWebhook.Write(webhook);
            }

            var response = (HttpWebResponse)webRequest.GetResponse();
        }
        public void SendObanWebhook(string bannedPly, string userId, string issuerStaffNickname, string reason, string time, bool IsPublic = true)
        {
            if (string.IsNullOrEmpty(reason))
                reason = " ";

            string name;
            if (!plugin.Config.ServerName.ContainsKey(Server.Port))
            {
                name = "Server #1 | Security";
            }
            else
            {
                name = plugin.Config.ServerName[Server.Port];
            }

            string desc;
            string finalurl;
            if (IsPublic)
            {
                finalurl = plugin.Config.PublicWebhookUrl;
                desc = $"{plugin.Config.UserBannedText}\n```{bannedPly}```\n{plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{plugin.Config.ReasonText}\n```{reason}```\n{plugin.Config.TimeBannedText}\n```{time}```";
            }
            else
            {
                finalurl = plugin.Config.SecurityWebhookUrl;
                desc = $"{plugin.Config.UserBannedText}\n```{bannedPly} ({userId})```\n{plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{plugin.Config.ReasonText}\n```{reason}```\n{plugin.Config.TimeBannedText}\n```{time}```";
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
                            Name = name, IconUrl= plugin.Config.ServerImgUrl,
                        },
                        Title = plugin.Config.Title,
                        Description = desc,
                        Image = new DiscordMessageEmbedImage()
                        {
                            Url = plugin.Config.ImageUrl,
                        },
                        Footer = new DiscordMessageEmbedFooter()
                        {
                            IconUrl = plugin.Config.FooterIconUrl, Text = plugin.Config.FooterTxt,
                        }
                    }
                }
            };

            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(finalurl);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

            using (var sendWebhook = new StreamWriter(webRequest.GetRequestStream()))
            {
                string webhook = JsonConvert.SerializeObject(message);
                sendWebhook.Write(webhook);
            }

            var response = (HttpWebResponse)webRequest.GetResponse();
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
                this.Color = color;
                this.Author = author;
                this.Title = title;
                this.Url = url?.ToLower();
                this.Description = description;
                this.Image = image;
                this.Footer = footer;
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
                this.Name = name;
                this.Url = url;
                this.IconUrl = iconUrl;
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
                this.Url = url;
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
                this.Text = text;
                this.IconUrl = iconUrl;
            }
        }

        private string TimeFormatter(int duration)
        {
            // This code is from @Sinsa's ScpAdminReports
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
