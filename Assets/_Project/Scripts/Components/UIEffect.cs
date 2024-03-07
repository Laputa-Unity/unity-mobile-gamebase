using CustomInspector;
using CustomTween;
using UnityEngine;

public class UIEffect : MonoBehaviour
{
    [Header("Data config")]
    [SerializeField] private AnimType animType;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private float animTime = .5f;
    [SerializeField] private float delayAnimTime;

    [ShowIf("animType",AnimType.OutBack)] [SerializeField] private Vector3 fromScale = Vector3.zero;
    
    private Vector3 _saveLocalScale;
    private Vector3 _saveAnchorPosition;
    private RectTransform _rectTransform;
    private Sequence _sequence;
    private bool _isFirstInstantiate = true;

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _saveAnchorPosition = _rectTransform.anchoredPosition;
        _saveLocalScale = _rectTransform.localScale;
    }

    public void OnEnable()
    {
        if (_isFirstInstantiate)
        {
            _isFirstInstantiate = false;
        }
        else
        {
            if (playOnAwake)
            {
                PlayAnim();
            }
        }
    }

    public void PlayAnim()
    {
        switch (animType)
        {
            case AnimType.OutBack:
                transform.localScale = fromScale;
                _sequence = Sequence.Create().ChainDelay(delayAnimTime)
                    .Chain(Tween.Scale(transform, Vector3.one, animTime, Ease.OutBack));
                break;
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
        _rectTransform.localScale = _saveLocalScale;
    }
}

public enum AnimType
{
    OutBack,
}
