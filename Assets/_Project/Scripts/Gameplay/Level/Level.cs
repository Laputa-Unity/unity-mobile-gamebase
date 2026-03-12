using System;
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
    [Header("Layer")] [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask holeLayer;

    private Camera _mainCamera;
    private bool _isFingerDown;
    private Pill _currentDragPill;

    private void Awake()
    {
        _mainCamera = Camera.main;
        Debug.Log(_mainCamera);
    }

#if UNITY_EDITOR
    [Button("Play This Level")]
    public void PlayThisLevel()
    {
        if (Application.isPlaying)
        {
            Data.PlayerData.CurrentLevelIndex = Utility.GetNumberInAString(gameObject.name, "1");
            GameManager.Instance.ReplayGame();
        }
        else
        {
            string path = $"{Application.persistentDataPath}/player_data.json";
            string encryptedLoadData = File.ReadAllText(path);
            string decryptedData = EncryptionHelper.Decrypt(encryptedLoadData);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(decryptedData);
            playerData.CurrentLevelIndex = Utility.GetNumberInAString(gameObject.name, "1");

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented, jsonSettings);
            string encryptedSaveData = EncryptionHelper.Encrypt(jsonData);
            File.WriteAllText(path, encryptedSaveData);

            EditorApplication.isPlaying = true;
        }
    }
#endif

    private void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp += HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= HandleFingerUp;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (!finger.IsOverGui)
        {
            _isFingerDown = true;
            var ray = _mainCamera.ScreenPointToRay(finger.ScreenPosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, targetLayer);
            _currentDragPill = hit.collider ? hit.collider.GetComponentInChildren<Pill>() : null;
        }
    }

    private void HandleFingerUpdate(Lean.Touch.LeanFinger finger)
    {
        if (!_isFingerDown || _currentDragPill == null) return;

        HandleFingerInput(finger);
    }

    private void HandleFingerUp(Lean.Touch.LeanFinger finger)
    {
        _isFingerDown = false;

        if (_currentDragPill != null)
        {
            CheckDropOnHole();
            _currentDragPill = null;
        }
    }

    private void HandleFingerInput(Lean.Touch.LeanFinger finger)
    {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(finger.ScreenPosition);
        worldPos.z = _currentDragPill.transform.position.z;
        _currentDragPill.transform.position = worldPos;
    }

    private void CheckDropOnHole()
    {
        Vector2 pillPos = _currentDragPill.transform.position;

        Collider2D holeCollider = Physics2D.OverlapPoint(pillPos, holeLayer);
        if (holeCollider == null)
            return;

        Hole hole = holeCollider.GetComponentInChildren<Hole>();
        if (hole == null)
            return;

        if (_currentDragPill.PillType == PillType.Red)
        {
            GameManager.Instance.OnWinGame(0);
        }
        else if (_currentDragPill.PillType == PillType.Blue)
        {
            GameManager.Instance.OnLoseGame(0);
        }
    }
}