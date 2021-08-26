using System;
using Exiled.API.Features;
using System.Globalization;
using BanLogger.Features;
using Exiled.Events.EventArgs;
using BanLogger.Features.Enums;
using BanLogger.Features.Structs;


namespace BanLogger
{
    public class EventHandlers
    {
        public void OnBanning(BanningEventArgs ev)
        {
            if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.Ban) && Plugin.Instance.Config.PrivateWebhooks[MessageType.Ban] != DiscordHandler.DefaultUrl)
                Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Target.Nickname, ev.Target.UserId), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Reason, ev.Duration), MessageType.Ban, WebhookType.Private);
            if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.Ban) && Plugin.Instance.Config.PublicWebhooks[MessageType.Ban] != DiscordHandler.DefaultUrl)
                Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Target.Nickname, ev.Target.UserId), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Reason, ev.Duration), MessageType.Ban, WebhookType.Public);
        }

        public void OnKicking(KickingEventArgs ev)
        {
            if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.Kick) && Plugin.Instance.Config.PrivateWebhooks[MessageType.Kick] != DiscordHandler.DefaultUrl)
                Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Target.Nickname, ev.Target.UserId), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Reason, -1), MessageType.Kick, WebhookType.Private);
            if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.Kick) && Plugin.Instance.Config.PublicWebhooks[MessageType.Kick] != DiscordHandler.DefaultUrl)
                Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Target.Nickname, ev.Target.UserId), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Reason, -1), MessageType.Kick, WebhookType.Public);
        }

        public void OnMuted(ChangingMuteStatusEventArgs ev)
        {
            if (ev.IsMuted)
            {
                if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.Mute) && Plugin.Instance.Config.PrivateWebhooks[MessageType.Mute] != DiscordHandler.DefaultUrl)
                    Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Player.Nickname, ev.Player.UserId), new UserInfo("n/a", "n/a"), "mute", -1), MessageType.Mute, WebhookType.Private);
                if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.Mute) && Plugin.Instance.Config.PublicWebhooks[MessageType.Mute] != DiscordHandler.DefaultUrl)
                    Features.Utils.CreateEmbed(new BanInfo(new UserInfo(ev.Player.Nickname, ev.Player.UserId), new UserInfo("n/a", "n/a"), "mute", -1), MessageType.Mute, WebhookType.Public);
            }
        }

        public void OnPlayerOBan(BannedEventArgs ev)
        {
            if (ev.Type != BanHandler.BanType.UserId) return;
            if (ev.Details.OriginalName == "Unknown - offline ban")
            {
                var ticks = TimeSpan.FromTicks(ev.Details.Expires - ev.Details.IssuanceTime).TotalSeconds.ToString(CultureInfo.InvariantCulture);
                var time = long.TryParse(ticks, out var timeInt) ? timeInt : -1;

                if (ev.Details.Id.Contains("@steam") && !string.IsNullOrEmpty(Plugin.Instance.Config.SteamApiKey))
                {
                    try
                    {
                        var nickname = Features.Utils.GetUserName(ev.Details.Id);

                        if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PrivateWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                            Features.Utils.CreateEmbed(new BanInfo(new UserInfo(nickname, ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Private);
                        if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PublicWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                            Features.Utils.CreateEmbed(new BanInfo(new UserInfo(nickname, ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Public);
                    }
                    catch(Exception)
                    {
                        Log.Error("An error has ocurred trying to get the username of an obaned user.");
                        
                        if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PrivateWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                            Features.Utils.CreateEmbed(new BanInfo(new UserInfo("Unknown (Incorrect API Key)", ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Private);
                        if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PublicWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                            Features.Utils.CreateEmbed(new BanInfo(new UserInfo("Unknown (Incorrect API Key)", ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Public);
                    }
                }
                else
                {
                    if(Plugin.Instance.Config.PrivateWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PrivateWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                        Features.Utils.CreateEmbed(new BanInfo(new UserInfo("Unknown (OBan)", ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Private);
                    if(Plugin.Instance.Config.PublicWebhooks.ContainsKey(MessageType.OBan) && Plugin.Instance.Config.PublicWebhooks[MessageType.OBan] != DiscordHandler.DefaultUrl)
                        Features.Utils.CreateEmbed(new BanInfo(new UserInfo("Unknown (OBan)", ev.Details.Id), new UserInfo(ev.Issuer?.Nickname ?? "Server Console", ev.Issuer?.UserId ?? "n/a"), ev.Details.Reason, time), MessageType.OBan, WebhookType.Public);
                }
            }
        }
    }
}
