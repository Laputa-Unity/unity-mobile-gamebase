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

    private void FixedUpdate()
    {
        if (GameState == GameState.PlayingGame)
        {
            AdsManager.TotalTimesPlay += Time.deltaTime;
        }
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
        FirebaseManager.OnStartLevel(Data.CurrentLevel,LevelController.Instance.CurrentLevel.gameObject.name);
        
        GameState = GameState.PlayingGame;
        
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupInGame>();
        LevelController.Instance.CurrentLevel.gameObject.SetActive(true);
    }

    public void OnWinGame(float delayPopupShowTime = 2.5f)
    {
        if (GameState == GameState.LoseGame || GameState == GameState.WinGame) return;
        GameState = GameState.WinGame;
        
        FirebaseManager.OnWinGame(Data.CurrentLevel,LevelController.Instance.CurrentLevel.gameObject.name);
        AdsManager.TotalLevelWinLose++;
        Data.CurrentLevel++;
        
        LevelController.OnWinGame();
        
        //SoundController.Instance.PlayFX(SoundType.FinishLevel);

        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            //SoundController.Instance.PlayFX(SoundType.Congrats);
            //PopupController.Instance.Hide<PopupIngame>();
            PopupController.Instance.Show<PopupWin>();
        });
    }
    
    public void OnLoseGame(float delayPopupShowTime = 2.5f)
    {
        if (GameState == GameState.LoseGame || GameState == GameState.WinGame) return;
        GameState = GameState.LoseGame;
        
        FirebaseManager.OnLoseGame(Data.CurrentLevel,LevelController.Instance.CurrentLevel.gameObject.name);
        AdsManager.TotalLevelWinLose++;
        
        LevelController.OnLoseGame();
        //SoundController.Instance.PlayFX(SoundType.Lose);
        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            //PopupController.Instance.Hide<PopupIngame>();
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