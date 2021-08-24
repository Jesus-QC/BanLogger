using MEC;
using Utf8Json;
using System.Linq;
using Exiled.API.Features;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace BanLogger.Features
{
    public class Discord
    {
        public static string DefaultUrl = "https://discord.com/api/webhooks/webhook.id/webhook.token";
        public static Dictionary<string, List<Embed>> Queue = new Dictionary<string, List<Embed>>();
        public static CoroutineHandle CoroutineHandle;

        private static IEnumerator<float> SendMessage(Message message, string url)
        {
            var webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            var uploadHandler = new UploadHandlerRaw(JsonSerializer.Serialize(message));
            uploadHandler.contentType = "application/json";
            webRequest.uploadHandler = uploadHandler;

            yield return Timing.WaitUntilDone(webRequest.SendWebRequest());

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Log.Error($"Error sending the message: {webRequest.responseCode} - {webRequest.error}");
            }
        }
        
        public static IEnumerator<float> HandleQueue()
        {
            for (;;)
            {
                foreach (var webhook in Queue.Keys.ToList())
                {
                    if (Queue[webhook].Count == 0)
                        Queue.Remove(webhook);
                    
                    var msg = new Message();
                    foreach (var embed in Queue[webhook].ToList())
                    {
                        if (msg.embeds.Count < 10)
                        {
                            msg.embeds.Add(embed);
                            Queue[webhook].Remove(embed);
                        }
                    }
                    yield return Timing.WaitUntilDone(SendMessage(msg, webhook));
                    yield return Timing.WaitForSeconds(5);
                }
                
                if(Queue.Keys.Count == 0)
                    yield break;
            }
        }
        
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
        
        public class EmbedImage
        {
            public EmbedImage(string url)
            {
                this.url = url;
            }
            
            public string url { get; set; }
        }
        
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
}