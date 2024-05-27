using UnityEngine;

[CreateAssetMenu(fileName = "InternetConfig", menuName = "ScriptableObject/InternetConfig")]
public class InternetConfig : ScriptableObject
{
    [Min(.1f)] public float repeatRate = .5f;
}
