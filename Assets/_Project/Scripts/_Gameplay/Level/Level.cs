using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int bonusMoney;
    
    private bool _isFingerDown;
    private bool _isFingerDrag;

    private Camera Camera => GetComponentInChildren<Camera>(true);
    
    #if UNITY_EDITOR
    private void StartLevel()
    {
        Data.CurrentLevel = Utility.GetNumberInAString(gameObject.name);
        
        EditorApplication.isPlaying = true;
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
        
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity)) { //ADDED LAYER SELECTION
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
        }
    }
    
    private void Start()
    {
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
        
    }

    public void OnLose(Level level)
    {
         
    }
}
