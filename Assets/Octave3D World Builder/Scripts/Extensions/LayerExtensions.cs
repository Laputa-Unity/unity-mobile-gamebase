#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public static class LayerExtensions
{
    #region Utilities
    public static int GetDefaultLayer()
    {
        return 0;
    }

    public static int GetMinLayerNumber()
    {
        return 0;
    }

    public static int GetMaxLayerNumber()
    {
        return 31;
    }

    public static bool IsLayerNumberValid(int layerNumber)
    {
        return layerNumber >= GetMinLayerNumber() && layerNumber <= GetMaxLayerNumber();
    }

    public static List<string> GetAllAvailableLayerNames()
    {
        int minLayerNumber = GetMinLayerNumber();
        int maxLayerNumber = GetMaxLayerNumber();

        var layerNames = new List<string>();
        for (int layerIndex = minLayerNumber; layerIndex <= maxLayerNumber; ++layerIndex)
        {
            if (DoesLayerExist(layerIndex)) layerNames.Add(LayerMask.LayerToName(layerIndex));
        }

        return layerNames;
    }

    public static List<int> GetAllAvailableLayers()
    {
        int minLayerNumber = GetMinLayerNumber();
        int maxLayerNumber = GetMaxLayerNumber();

        var layerNumbers = new List<int>();
        for (int layerIndex = minLayerNumber; layerIndex <= maxLayerNumber; ++layerIndex)
        {
            if (DoesLayerExist(layerIndex)) layerNumbers.Add(layerIndex);
        }

        return layerNumbers;
    }

    public static bool DoesLayerExist(int layerNumber)
    {
        string layerName = LayerMask.LayerToName(layerNumber);
        return !string.IsNullOrEmpty(layerName);
    }

    public static int ClearMaskOfInvalidLayers(int layerMask)
    {
        int minLayerNumber = GetMinLayerNumber();
        int maxLayerNumber = GetMaxLayerNumber();

        int newLayerMask = layerMask;
        for (int layerIndex = minLayerNumber; layerIndex <= maxLayerNumber; ++layerIndex)
        {
            if (!DoesLayerExist(layerIndex)) newLayerMask &= ~(1 << layerIndex);
        }

        return newLayerMask;
    }
    #endregion
}
#endif