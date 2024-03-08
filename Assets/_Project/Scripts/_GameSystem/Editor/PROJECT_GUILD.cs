using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GUILD", menuName = "ScriptableObject/GUILD")]
public class PROJECT_GUILD : ScriptableObject
{
    public string version = "1.0.0";
    public string projectName = "Laputa's gamebase";
    public TextAsset documentation;

    [System.NonSerialized] private Texture2D _icon;

    public Texture2D Icon
    {
        get
        {
            if (_icon == null)
            {
                _icon = new Texture2D(1, 1);
                _icon = Resources.Load<Texture2D>( "Laputa");
            }
            
            return _icon;
        }
    }
}