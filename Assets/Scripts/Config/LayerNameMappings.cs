using System.Collections.Generic;
using UnityEngine;

public enum LayerName
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Water,
    UI,
    Environment,
}

public static class Layers
{
    private static readonly Dictionary<LayerName, string> LayerNameMap = new Dictionary<LayerName, string>
    {
        { LayerName.Default, "Default" },
        { LayerName.TransparentFX, "TransparentFX" },
        { LayerName.IgnoreRaycast, "Ignore Raycast" },
        { LayerName.Water, "Water" },
        { LayerName.UI, "UI" },
        { LayerName.Environment, "Environment" },
    };

    public static string GetName(LayerName layer)
    {
        return LayerNameMap[layer];
    }

    public static LayerMask GetLayer(LayerName layer)
    {
        int layerValue = LayerMask.NameToLayer(LayerNameMap[layer]);
        return 1 << layerValue;
    }

    public static int GetLayerValue(LayerName layer)
    {
        return LayerMask.NameToLayer(LayerNameMap[layer]);
    }
}
