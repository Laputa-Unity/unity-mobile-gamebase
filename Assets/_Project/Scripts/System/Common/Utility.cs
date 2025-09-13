using System;
using System.Collections;
using System.Text.RegularExpressions;
using Lean.Pool;
using UnityEngine;

public static class Utility
{
    public static string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

    public static void Clear(this Transform transform)
    {
        var children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject, true);
        }
    }
    
    public static void LeanPoolClear(this Transform transform)
    {
        var children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            LeanPool.Despawn(transform.GetChild(i).gameObject);
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
            if(tr.CompareTag(tag))
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }

    public static int GetNumberInAString(string str)
    {
        try
        {
            var getNumb = Regex.Match(str, @"\d+").Value;
            return Int32.Parse(getNumb);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return -1;
        }
    }
    
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    
    public static IEnumerator WaitAFrame(Action onEnd)
    {
        yield return null;
        onEnd?.Invoke();
    }
}
