# V Rising Discord Bot Companion

This BepInEx plugin for V Rising Servers adds additional http endpoints to the server's `ServerWebAPISystem`. This
allows you to expose additional information about players, such as the current gear level. Intended to be used
with [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot#v-rising-discord-bot-companion-integration).

It is recommended to **not** expose the server's api port to the internet.

## Endpoints

### `/v-rising-discord-bot/characters`

Returns information about all characters that exist on the server. Intended to be used in conjunction with
the [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot) to display the gear level for all characters.

#### Example Response

```http
HTTP/1.1 200 OK
Transfer-Encoding: chunked
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

### `/v-rising-discord-bot/player-activities`

Returns a list of connect and disconnect events for the last 10 minutes. Intended to be used in conjunction with
the [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot) to log connect and disconnect messages on discord.

Note that this activity list is not persistent across server restarts.

#### Example Response

```http
HTTP/1.1 200 OK
Transfer-Encoding: chunked
Content-Type: application/json

[
  {
    "type": "CONNECTED",
    "playerName": "Atra",
    "occurred": "2023-01-01T00:00:00Z"
  },
  {
    "type": "DISCONNECTED",
    "playerName": "Atra",
    "occurred": "2023-01-01T01:00:00Z"
  },
]
```

## Installing this BepInEx plugin on your V Rising Server

Please note that modding support for V Rising Gloomrot is still experimental. Proceed at your own risk.

1. Validate that the server has it's api enabled and a port configured. **It is not recommended to expose the api port to the internet.**
2. [Install the latest version of BepInEx](https://github.com/decaprime/VRising-Modding/releases/tag/1.668.5) on your Gloomrot V Rising server.
3. Download the [v-rising-discord-bot-companion.dll](https://github.com/DarkAtra/v-rising-discord-bot-companion/releases/tag/v0.1.6) or build it yourself by
   cloning this repository and running `dotnet build`.
4. Download the [Bloodstone.dll](https://github.com/decaprime/Bloodstone/releases/tag/v0.1.4).
5. Move all dlls into your server's BepInEx `plugins` folder.
6. Start the server and test if the new endpoint is functional by executing the following command in the
   terminal: `curl http://<your-server-hostname-here>:<your-api-port-here>/v-rising-discord-bot/characters`. Validate that the returned status code is 200 as
   soon as the server has fully started.
