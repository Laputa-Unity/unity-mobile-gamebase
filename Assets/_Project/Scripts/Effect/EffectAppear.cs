using DG.Tweening;
using UnityEngine;

public class EffectAppear : MonoBehaviour
{
    [Range(0,2f)]public float TimeScale = .7f;
    public Ease EaseType;
    
    private Vector3 CurrentScale;
    public void Awake()
    {
        CurrentScale = transform.localScale;
    }

    public void OnEnable()
    {
        transform.localScale = CurrentScale/10;
        DoEffect();
    }
    
    public void DoEffect()
    {
        if (!gameObject.activeInHierarchy) return;
        transform.DOScale(CurrentScale, TimeScale).SetEase(EaseType);
    }
}