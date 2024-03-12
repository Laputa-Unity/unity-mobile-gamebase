using System;

public static partial class Observer
{
    public static Action DebugChanged;
    public static Action<int> MoneyChanged;
    public static Action CurrentLevelChanged;
}
