using System;
using System.Collections.Generic;
using UnityEngine;

namespace v_rising_discord_bot_companion.query;

public class QueryDispatcher : MonoBehaviour {

    public static QueryDispatcher Instance = null!;

    private readonly Queue<AsyncQuery> _pendingQueries = new();

    public AsyncQuery Dispatch(Func<object> query) {
        var commandResponse = new AsyncQuery(query);
        _pendingQueries.Enqueue(commandResponse);
        return commandResponse;
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (_pendingQueries.Count <= 0) {
            return;
        }
        var pendingCommand = _pendingQueries.Dequeue();
        pendingCommand.Invoke();
    }
}
