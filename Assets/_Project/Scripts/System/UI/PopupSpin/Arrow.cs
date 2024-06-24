using CustomInspector;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Spin spin;
    [ReadOnly] public SlotItem currentSlotItem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SlotItem"))
        {
            currentSlotItem = other.GetComponent<SlotItem>();
            spin.currentSlotItem = currentSlotItem;
            SoundController.Instance.PlayFX(SoundName.SpinWheel);
        }
    }
}
