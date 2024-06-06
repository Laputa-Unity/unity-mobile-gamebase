using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using UnityEngine;

public class LevelController : SingletonDontDestroy<LevelController>
{
    [SerializeField] private LevelConfig levelConfig;
    [ReadOnly] public Level currentLevel;
    
    private Stack<Level> _stackLevel = new Stack<Level>();
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
    }

    public Level GetLevelByIndex(int indexLevel)
    {
        if (indexLevel >= levelConfig.levels.Count)
        {
            switch (levelConfig.levelLoopType)
            {
                case LevelLoopType.Recycle:
                    var newIndexLevel = indexLevel - levelConfig.levels.Count;
                    return levelConfig.loopLevels[newIndexLevel % levelConfig.loopLevels.Count];
                case LevelLoopType.Random:
                    return levelConfig.loopLevels[Random.Range(0, levelConfig.loopLevels.Count)];
                case LevelLoopType.RandomNoRepeat:
                    if (_stackLevel == null || _stackLevel.Count == 0)
                    {
                        _stackLevel = new Stack<Level>(levelConfig.loopLevels.OrderBy( x => Random.value));
                        return _stackLevel.Pop();
                    }

                    return _stackLevel.Pop();
            }
           
        }
        return levelConfig.levels[indexLevel];
    }
}

