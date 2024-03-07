using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class LevelController : SingletonDontDestroy<LevelController>
{
    public Level currentLevel;
    private GameConfig Game => ConfigController.Game;
    public void PrepareLevel()
    {
        GenerateLevel(Data.CurrentLevel);
    }
    
    public void GenerateLevel(int indexLevel)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
        }

        if (indexLevel > ConfigController.Game.maxLevel)
        {
            indexLevel = (indexLevel-Game.startLoopLevel) % (Game.maxLevel - Game.startLoopLevel + 1) + Game.startLoopLevel;
        }
        else
        {
            if (Game.levelLoopType == LevelLoopType.NormalLoop)
            {
                indexLevel = (indexLevel-1) % ConfigController.Game.maxLevel + 1;
            }
            else if (Game.levelLoopType == LevelLoopType.RandomLoop)
            {
                indexLevel = UnityEngine.Random.Range(Game.startLoopLevel, Game.maxLevel);
            }
        }

        Level level = GetLevelByIndex(indexLevel);
        currentLevel = Instantiate(level);
        currentLevel.gameObject.SetActive(false);
    }

    public Level GetLevelByIndex(int indexLevel)
    {
        var levelGo = Resources.Load($"Levels/Level {indexLevel}") as GameObject;
        Debug.Assert(levelGo != null, nameof(levelGo) + " != null");
        return levelGo.GetComponent<Level>();
    }
}

