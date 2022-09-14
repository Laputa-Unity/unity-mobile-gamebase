#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementBlockExcludeCornersWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementBlockManualConstructionSettings _blockManualConstructionSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockManualConstructionSettings BlockManualConstructionSettings { get { return _blockManualConstructionSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockExcludeCornersWasChangedMessage(ObjectPlacementBlockManualConstructionSettings blockManualConstructionSettings)
            : base(MessageType.ObjectPlacementBlockExcludeCornersWasChanged)
        {
            _blockManualConstructionSettings = blockManualConstructionSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementBlockManualConstructionSettings blockManualConstructionSettings)
        {
            var message = new ObjectPlacementBlockExcludeCornersWasChangedMessage(blockManualConstructionSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementBlockPaddingSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementBlockPaddingSettings _paddingSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockPaddingSettings PaddingSettings { get { return _paddingSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockPaddingSettingsWereChangedMessage(ObjectPlacementBlockPaddingSettings paddingSettings)
            : base(MessageType.ObjectPlacementBlockPaddingSettingsWereChanged)
        {
            _paddingSettings = paddingSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementBlockPaddingSettings paddingSettings)
        {
            var message = new ObjectPlacementBlockPaddingSettingsWereChangedMessage(paddingSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings _automaticRandomHeightAdjustmentSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings AutomaticRandomHeightAdjustmentSettings { get { return _automaticRandomHeightAdjustmentSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage(ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
            : base(MessageType.ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChanged)
        {
            _automaticRandomHeightAdjustmentSettings = automaticRandomHeightAdjustmentSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            var message = new ObjectPlacementBlockAutomaticRandomHeightAdjustmentSettingsWereChangedMessage(automaticRandomHeightAdjustmentSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementBlockHeightAdjustmentSettings _heightAdjustmentSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockHeightAdjustmentSettings HeightAdjustmentSettings { get { return _heightAdjustmentSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage(ObjectPlacementBlockHeightAdjustmentSettings heightAdjustmentSettings)
            : base(MessageType.ObjectPlacementBlockHeightAdjustmentModeWasChanged)
        {
            _heightAdjustmentSettings = heightAdjustmentSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementBlockHeightAdjustmentSettings heightAdjustmentSettings)
        {
            var message = new ObjectPlacementBlockHeightAdjustmentModeWasChangedMessage(heightAdjustmentSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementBlockSubdivisionSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementBlockSubdivisionSettings _subdivisionSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementBlockSubdivisionSettings SubdivisionSettings { get { return _subdivisionSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockSubdivisionSettingsWereChangedMessage(ObjectPlacementBlockSubdivisionSettings subdivisionSettings)
            : base(MessageType.ObjectPlacementBlockSubdivisionSettingsWereChanged)
        {
            _subdivisionSettings = subdivisionSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementBlockSubdivisionSettings subdivisionSettings)
        {
            var message = new ObjectPlacementBlockSubdivisionSettingsWereChangedMessage(subdivisionSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif
