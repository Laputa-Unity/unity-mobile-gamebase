using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Utility
{
    public static void RemoveChildrenGameObject(this Transform transform)
    {
        var children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
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
            return -1;
        }

        return -1;
    }
}
