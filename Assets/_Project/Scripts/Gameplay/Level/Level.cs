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
    [SerializeField] private Environment environment;
    [SerializeField] private Board board;
    private Camera Camera => GetComponentInChildren<Camera>(true);

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
      
    }
}