using System.Threading.Tasks;
using Lean.Pool;
using CustomTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MoneyHandler : SingletonDontDestroy<MoneyHandler>
{
    [SerializeField] private int numberObject;
    [SerializeField] private int delay;
    [SerializeField] private float durationNear;
    [SerializeField] private float durationTarget;
    [SerializeField] private Ease easeNear;
    [SerializeField] private Ease easeTarget;
    [SerializeField] private float scale = 1;

    [SerializeField] private Vector3? from;
    [SerializeField] private GameObject bar;
    [SerializeField] private GameObject target;
    [SerializeField] private Money moneyPrefab;
    [SerializeField] private TextMeshProUGUI currencyAmountText;
    [SerializeField] private PopupUI popupUI;
    
    private int _moneyCache;
    private int _canvasOrderInLayer;

    private void Start()
    {
        Observer.MoneyChanged += OnMoneyChanged;

        _canvasOrderInLayer = GetComponentInParent<Canvas>().sortingOrder;
    }
    
    private void OnDestroy()
    {
        Observer.MoneyChanged -= OnMoneyChanged;
    }

    private void OnEnable()
    {
        ResetCache();
    }
    
    void ResetCache()
    {
        _moneyCache = Data.PlayerData.CurrentMoney;
        currencyAmountText.text = $"{_moneyCache}";
    }

    private void OnMoneyChanged(int moneyAmount)
    {
        bool isPopupUIActive = popupUI.isActiveAndEnabled;
        if (!isPopupUIActive) popupUI.Show();
        
        if (moneyAmount >= 0)
        {
            IncreaseMoney(moneyAmount);
        }
        else
        {
            DecreaseMoney(moneyAmount);
        }
    }
    
    private void IncreaseMoney(int moneyAmount)
    {
        GenerateMoney(moneyAmount);
    }

    private async void DecreaseMoney(int moneyAmount)
    {
        int moneyPerStep = moneyAmount/numberObject;
        
        for (int i = 0; i < numberObject; i++)
        {
            await Task.Delay(Random.Range(0, delay));
            _moneyCache += moneyPerStep;
            _moneyCache = Mathf.Max(0, _moneyCache);
            currencyAmountText.text = $"{_moneyCache}";
            if (_moneyCache == 0)
            {
                return;
            }
        }
    }

    public void SetFrom(Vector3 from)
    {
        this.from = from;
    }
    
    public void SetToGameObject(GameObject targetGo)
    {
        target = targetGo;
    }

    public async void GenerateMoney(int moneyAmount)
    {
        int moneyPerStep = moneyAmount/numberObject;
        
        SoundController.Instance.PlayFX(SoundName.SpawnCoin);
        for (int i = 0; i < numberObject; i++)
        {
            await Task.Delay(Random.Range(0, delay));
            Money money = LeanPool.Spawn(moneyPrefab, transform);
            money.transform.localScale = Vector3.one * scale;
            if (from != null)
            {
                money.transform.position = from.Value;
            }
            else
            {
                money.transform.localPosition = Vector3.zero;
            }
            
            money.SetTrailOrderInLayer(_canvasOrderInLayer);
            money.SetTrailState(false);
            MoveToNear(money.gameObject).OnComplete(() =>
            {
                money.SetTrailState(true);
                MoveToTarget(money.gameObject).OnComplete(() =>
                {
                    SoundController.Instance.PlayFX(SoundName.CollectCoin);
                    VisualEffectsController.Instance.SpawnEffect(EffectName.SparkCoin, Vector3.zero, target.transform, .5f);
                    LeanPool.Despawn(money);
                    
                    _moneyCache += moneyPerStep;
                    currencyAmountText.text = $"{_moneyCache}";
                    from = null;

                    Tween.PunchScale(bar.transform, Vector3.one, .2f,1,easeBetweenShakes: Ease.Linear);
                });
            });
        }
    }
    
    private Tween MoveTo(Vector3 endValue, GameObject money, float duration, Ease ease)
    {
        return Tween.Position(money.transform,  endValue, duration, ease);
    }

    private Tween MoveToNear(GameObject money)
    {
        return MoveTo(money.transform.position + (Vector3)Random.insideUnitCircle*3, money, durationNear, easeNear);
    }

    private Tween MoveToTarget(GameObject money)
    {
        return MoveTo(target.transform.position, money, durationTarget, easeTarget);
    }
    
    public void SetNumberCoin(int money)
    {
        numberObject = money;
    }
}
