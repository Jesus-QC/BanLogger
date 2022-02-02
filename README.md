<h4 align="center">
  <img alt="Ban Logger" src="https://user-images.githubusercontent.com/69375249/152147913-23ea3f20-dafb-4133-a1d2-4b13b2926b86.png">
</h4>

***Ban Logger*** This plugin allows you to log bans into any discord channel of your server with the help of webhooks.

Who is **Webhooks** and what is the **Steam API key**? Well, if you don't know what does this mean continue scrolling down.

<img alt="Ban Logger" src="https://cdn.discordapp.com/attachments/812402319467610192/812403037486186506/unknown.png">

---

## Webhooks

First, webhooks are a utiliy used to send messages to text channels without needing a discord application (like bots).

This is how you can *get the channel webhook url* of your server:

1. Click *Edit channel* in the desired server channel.
2. Go to the *integrations* section
3. Click *Create Webhook*, this will create the webhook.
4. **Dont edit the name or avatar of the webhook.** This won't make something in the plugin.
5. Copy the webhook URL with *Copy webhook url* button.

## Steam API key

This plugin uses *Steam API* to get the username of obanned users.

> Steam API Keys are connection iddentifiers needed to get data from steam servers.

You can get your own Steam API key by clicking [`here`](https://steamcommunity.com/dev/apikey)

If you don't have your own Steam API Key you will have to create a new one:


## Installation

I always recommend downloading the file from official plugin [releases url](https://github.com/Jesus-QC/BanLogger/releases/latest/).

However in linux you can install it via commands too so it would be something like this:

    $ cd `exiled plugins folder dir`
    $ wget --no-check-certificate --content-disposition https://github.com/Jesus-QC/BanLogger/releases/latest/download/BanLogger.dll

You will see how `BanLogger.dll` appears in your plugin folder and can be used without problems.
