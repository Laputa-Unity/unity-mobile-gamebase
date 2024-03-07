using System;
using CustomInspector;
using UnityEngine;

public class Validators_InfoBoxSample : ScriptableObject
{
    [Title("InfoBox Message Types")]
    [InfoBox("Default info box")]
    public int a;

    [InfoBox("None info box", CustomMessageType.None)]
    public int b;

    [InfoBox("Warning info box", CustomMessageType.Warning)]
    public int c;

    [InfoBox("Error info box", CustomMessageType.Error)]
    public int d;

    [InfoBox("$" + nameof(DynamicInfo), visibleIf: nameof(VisibleInEditMode))]
    public Vector3 vec;

    private string DynamicInfo => "Dynamic info box: " + DateTime.Now.ToLongTimeString();

    private bool VisibleInEditMode => !Application.isPlaying;
}