using System;

namespace v_rising_discord_bot_companion.query;

public class AsyncQuery(
    Func<object> action
) {

    public Status Status { get; private set; } = Status.PENDING;
    public object? Data { get; private set; }
    public Exception? Exception { get; private set; }

    public void Invoke() {
        try {
            Data = action();
            Status = Status.SUCCESSFUL;
        } catch (Exception e) {
            Exception = e;
            Status = Status.FAILED;
        }
    }
}
