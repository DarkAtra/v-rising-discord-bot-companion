using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace v_rising_discord_bot_companion;

public static class ListUtils {

    public static List<T> Convert<T>(DynamicBuffer<T> buffer) where T : unmanaged {
        var _list = new List<T>();
        foreach (var entity in buffer) {
            _list.Add(entity);
        }
        return _list;
    }

    public static List<T> Convert<T>(NativeArray<T> array) where T : unmanaged {
        var _list = new List<T>();
        foreach (var entity in array) {
            _list.Add(entity);
        }
        return _list;
    }
}
