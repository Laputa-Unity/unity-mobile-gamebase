using CustomInspector;
using UnityEngine;

public class LevelController : SingletonDontDestroy<LevelController>
{
    [SerializeField] private LevelConfig levelConfig;
    [ReadOnly] public Level currentLevel;

    public void PrepareLevel()
    {
        GenerateLevel(Data.PlayerData.CurrentLevelIndex);
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
        currentLevel.name = indexLevel > levelConfig.maxLevel ? $"Level {indexLevel} - {currentLevel.name}" : $"Level {indexLevel}";
    }

    public Level GetLevelByIndex(int indexLevel)
    {
        if (indexLevel >= levelConfig.maxLevel)
        {
            switch (levelConfig.levelLoopType)
            {
                case LevelLoopType.Recycle:
                    indexLevel = (indexLevel - levelConfig.startLoopLevel) % (levelConfig.maxLevel - levelConfig.startLoopLevel + 1) + levelConfig.startLoopLevel;
                    break;
                case LevelLoopType.Random:
                    indexLevel = Random.Range(1, levelConfig.maxLevel + 1);
                    break;
            }
        }
        else
        {
            indexLevel = (indexLevel - 1) % levelConfig.maxLevel + 1;
        }

        if (Resources.Load($"Levels/Level {indexLevel}") is GameObject levelGo) return levelGo.GetComponent<Level>();
        return null;
    }
}