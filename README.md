# V Rising Discord Bot Companion

This BepInEx plugin for V Rising Servers adds additional http endpoints to the server's `ServerWebAPISystem`. This
allows you to expose additional information about players, such as the current gear level. Intended to be used
with [v-rising-discord-bot](https://github.com/DarkAtra/v-rising-discord-bot#v-rising-discord-bot-companion-integration).

[Refer to the documentation for details on how to use this mod.](https://vrising.darkatra.dev/bot-companion.html)

## Support

If you have questions or need support, feel free to join [this discord server](https://discord.gg/KcMcYKa6Nt).

## Build

Build the project using the following command:

```
dotnet build --no-restore --configuration Release
```

The resulting dll can be found in the `build` directory.
