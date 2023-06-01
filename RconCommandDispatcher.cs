using System;
using System.Collections.Generic;
using UnityEngine;

namespace v_rising_server_mod_test;

public class RconCommandDispatcher : MonoBehaviour {

    public static RconCommandDispatcher Instance = null!;

    private readonly Queue<AsyncRconCommandResponse> pendingCommands = new();

    public AsyncRconCommandResponse Dispatch(string command, List<string> args, Func<string, List<string>, string> action) {
        var commandResponse = new AsyncRconCommandResponse(() => action(command, args));
        pendingCommands.Enqueue(commandResponse);
        return commandResponse;
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (pendingCommands.Count > 0) {
            var pendingCommand = pendingCommands.Dequeue();
            pendingCommand.Invoke();
        }
    }
}
