using System;
using UnityEngine;

public static partial class Observer
{
    public static Action DebugChanged;
    public static Action<int> MoneyChanged;
    public static Action CurrentLevelChanged;
    public static Action<string, Vector3> Notify;
}
