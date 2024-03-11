using CustomInspector;
using UnityEngine;

public class LevelController : SingletonDontDestroy<LevelController>
{
    [SerializeField] private LevelConfig levelConfig;
    [ReadOnly] public Level currentLevel;
    
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

