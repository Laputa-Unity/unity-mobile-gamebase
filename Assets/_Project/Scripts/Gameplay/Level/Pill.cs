using UnityEngine;

public class Pill : MonoBehaviour
{
    [SerializeField] private PillType pillType;

    public PillType PillType
    {
        get => pillType;
        set => pillType = value;
    }
}

public enum PillType
{
    Blue,
    Red
}
