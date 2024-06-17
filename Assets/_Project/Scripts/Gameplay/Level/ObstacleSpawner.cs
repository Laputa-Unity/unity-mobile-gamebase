using Lean.Pool;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float delayTime = 1f;


    [SerializeField] private Transform bottomLeftPoint;
    [SerializeField] private Transform topRightPoint;
    [SerializeField] Nuke nukePrefab;

    private int _bombNumber;
    private float _delayCounter;
    private bool _isPause;


    private void Update()
    {
        if (_isPause) return;
        if (_delayCounter <= 0)
        {
            SpawnObstacle();
            _delayCounter = delayTime;
        }
        else
        {
            _delayCounter -= Time.deltaTime;
        }
    }

    public void Setup(int bombNumber)
    {
        _bombNumber = bombNumber;
    }

    public void StartSpawn()
    {
        _isPause = false;
    }

    public void StopSpawn()
    {
        _isPause = true;
    }

    private void SpawnObstacle()
    {
        if (_bombNumber <= 0) return;
        var bomb = LeanPool.Spawn(nukePrefab,
            new Vector3(Random.Range(bottomLeftPoint.position.x, topRightPoint.position.x),
                Random.Range(bottomLeftPoint.position.y, topRightPoint.position.y), 0), Quaternion.Euler(90, 0, 0),
            LevelController.Instance.currentLevel.transform);
        bomb.Setup(_bombNumber == 1);
        _bombNumber -= 1;
    }
}