﻿using System.Collections.Generic;
using Unity.Collections;

namespace v_rising_server_mod_test;

public static class ListUtils {

    public static List<T> Convert<T>(Il2CppSystem.Collections.Generic.IList<T> list) {
        return Convert(list.Cast<Il2CppSystem.Collections.Generic.List<T>>());
    }

    public static List<T> Convert<T>(Il2CppSystem.Collections.Generic.List<T> list) {
        var _list = new List<T>();
        foreach (var arg in list) {
            _list.Add(arg);
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