using UnityEngine;

[CreateAssetMenu(fileName = "GUIDE", menuName = "ScriptableObject/GUIDE")]
public class PROJECT_GUIDE : ScriptableObject
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