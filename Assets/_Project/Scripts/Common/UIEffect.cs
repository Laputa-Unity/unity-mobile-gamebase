using PrimeTween;
using UnityEngine;

public class UIEffect : MonoBehaviour
{
    [Header("Data config")]
    [SerializeField] private AnimType animType;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private float animTime = .5f;
    [SerializeField] private float delayAnimTime;
    
    [SerializeField] private Vector3 fromScale = Vector3.zero;
    private Vector3 saveLocalScale; 
    [Header("Shake Effect")]
    [SerializeField] private float strength = 3f;
    [Header("Move Effect")] 
    private MoveType _moveType;
    [SerializeField] private Vector3 fromPosition;
    [SerializeField] private DirectionType directionType;
    [SerializeField] private float offset;
    private Vector3 _saveAnchorPosition;

    private RectTransform _rectTransform;
    private Sequence _sequence;

    private bool IsShowAttributeFromPosition => animType == AnimType.Move && _moveType == MoveType.Vector3;
    private bool IsShowAttributesMoveDirection => animType == AnimType.Move && _moveType == MoveType.Direction;

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _saveAnchorPosition = _rectTransform.anchoredPosition;
        saveLocalScale = _rectTransform.localScale;
    }

    public void OnEnable()
    {
        if (playOnAwake)
        {
            PlayAnim();
        }
    }

    public void PlayAnim()
    {
        switch (animType)
        {
            case AnimType.OutBack:
                _sequence = Sequence.Create().ChainDelay(delayAnimTime).ChainCallback(()=>transform.localScale = fromScale).Chain(Tween.Scale(transform, Vector3.one, animTime,Ease.OutBack));
                break; ;
        }
    }

    public void OnDisable()
    {
        Reset();
        _sequence.Stop();
    }
    
    
    public void Reset()
    {
        if (!Application.isPlaying) return;
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = _saveAnchorPosition;
        _rectTransform.localScale = saveLocalScale;
    }
}

public enum AnimType
{
    OutBack,
    Shake,
    Move,
}

public enum MoveType
{
    Vector3,
    Direction,
}

public enum DirectionType
{
    Up,
    Down,
    Left,
    Right,
}