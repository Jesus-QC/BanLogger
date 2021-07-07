using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Utf8Json;

namespace BanLogger
{
    public class EventHandlers
    {
        private readonly Plugin _plugin;
        public EventHandlers(Plugin plugin) => _plugin = plugin;

        private string _defaultUrl = "https://discord.com/api/webhooks/webhook.id/webhook.token";
        
        public void OnBanning(BanningEventArgs ev)
        {
            if (!string.IsNullOrEmpty(_plugin.Config.PublicWebhookUrl) && _plugin.Config.PublicWebhookUrl != _defaultUrl)
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, TimeFormatter(ev.Duration));
            }
            if (!string.IsNullOrEmpty(_plugin.Config.SecurityWebhookUrl) && _plugin.Config.SecurityWebhookUrl != _defaultUrl)
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, TimeFormatter(ev.Duration), false);
            }
        }

        public void OnKicking(KickingEventArgs ev)
        {
            if (!string.IsNullOrEmpty(_plugin.Config.PublicWebhookUrl) && _plugin.Config.PublicWebhookUrl != _defaultUrl)
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, "Kick");
            }
            if (!string.IsNullOrEmpty(_plugin.Config.SecurityWebhookUrl) && _plugin.Config.SecurityWebhookUrl != _defaultUrl)
            {
                SendWebhook(ev.Target.Nickname, ev.Target.UserId, ev.Issuer?.Nickname ?? "Server Console", ev.Reason, "Kick", false);
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
                    time = TimeFormatter(timeInt);
                else
                    time = "Unknown";
                
                if (ev.Details.Id.Contains("@steam") && !string.IsNullOrEmpty(_plugin.Config.SteamApiKey))
                {
                    try
                    {
                        GetUserName(ev.Details.Id, out string nickname);

                        if (!string.IsNullOrEmpty(_plugin.Config.PublicWebhookUrl) && _plugin.Config.PublicWebhookUrl != _defaultUrl)
                        {
                            SendWebhook(nickname, ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time);
                        }
                        if (!string.IsNullOrEmpty(_plugin.Config.SecurityWebhookUrl) && _plugin.Config.SecurityWebhookUrl != _defaultUrl)
                        {
                            SendWebhook(nickname, ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time, false);
                        }
                    }
                    catch(Exception e)
                    {
                        if (!string.IsNullOrEmpty(_plugin.Config.PublicWebhookUrl) && _plugin.Config.PublicWebhookUrl != _defaultUrl)
                        {
                            SendWebhook("UNKNOWN (invalid steam api key)", ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time);
                        }
                        if (!string.IsNullOrEmpty(_plugin.Config.SecurityWebhookUrl) && _plugin.Config.SecurityWebhookUrl != _defaultUrl)
                        {
                            SendWebhook("UNKNOWN (invalid steam api key)", ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time, false);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(_plugin.Config.PublicWebhookUrl) && _plugin.Config.PublicWebhookUrl != _defaultUrl)
                    {
                        SendWebhook("UNKNOWN", ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time);
                    }
                    if (!string.IsNullOrEmpty(_plugin.Config.SecurityWebhookUrl) && _plugin.Config.SecurityWebhookUrl != _defaultUrl)
                    {
                        SendWebhook("UNKNOWN", ev.Details.Id, ev.Issuer?.Nickname ?? "Server Console", ev.Details.Reason, time, false);
                    }
                }
            }
        }

        public void SendWebhook(string bannedPly, string bannedPlyId, string issuerStaffNickname, string reason, string time, bool isPublic = true)
        {
            if (string.IsNullOrEmpty(reason))
                reason = " ";

            string name;
            if (!_plugin.Config.ServerName.ContainsKey(Server.Port))
                name = "Server #1 | Security";
            else
                name = _plugin.Config.ServerName[Server.Port];
            
            string desc;
            string finalurl;
            if (isPublic)
            {
                finalurl = _plugin.Config.PublicWebhookUrl;
                desc = $"{_plugin.Config.UserBannedText}\n```{bannedPly}```\n{_plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{_plugin.Config.ReasonText}\n```{reason}```\n{_plugin.Config.TimeBannedText}\n```{time}```";
            }
            else
            {
                finalurl = _plugin.Config.SecurityWebhookUrl;
                desc = $"{_plugin.Config.UserBannedText}\n```{bannedPly} ({bannedPlyId})```\n{_plugin.Config.IssuingStaffText}\n```{issuerStaffNickname}```\n{_plugin.Config.ReasonText}\n```{reason}```\n{_plugin.Config.TimeBannedText}\n```{time}```";
            }

            var message = new Message
            {
                username = _plugin.Config.Username,
                avatar_url = _plugin.Config.AvatarUrl,
                content = _plugin.Config.Content,
                tts = _plugin.Config.IsTtsEnabled,
                embeds = new[]{
                    new DiscordMessageEmbed
                    {
                        color = int.Parse(_plugin.Config.HexColor.Replace("#", ""), NumberStyles.HexNumber),
                        author = new DiscordMessageEmbedAuthor
                        {
                            name = name, icon_url = _plugin.Config.ServerImgUrl,
                        },
                        title = _plugin.Config.Title,
                        description = desc,
                        image = new DiscordMessageEmbedImage
                        {
                            url = _plugin.Config.ImageUrl,
                        },
                        footer = new DiscordMessageEmbedFooter
                        {
                            icon_url = _plugin.Config.FooterIconUrl, text = _plugin.Config.FooterTxt,
                        }
                    }
                }
            };

            SendMessage(message, finalurl);
        }

        public void SendMessage(Message message, string url)
        {
            var secondaryThread = new Thread(() =>
            {
                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.ContentType = "application/json";
                webRequest.Method = "POST";

                try
                {
                    using (var sendWebhook = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        var webhook = JsonSerializer.Serialize(message);
                        sendWebhook.Write(System.Text.Encoding.UTF8.GetString(webhook));
                    }

                    var response = (HttpWebResponse)webRequest.GetResponse();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });
            
            secondaryThread.Start();
        }

        public void GetUserName(string userid, out string nickname)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_plugin.Config.SteamApiKey}&steamids={userid}");
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                nickname = Regex.Match(result, @"\x22personaname\x22:\x22(.+?)\x22").Groups[1].Value;
            }
        }

        public class Message
        {
            public string username { get; set; }
            public string avatar_url { get; set; }
            public string content { get; set; }
            public bool tts { get; set; }
            public DiscordMessageEmbed[] embeds { get; set; }
        }
        
        public class DiscordMessageEmbed
        {
            public int? color { get; set; }
            public DiscordMessageEmbedAuthor author { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string description { get; set; }
            public DiscordMessageEmbedImage image { get; set; }
            public DiscordMessageEmbedFooter footer { get; set; }
        }

        public class DiscordMessageEmbedAuthor
        {
            public string name { get; set; }
            public string url { get; set; }
            public string icon_url { get; set; }
        }
        
        public class DiscordMessageEmbedImage
        {
            public string url { get; set; }
        }
        
        public class DiscordMessageEmbedFooter
        {
            public string text { get; set; }
            public string icon_url { get; set; }
        }

        private string TimeFormatter(int duration)
        {
            // This code is from @Sinsa's ScpAdminReports
            
            if (duration < 60)
            {
                return $"{duration}s";
            }

            if (duration < 7200)
            {
                int newtime = (duration + 59) / 60;
                return $"{newtime}min";
            }

            if (duration < 129600)
            {
                int newtime = (duration + 3599) / 3600;
                string newtimestring;
                
                if (newtime.ToString().Length > 2)
                    newtimestring = newtime.ToString().Substring(0, 3);
                else
                    newtimestring = newtime.ToString();
                
                return $"{newtimestring}h";
            }

            if (duration < 2678400)
            {
                int newtime = duration / 86400;
                string newtimestring;
                
                if (newtime.ToString().Length > 2)
                    newtimestring = newtime.ToString().Substring(0, 3);
                else
                    newtimestring = newtime.ToString();
                    
                
                return $"{newtimestring}d";
            }

            if (duration < 31622400)
            {
                int newtime = duration / 2592000;
                string newtimestring;
                
                if (newtime.ToString().Length > 2)
                    newtimestring = newtime.ToString().Substring(0, 3);
                else
                    newtimestring = newtime.ToString();
                

                return $"{newtimestring}mon";
            }
            
            int newduration = duration / 31536000;
            string newdurationstring;
            if (newduration.ToString().Length > 2)
                newdurationstring = newduration.ToString().Substring(0, 3);
            else
                newdurationstring = newduration.ToString();
            

            return $"{newdurationstring}y";
        }
    }
}
