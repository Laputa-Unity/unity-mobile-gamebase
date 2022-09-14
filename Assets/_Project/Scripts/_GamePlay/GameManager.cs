using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public LevelController LevelController;
    public GameState GameState;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
    void Start()
    {
        DontDestroyOnLoad(this);
        ReturnHome();
    }

    public void PrepareLevel()
    {
        GameState = GameState.PrepareGame;
        LevelController.PrepareLevel();
    }

    public void ReturnHome()
    {
        PrepareLevel();
        
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupBackground>();
        PopupController.Instance.Show<PopupHome>();
    }

    public void ReplayGame()
    {
        PrepareLevel();
        StartGame();
    }

    public void BackLevel()
    {
        Data.CurrentLevel--;
        
        PrepareLevel();
        StartGame();
    }

    public void NextLevel()
    {
        Data.CurrentLevel++;

        PrepareLevel();
        StartGame();
    }
    
    public void StartGame()
    {
        GameState = GameState.PlayingGame;
        
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupInGame>();
        LevelController.Instance.CurrentLevel.gameObject.SetActive(true);
    }

    public void OnWinGame(float delayPopupShowTime = 2.5f)
    {
        if (GameState == GameState.LoseGame || GameState == GameState.WinGame) return;
        GameState = GameState.WinGame;
        
        SoundController.Instance.PlayFX(SoundType.LevelCompleted);
        Data.CurrentLevel++;

        LevelController.OnWinGame();

        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            PopupController.Instance.Hide<PopupInGame>();
            PopupController.Instance.Show<PopupWin>();
        });
    }
    
    public void OnLoseGame(float delayPopupShowTime = 2.5f)
    {
        if (GameState == GameState.LoseGame || GameState == GameState.WinGame) return;
        GameState = GameState.LoseGame;

        SoundController.Instance.PlayFX(SoundType.LevelFailed);
        
        LevelController.OnLoseGame();
        
        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            PopupController.Instance.Hide<PopupInGame>();
            PopupController.Instance.Show<PopupLose>();
        });
    }
}

public enum GameState
{
    PrepareGame,
    PlayingGame,
    LoseGame,
    WinGame,
}