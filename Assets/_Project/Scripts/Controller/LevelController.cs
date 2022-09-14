using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    public Level CurrentLevel;

    public void PrepareLevel()
    {
        GenerateLevel(Data.CurrentLevel);
    }
    
    public void GenerateLevel(int indexLevel)
    {
        if (CurrentLevel != null)
        {
            Destroy(CurrentLevel.gameObject);
        }

        if (indexLevel > ConfigController.Game.MaxLevel)
        {
            indexLevel = (indexLevel-ConfigController.Game.StartLoopLevel) % (ConfigController.Game.MaxLevel - ConfigController.Game.StartLoopLevel+1) + ConfigController.Game.StartLoopLevel;
        }
        else
        {
            indexLevel = (indexLevel-1) % ConfigController.Game.MaxLevel + 1;
        }

        Level level = GetLevelByIndex(indexLevel);
        CurrentLevel = Instantiate(level);
        CurrentLevel.gameObject.SetActive(false);
    }

    public Level GetLevelByIndex(int indexLevel)
    {
        GameObject levelGO;
        levelGO = Resources.Load($"Levels/Level {indexLevel}") as GameObject;
        return levelGO.GetComponent<Level>();
    }
    
    public void OnLoseGame()
    {
        
    }

    public void OnWinGame()
    {
        
    }
        
    public void OnReplay()
    {
        
    }
}