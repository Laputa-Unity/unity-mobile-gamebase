using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;

    public void SetLineRenderState(bool active)
    {
        trailRenderer.enabled = active;
    }
}
