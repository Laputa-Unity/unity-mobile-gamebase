using UnityEngine;

public class BonusArrowHandler : MonoBehaviour
{
    public AreaItem currentAreaItem;
    public GoMove MoveObject => GetComponent<GoMove>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BonusArea"))
        {
            if (currentAreaItem != null)
            {
                currentAreaItem.DeActivateBorderLight();
            }
            currentAreaItem = other.GetComponent<AreaItem>();
            currentAreaItem.ActivateBorderLight();
        }
    }
}