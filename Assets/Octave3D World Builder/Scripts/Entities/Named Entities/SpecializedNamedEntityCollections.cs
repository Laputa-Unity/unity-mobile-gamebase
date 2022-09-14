#if UNITY_EDITOR
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGroupCollection : NamedEntityCollectionWithEntityMarker<ObjectGroup> { }

    [Serializable]
    public class PrefabCategoryCollection : NamedEntityCollectionWithEntityMarker<PrefabCategory> { }

    [Serializable]
    public class PrefabCollection : NamedEntityCollectionWithEntityMarker<Prefab> { }

    [Serializable]
    public class PrefabTagCollection : NamedEntityCollection<PrefabTag> { }

    [Serializable]
    public class ObjectPlacementPathHeightPatternCollection : NamedEntityCollectionWithEntityMarker<ObjectPlacementPathHeightPattern> { }

    [Serializable]
    public class ObjectPlacementPathTileConnectionConfigurationCollection : NamedEntityCollectionWithEntityMarker<ObjectPlacementPathTileConnectionConfiguration> { }

    [Serializable]
    public class DecorPaintObjectPlacementBrushCollection : NamedEntityCollectionWithEntityMarker<DecorPaintObjectPlacementBrush> { }
}
#endif
