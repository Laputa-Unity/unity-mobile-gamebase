#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions
{
    #region Extension Methods
    public static void Fill<T>(this List<T> list, T fillValue, int fillAmount)
    {
        if(fillAmount > 0)
        {
            list.Clear();
            for(int valueIndex = 0; valueIndex < fillAmount; ++valueIndex)
            {
                list.Add(fillValue);
            }
        }
    }

    public static void RemoveLast<T>(this List<T> list)
    {
        if (list.Count != 0) list.RemoveAt(list.Count - 1);
    }
    #endregion

    #region Utilities
    public static List<T> GetFilledList<T>(T fillValue, int fillAmount)
    {
        var list = new List<T>(fillAmount);
        list.Fill(fillValue, fillAmount);

        return list;
    }
    #endregion
}
#endif