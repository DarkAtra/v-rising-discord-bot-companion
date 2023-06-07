# V Rising Discord Bot Companion

This BepInEx plugin for V Rising Servers adds additional http endpoints to the server's `ServerWebAPISystem`. This
allows you to expose additional information about players, such as the current gear level.

It is recommended to **not** expose the server's api port to the internet.

## Endpoints

### `/v-rising-discord-bot/characters`

Returns information about all characters that exist on the server. Intended to be used in conjunction with
the [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot) to display the gear level for all characters.

#### Example Response

```http
HTTP/1.1 200 OK
Server: Mono-HTTPAPI/1.0
Date: Fri, 02 Jun 2023 19:44:16 GMT
Transfer-Encoding: chunked
Keep-Alive: timeout=15,max=100
Content-Type: application/json

[
  {
    "name": "Atra",
    "gearLevel": 83,
    "clan": "Test",
    "killedVBloods": [
      "FOREST_WOLF",
      "BANDIT_STONEBREAKER"
    ]
  },
  {
    "name": "Socium",
    "gearLevel": 84,
    "killedVBloods": []
  }
]
```

## Installing this BepInEx plugin on your V Rising Server

Please note that modding support for V Rising Gloomrot is still experimental. Proceed at your own risk.

1. Validate that the server has it's api enabled and a port configured. **It is not recommended to expose the api port to the internet.**
2. [Install the latest version of BepInEx](https://github.com/decaprime/VRising-Modding/releases/tag/1.668.4) on your Gloomrot V Rising server.
3. Download the [v-rising-discord-bot-companion.dll](https://github.com/DarkAtra/v-rising-discord-bot-companion/releases/tag/v0.1.6) or build it yourself by
   cloning this repository and running `dotnet build`.
4. Download the [Bloodstone.dll](https://github.com/decaprime/Bloodstone/releases/tag/v0.1.4)
   and [VampireCommandFramework.dll](https://github.com/decaprime/VampireCommandFramework/releases/tag/v0.8.0).
5. Move all dlls into your server's BepInEx `plugins` folder.
6. Start the server and test if the new endpoint is functional by executing the following command in the
   terminal: `curl http://localhost:<your-api-port-here>/v-rising-discord-bot/characters`. Validate that the returned status code is 200 as soon as the server has fully
   started.
