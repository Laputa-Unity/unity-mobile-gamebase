using UnityEngine;

public class GameController : SingletonDontDestroy<GameController>
{
    [SerializeField] private GameConfig gameConfig;
    public static bool IsTesting;
    protected override void Awake()
    {
        base.Awake();
        IsTesting = gameConfig.IsTesting;
    }
}
