using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfomationAreaItem : MonoBehaviour
{
    [SerializeField] private GameObject group;

    public void OnClickTitleFrame()
    {
        group.SetActive(!group.activeSelf);
    }
}
