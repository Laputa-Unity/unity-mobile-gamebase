using System;
using UnityEngine;

public static partial class Observer
{
    public static Action DebugChanged;
    public static Action<int> EnergyChanged;
    public static Action EnergyChangedDone;
    public static Action<int> GoldChanged;
    public static Action GoldChangedDone;
    public static Action<int> DiamondChanged;
    public static Action DiamondChangedDone;
    public static Action CurrentChapterChanged;
    public static Action<string, Vector3> Notify;
}
