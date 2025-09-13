using CustomTween;
using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public LevelController levelController;
    public GameState gameState;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = true;
        CustomTweenConfig.warnZeroDuration = false;
    }
    
    void Start()
    {
        ReturnHome();
    }

    public void PlayCurrentLevel(bool ignorePrepareLevel = true)
    {
        if (ignorePrepareLevel) PrepareLevel();
        StartGame();
    }

    public void PrepareLevel()
    {
        gameState = GameState.PrepareGame;
        levelController.PrepareLevel();
    }

    public void ReturnHome()
    {
        PrepareLevel();
        
        SoundController.Instance.PlayBackground(SoundName.Background);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupBackground>();
        PopupController.Instance.Show<PopupHome>();
    }

    public void ReplayGame()
    {
        Observer.ReplayLevel?.Invoke(levelController.currentLevel);
        PrepareLevel();
        StartGame();
    }

    public void BackLevel()
    {
        Data.PlayerData.CurrentLevelIndex--;
        
        PrepareLevel();
        StartGame();
    }

    public void NextLevel()
    {
        Observer.SkipLevel?.Invoke(levelController.currentLevel);
        Data.PlayerData.CurrentLevelIndex++;

        PrepareLevel();
        StartGame();
    }
    
    public void StartGame()
    {
        gameState = GameState.PlayingGame;
        Observer.StartLevel?.Invoke(levelController.currentLevel);
        
        SoundController.Instance.PlayBackground(SoundName.BackgroundIngame);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupInGame>();
        
        levelController.currentLevel.gameObject.SetActive(true);
    }

    public void OnWinGame(float delayPopupShowTime = 2.5f)
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.WinGame;
        Observer.WinLevel?.Invoke(levelController.currentLevel);
        Data.PlayerData.CurrentLevelIndex++;
        Sequence.Create().ChainDelay(delayPopupShowTime).ChainCallback(() =>
        {
            PopupController.Instance.HideAll();
            if (PopupController.Instance.Get<PopupWin>() is PopupWin popupWin)
            {
                popupWin.Show();
            }
        });
    }
    
    public void OnLoseGame(float delayPopupShowTime = 2.5f)
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.LoseGame;
        Observer.LoseLevel?.Invoke(levelController.currentLevel);
        
        Sequence.Create().ChainDelay(delayPopupShowTime).ChainCallback(() =>
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
    WaitingResult,
    LoseGame,
    WinGame,
}