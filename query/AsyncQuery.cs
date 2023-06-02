using System;

namespace v_rising_discord_bot_companion.query;

public interface Query {
    public void Invoke();
}

public class AsyncQuery<T> : Query {
    private readonly Func<T> _action;

    public AsyncQuery(Func<T> action) {
        Status = Status.PENDING;
        _action = action;
    }

    public T? Data { get; private set; }
    public Exception? Exception { get; private set; }
    public Status Status { get; private set; }

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
