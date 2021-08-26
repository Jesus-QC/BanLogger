using MEC;
using Utf8Json;
using System.Linq;
using Exiled.API.Features;
using UnityEngine.Networking;
using System.Collections.Generic;
using BanLogger.Features.Discord;

namespace BanLogger.Features
{
    public class DiscordHandler
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
    }
}