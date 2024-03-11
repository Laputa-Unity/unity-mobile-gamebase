using System;
using System.Collections;
using CustomInspector;
using UnityEngine;

public class InternetController : SingletonDontDestroy<InternetController>
{
    [SerializeField] private InternetConfig internetConfig;
    [ReadOnly] public bool isConnected;

    private const float TimeStart = 0f;

    void Start()
    {
        var repeatRate = internetConfig.repeatRate;
        InvokeRepeating(nameof(CheckInternet), TimeStart, repeatRate);
    }

    private void CheckInternet()
    {
        StartCoroutine(CheckInternetConnection((isConnect) =>
        {
            isConnected = isConnect;
        }));
    }

    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        action(www.error == null);
    }
}
