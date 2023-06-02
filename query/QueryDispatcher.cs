using System;
using System.Collections.Generic;
using UnityEngine;

namespace v_rising_server_mod_test.query;

public class QueryDispatcher : MonoBehaviour {

    public static QueryDispatcher Instance = null!;

    private readonly Queue<Query> _pendingQueries = new();

    public AsyncQuery<T> Dispatch<T>(Func<T> query) {
        var commandResponse = new AsyncQuery<T>(query);
        _pendingQueries.Enqueue(commandResponse);
        return commandResponse;
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (_pendingQueries.Count > 0) {
            var pendingCommand = _pendingQueries.Dequeue();
            pendingCommand.Invoke();
        }
    }
}
