using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static void Clear(this Transform transform)
    {
        var childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject, true);
        }
    }

    public static float GetScreenRatio()
    {
        return (1920f / 1080f) / (Screen.height / (float) Screen.width);
    }
    
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag)where T:Component{
        Transform t = parent.transform;
        foreach(Transform tr in t)
        {
            if(tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}
