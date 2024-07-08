using TMPro;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class PopupSetting : Popup
{
    public GameObject btnRestorePurchased;

    void Start()
    {
        #if UNITY_ANDROID
            btnRestorePurchased.SetActive(false);
        #endif
    }

    public void OnClickBack()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Hide();
    }
    
    public void OnClickRestorePurchase()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        #if UNITY_IOS
            IAPManager.Instance.RestorePurchase();
        #endif
    }
}