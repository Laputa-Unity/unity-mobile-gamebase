using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CurrencyGenerate : MonoBehaviour
{
       public GameObject overlay;
    public GameObject coinPrefab;
    public GameObject from;
    public GameObject to;
    public int numberCoin;
    public int delay;
    public float durationNear;
    public float durationTarget;
    public Ease easeNear;
    public Ease easeTarget;
    public float scale = 1;
    private int numberCoinMoveDone;
    private System.Action moveOneCoinDone;
    private System.Action moveAllCoinDone;

    public void SetFromGameObject(GameObject from)
    {
        this.from = from;
    }

    public void SetToGameObject(GameObject to)
    {
        this.to = to;
    }

    private void Start()
    {
        overlay.SetActive(false);
    }

    public async void GenerateCoin(System.Action moveOneCoinDone, System.Action moveAllCoinDone, GameObject from = null, GameObject to = null, int numberCoin = -1)
    {
        this.moveOneCoinDone = moveOneCoinDone;
        this.moveAllCoinDone = moveAllCoinDone;
        this.from = from == null ? this.from : from;
        this.to = to == null ? this.to : to;
        this.numberCoin = numberCoin < 0 ? this.numberCoin : numberCoin;
        numberCoinMoveDone = 0;
        overlay.SetActive(true);
        for (int i = 0; i < this.numberCoin; i++)
        {
            await Task.Delay(Random.Range(0, delay));
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.transform.localScale = Vector3.one * scale;
            coin.transform.position = this.from.transform.position;
            MoveCoin(coin);
        }
    }

    private void MoveCoin(GameObject coin)
    {
        //SoundController.Instance.PlayOnce(SoundType.CoinMove);
        MoveToNear(coin).OnComplete(() =>
        {
            MoveToTarget(coin).OnComplete(() =>
            {
                numberCoinMoveDone++;
                Destroy(coin);
                moveOneCoinDone?.Invoke();
                if (numberCoinMoveDone >= numberCoin)
                {
                    moveAllCoinDone?.Invoke();
                    overlay.SetActive(false);
                }
            });
        });
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
        return MoveTo(to.transform.position, coin, durationTarget, easeTarget);
    }
    
    public void SetNumberCoin(int _numberCoin)
    {
        numberCoin = _numberCoin;
    }
}
