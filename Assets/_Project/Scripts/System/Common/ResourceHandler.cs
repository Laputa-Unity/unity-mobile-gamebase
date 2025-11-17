using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lean.Pool;
using CustomTween;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ResourceHandler : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] protected int numberObject = 5;
    [SerializeField] protected int delay = 20;
    [SerializeField] protected float durationNear = .4f;
    [SerializeField] protected float durationTarget = .4f;
    [SerializeField] protected Ease easeNear = Ease.OutQuad;
    [SerializeField] protected Ease easeTarget = Ease.InQuad;
    [SerializeField] protected float scale = 1;

    [Header("Runtime")]
    protected Vector3? from;
    protected Canvas canvas;

    protected List<Resource> visibleResources = new List<Resource>();

    // -------- ABSTRACT: mỗi resource khác nhau ----------
    protected abstract Resource Prefab { get; }
    protected abstract GameObject Target { get; }
    protected abstract TextMeshProUGUI AmountText { get; }
    protected abstract GameObject Bar { get; }
    protected abstract int Cache { get; set; }
    protected abstract int CurrentValue { get; }
   
    // -------- ABSTRACT: event riêng vàng/kim cương -------
    protected abstract void SubscribeEvents();
    protected abstract void UnsubscribeEvents();
    protected abstract void OnCollectedEffect(GameObject target);

    protected void Awake()
    {
        canvas = PopupController.Instance.CanvasTransform.GetComponent<Canvas>();
    }

    protected virtual void OnEnable()
    {
        SubscribeEvents();
        Observer.SpawnResourcesChanged += SpawnResourcesChanged;
    }

    protected virtual void OnDisable()
    {
        UnsubscribeEvents();
        Observer.SpawnResourcesChanged -= SpawnResourcesChanged;
    }

    private void SpawnResourcesChanged(Vector3 position)
    {
        from = position;
    }
    
    protected virtual void Start()
    {
        ResetCache();
    }

    public void ResetCache()
    {
        Cache = CurrentValue;
        AmountText.text = Cache.ToString();
    }

    // ============================================================
    // INCREASE / DECREASE
    // ============================================================

    public void Increase(int value)
    {
        GenerateResource(value);
    }

    public async void Decrease(int amount)
    {
        int step = (amount % numberObject == 0) ? numberObject : 1;
        int perStep = amount / step;

        for (int i = 0; i < step; i++)
        {
            await Task.Delay(Random.Range(0, delay));

            Cache -= perStep;
            Cache = Mathf.Max(0, Cache);
            AmountText.text = Cache.ToString();

            if (Cache == 0) return;
        }
    }

    // ============================================================
    // SPAWN + MOVE
    // ============================================================

    protected async void GenerateResource(int amount)
    {
        if (Target == null) return;

        int objects = 1;

        for (int i = 60; i >= 0; i--)
            if (i > 0 && amount % i == 0)
            {
                objects = i;
                break;
            }

        int perStep = amount / objects;

        for (int i = 0; i < objects; i++)
        {
            await Task.Delay(Random.Range(0, delay));

            Resource r = LeanPool.Spawn(Prefab, PopupController.Instance.CanvasTransform);
            r.transform.localScale = Vector3.one * scale;
            visibleResources.Add(r);

            float zCanvas = canvas.planeDistance;
            Vector3 startPos = from ?? Vector3.zero;
            startPos.z = zCanvas;
            r.transform.position = startPos;
            r.SetTrailOrderInLayer(canvas.sortingOrder);
            r.SetTrailState(false);

            MoveToNear(r.gameObject).OnComplete(() =>
            {
                r.SetTrailState(true);
                MoveToTarget(r.gameObject, Target).OnComplete(() =>
                {
                    LeanPool.Despawn(r);
                    visibleResources.Remove(r);

                    if (visibleResources.Count == 0)
                        from = null;

                    Cache += perStep;
                    AmountText.text = Cache.ToString();
                    
                    OnCollectedEffect(Target);

                    Tween.PunchScale(Bar.transform, Vector3.one, .2f, 1);
                });
            });
        }
    }

    private Tween MoveTo(Vector3 endValue, GameObject obj, float duration, Ease ease)
    {
        return Tween.Position(obj.transform, endValue, duration, ease);
    }

    private Tween MoveToNear(GameObject obj)
    {
        return MoveTo(obj.transform.position + (Vector3)Random.insideUnitCircle * 3, obj, durationNear, easeNear);
    }

    private Tween MoveToTarget(GameObject obj, GameObject target)
    {
        return MoveTo(target.transform.position, obj, durationTarget, easeTarget);
    }
}
