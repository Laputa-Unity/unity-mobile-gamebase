using System.Threading.Tasks;
using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyHandler : SingletonDontDestroy<MoneyHandler>
{
    [SerializeField] private int numberCoin;
    [SerializeField] private int delay;
    [SerializeField] private float durationNear;
    [SerializeField] private float durationTarget;
    [SerializeField] private Ease easeNear;
    [SerializeField] private Ease easeTarget;
    [SerializeField] private float scale = 1;

    [SerializeField] private Vector3? from;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private TextMeshProUGUI currencyAmountText;
    [SerializeField] private PopupUI popupUI;
    
    private int _moneyCache;

    private void Start()
    {
        Observer.MoneyChanged += OnMoneyChanged;
    }

    private void OnEnable()
    {
        ResetCache();
    }
    
    void ResetCache()
    {
        _moneyCache = Data.MoneyTotal;
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
        GenerateCoin(moneyAmount);
    }

    private async void DecreaseMoney(int moneyAmount)
    {
        int moneyPerStep = moneyAmount/numberCoin;
        
        for (int i = 0; i < numberCoin; i++)
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

    public async void GenerateCoin(int moneyAmount)
    {
        int moneyPerStep = moneyAmount/numberCoin;
        
        for (int i = 0; i < numberCoin; i++)
        {
            await Task.Delay(Random.Range(0, delay));
            GameObject coin = LeanPool.Spawn(coinPrefab, transform);
            coin.transform.localScale = Vector3.one * scale;
            if (from != null)
            {
                coin.transform.position = from.Value;
            }
            else
            {
                coin.transform.localPosition = Vector3.zero;
            }
            
            SoundController.Instance.PlayFX(SoundName.CoinMoving);
            
            MoveToNear(coin).OnComplete(() =>
            {
                MoveToTarget(coin).OnComplete(() =>
                {
                    LeanPool.Despawn(coin);

                    _moneyCache += moneyPerStep;
                    currencyAmountText.text = $"{_moneyCache}";
                    from = null;
                });
            });
        }
    }
    
    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveTo(Vector3 endValue, GameObject coin, float duration, Ease ease)
    {
        return coin.transform.DOMove(endValue, duration).SetEase(ease);
    }

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveToNear(GameObject coin)
    {
        return MoveTo(coin.transform.position + (Vector3)Random.insideUnitCircle*3, coin, durationNear, easeNear);
    }

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> MoveToTarget(GameObject coin)
    {
        return MoveTo(target.transform.position, coin, durationTarget, easeTarget);
    }
    
    public void SetNumberCoin(int coin)
    {
        numberCoin = coin;
    }
}
