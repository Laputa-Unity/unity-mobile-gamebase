using Lean.Pool;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private bool _isLastBomb;

    public void Setup(bool isLastBomb)
    {
        _isLastBomb = isLastBomb;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        LeanPool.Despawn(gameObject);
        VisualEffectsController.Instance.SpawnEffect(EffectName.NukeExplosion, other.GetContact(0).point, LevelController.Instance.currentLevel.transform, 1.5f);
        
        if (other.gameObject.CompareTag("Ground") && _isLastBomb)
        {
            GameManager.Instance.OnWinGame();
        }
        RefreshAfterPool();
    }

    private void RefreshAfterPool()
    {
        _isLastBomb = false;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(90, 0, 0);
    }
}
