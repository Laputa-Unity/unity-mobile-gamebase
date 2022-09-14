#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPlacementPathTileConnectionTypeTexturePaths
    {
        #region Private Static Variables
        private static string[] _relativeTileConnectionTypeTexturePaths;
        #endregion

        #region Static Constructor
        static ObjectPlacementPathTileConnectionTypeTexturePaths()
        {
            _relativeTileConnectionTypeTexturePaths = new string[ObjectPlacementPathTileConnectionTypes.Count];
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.Begin] = "/Textures/GUI Textures/Tile Connection Buttons/BeginTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.End] = "/Textures/GUI Textures/Tile Connection Buttons/EndTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.Forward] = "/Textures/GUI Textures/Tile Connection Buttons/ForwardTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.TJunction] = "/Textures/GUI Textures/Tile Connection Buttons/TJunctionTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.Cross] = "/Textures/GUI Textures/Tile Connection Buttons/CrossTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.Turn] = "/Textures/GUI Textures/Tile Connection Buttons/TurnTileConnectionButton";
            _relativeTileConnectionTypeTexturePaths[(int)ObjectPlacementPathTileConnectionType.Autofill] = "/Textures/GUI Textures/Tile Connection Buttons/AutofillTileConnectionButton";
        }
        #endregion

        #region Public Static Functions
        public static string GetRelativeTexturePathForTileConnectionType(ObjectPlacementPathTileConnectionType tileConnectionType)
        {
            return _relativeTileConnectionTypeTexturePaths[(int)tileConnectionType];
        }
        #endregion
    }
}
#endif