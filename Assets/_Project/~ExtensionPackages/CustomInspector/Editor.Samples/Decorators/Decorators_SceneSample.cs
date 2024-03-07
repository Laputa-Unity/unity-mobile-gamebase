using System.Collections.Generic;
using CustomInspector;
using UnityEngine;

public class Decorators_SceneSample : ScriptableObject
{
    [Scene] public string scene;

    [Scene] public List<string> scenes;
}