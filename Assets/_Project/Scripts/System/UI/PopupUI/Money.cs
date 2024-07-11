using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;

    public void SetTrailOrderInLayer(int sortingOrder)
    {
        trailRenderer.sortingOrder = sortingOrder;
    }
    
    public void SetTrailState(bool active)
    {
        trailRenderer.enabled = active;
    }
}
