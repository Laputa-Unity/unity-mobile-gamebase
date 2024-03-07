using UnityEngine;

public class BonusArrowHandler : MonoBehaviour
{
    public AreaItem CurrentAreaItem;
    public GoMove MoveObject => GetComponent<GoMove>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BonusArea"))
        {
            CurrentAreaItem = other.GetComponent<AreaItem>();
            CurrentAreaItem.ActivateBorderLight();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BonusArea"))
        {
            other.GetComponent<AreaItem>().DeActivateBorderLight();
        }
    }
}