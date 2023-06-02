# V Rising Discord Bot Companion

This BepInEx plugin for V Rising Servers adds additional http endpoints to the server's `ServerWebAPISystem`. This
allows you to expose additional information about players, such as the current gear level.

## Endpoints

### `/v-rising-discord-bot/characters`

Returns information about all characters that are currently on the server. Intended to be used in conjunction
with the [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot) to display the gear level for all
characters.

#### Example Response

```http
HTTP/1.1 200 OK
Server: Mono-HTTPAPI/1.0
Date: Fri, 02 Jun 2023 19:44:16 GMT
Transfer-Encoding: chunked
Keep-Alive: timeout=15,max=100

[
  {
    "name": "Atra",
    "gearLevel": 83
  },
  {
    "name": "Socium",
    "gearLevel": 84
  }
]
```
