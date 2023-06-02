﻿namespace v_rising_discord_bot_companion.command;

public class Problem {
    public string Type { get; private set; }
    public string Title { get; private set; }

    public Problem(string type, string title) {
        Type = type;
        Title = title;
    }
}