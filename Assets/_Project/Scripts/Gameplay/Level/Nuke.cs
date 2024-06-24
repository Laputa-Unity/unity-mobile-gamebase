using Lean.Pool;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    private bool _isLastNuke;

    public void Setup(bool isLasNuke)
    {
        _isLastNuke = isLasNuke;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        LeanPool.Despawn(gameObject);
        VibrationController.Instance.HapticLight();
        SoundController.Instance.PlayFX(SoundName.NukeExplosion);
        VisualEffectsController.Instance.SpawnEffect(EffectName.NukeExplosion, other.GetContact(0).point, LevelController.Instance.currentLevel.transform, 1.5f);
        
        if (other.gameObject.CompareTag("Ground") && _isLastNuke)
        {
            GameManager.Instance.OnWinGame();
        }
        RefreshAfterPool();
    }

    private void RefreshAfterPool()
    {
        _isLastNuke = false;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(90, 0, 0);
    }
}
