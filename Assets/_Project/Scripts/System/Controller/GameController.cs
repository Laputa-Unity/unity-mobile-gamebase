using UnityEngine;

public class GameController : SingletonDontDestroy<GameController>
{
    public static bool IsTesting;
    [SerializeField] private GameConfig gameConfig;

    protected override void Awake()
    {
        base.Awake();
        IsTesting = gameConfig.isTesting;
    }
}
