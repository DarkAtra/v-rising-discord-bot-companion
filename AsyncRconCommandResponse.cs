using System;

namespace v_rising_server_mod_test;

public class AsyncRconCommandResponse {
    public string? Data { get; private set; }
    public Exception? Exception { get; private set; }
    public Status Status { get; private set; }
    private Func<string> _action;

    public AsyncRconCommandResponse(Func<string> action) {
        Status = Status.PENDING;
        _action = action;
    }

    public void Invoke() {
        try {
            Data = _action();
            Status = Status.SUCCESSFUL;
        }
        catch (Exception e) {
            Exception = e;
            Status = Status.FAILED;
        }
    }
}

public enum Status {
    PENDING,
    SUCCESSFUL,
    FAILED
}
