using UnityEngine;

[CreateAssetMenu(fileName = "EnergyConfig", menuName = "ScriptableObject/EnergyConfig")]
public class EnergyConfig : ScriptableObject
{
    public int maxEnergy = 30;
    public int refillTimeInSeconds = 300;
}
