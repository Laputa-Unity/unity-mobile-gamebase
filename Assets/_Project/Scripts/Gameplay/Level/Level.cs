using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using CustomInspector;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#endif

public class Level : MonoBehaviour
{
    [SerializeField] private int nukeNumber = 10;
    [ReadOnly] public int bonusMoney;

    private Camera Camera => GetComponentInChildren<Camera>(true);
    private ObstacleSpawner _obstacleSpawner;
    private Player _player;

    private bool _isFingerDown;
    private bool _isFingerDrag;
    private float _screenWidth;

#if UNITY_EDITOR
    [Button("Play This Level")]
    public void PlayThisLevel()
    {
        if (Application.isPlaying)
        {
            Data.PlayerData.CurrentLevelIndex = Utility.GetNumberInAString(gameObject.name);
            GameManager.Instance.ReplayGame();
        }
        else
        {
            string path = $"{Application.persistentDataPath}/player_data.json";
            string encryptedLoadData = File.ReadAllText(path);
            string decryptedData = EncryptionHelper.Decrypt(encryptedLoadData);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(decryptedData);
            playerData.CurrentLevelIndex = Utility.GetNumberInAString(gameObject.name);
            var jsonSettings = new JsonSerializerSettings
                { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented, jsonSettings);
            string encryptedSaveData = EncryptionHelper.Encrypt(jsonData);
            File.WriteAllText(path, encryptedSaveData);
            EditorApplication.isPlaying = true;
        }
    }
#endif

    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp += HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (!finger.IsOverGui)
        {
            _isFingerDown = true;

            //Get Object raycast hit
            var ray = finger.GetRay(Camera);
            var hit = default(RaycastHit);

            if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
            {
                //ADDED LAYER SELECTION
                Debug.Log(hit.collider.gameObject);
            }
        }
    }

    void HandleFingerUp(Lean.Touch.LeanFinger finger)
    {
        _isFingerDown = false;
    }

    void HandleFingerUpdate(Lean.Touch.LeanFinger finger)
    {
        if (_isFingerDown)
        {
            _isFingerDrag = true;
            HandleFingerInput(finger);
        }
    }

    void HandleFingerInput(Lean.Touch.LeanFinger finger)
    {
        if (finger.ScreenPosition.x < _screenWidth / 2)
        {
            _player.MoveLeft();
        }
        else if (finger.ScreenPosition.x > _screenWidth / 2)
        {
            _player.MoveRight();
        }
    }

    private void Awake()
    {
        _obstacleSpawner = GetComponentInChildren<ObstacleSpawner>();
        _player = GetComponentInChildren<Player>();

        _screenWidth = Screen.width;
    }

    private void Start()
    {
        _obstacleSpawner.Setup(nukeNumber);
        _obstacleSpawner.StartSpawn();

        Observer.WinLevel += OnWin;
        Observer.LoseLevel += OnLose;
    }

    private void OnDestroy()
    {
        Observer.WinLevel -= OnWin;
        Observer.LoseLevel -= OnLose;
    }

    public void OnWin(Level level)
    {
        _obstacleSpawner.StopSpawn();
    }

    public void OnLose(Level level)
    {
        _obstacleSpawner.StopSpawn();
    }
}