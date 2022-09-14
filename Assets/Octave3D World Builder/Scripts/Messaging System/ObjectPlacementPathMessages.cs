#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathExcludeCornersWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathManualConstructionSettings _pathManualConstructionSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathManualConstructionSettings PathManualConstructionSettings { get { return _pathManualConstructionSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathExcludeCornersWasChangedMessage(ObjectPlacementPathManualConstructionSettings pathManualConstructionSettings)
            : base(MessageType.ObjectPlacementPathExcludeCornersWasChanged)
        {
            _pathManualConstructionSettings = pathManualConstructionSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathManualConstructionSettings pathManualConstructionSettings)
        {
            var message = new ObjectPlacementPathExcludeCornersWasChangedMessage(pathManualConstructionSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathManualConstructionSettings _pathManualConstructionSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathManualConstructionSettings PathManualConstructionSettings { get { return _pathManualConstructionSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage(ObjectPlacementPathManualConstructionSettings pathManualConstructionSettings)
            : base(MessageType.ObjectPlacementPathRotateObjectsToFollowPathWasChanged)
        {
            _pathManualConstructionSettings = pathManualConstructionSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathManualConstructionSettings pathManualConstructionSettings)
        {
            var message = new ObjectPlacementPathRotateObjectsToFollowPathWasChangedMessage(pathManualConstructionSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathPaddingSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathPaddingSettings _paddingSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathPaddingSettings PaddingSettings { get { return _paddingSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathPaddingSettingsWereChangedMessage(ObjectPlacementPathPaddingSettings paddingSettings)
            : base(MessageType.ObjectPlacementPathPaddingSettingsWereChanged)
        {
            _paddingSettings = paddingSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathPaddingSettings paddingSettings)
        {
            var message = new ObjectPlacementPathPaddingSettingsWereChangedMessage(paddingSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathBorderSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathBorderSettings _borderSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathBorderSettings BorderSettings { get { return _borderSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathBorderSettingsWereChangedMessage(ObjectPlacementPathBorderSettings borderSettings)
            : base(MessageType.ObjectPlacementPathBorderSettingsWereChanged)
        {
            _borderSettings = borderSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathBorderSettings borderSettings)
        {
            var message = new ObjectPlacementPathBorderSettingsWereChangedMessage(borderSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathHeightAdjustmentModeWasChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathHeightAdjustmentSettings _heightAdjustmentSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathHeightAdjustmentSettings HeightAdjustmentSettings { get { return _heightAdjustmentSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathHeightAdjustmentModeWasChangedMessage(ObjectPlacementPathHeightAdjustmentSettings heightAdjustmentSettings)
            : base(MessageType.ObjectPlacementPathHeightAdjustmentModeWasChanged)
        {
            _heightAdjustmentSettings = heightAdjustmentSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathHeightAdjustmentSettings heightAdjustmentSettings)
        {
            var message = new ObjectPlacementPathHeightAdjustmentModeWasChangedMessage(heightAdjustmentSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings _automaticRandomHeightAdjustmentSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings AutomaticRandomHeightAdjustmentSettings { get { return _automaticRandomHeightAdjustmentSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChangedMessage(ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
            : base(MessageType.ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChanged)
        {
            _automaticRandomHeightAdjustmentSettings = automaticRandomHeightAdjustmentSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathAutomaticRandomHeightAdjustmentSettings automaticRandomHeightAdjustmentSettings)
        {
            var message = new ObjectPlacementPathAutomaticRandomHeightAdjustmentSettingsWereChangedMessage(automaticRandomHeightAdjustmentSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }

    public class ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage : Message
    {
        #region Private Variables
        private ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings _automaticPatternHeightAdjustmentSettings;
        #endregion

        #region Public Properties
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings AutomaticPatternHeightAdjustmentSettings { get { return _automaticPatternHeightAdjustmentSettings; } }
        #endregion

        #region Constructors
        public ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage(ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings automaticPatternHeightAdjustmentSettings)
            : base(MessageType.ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChanged)
        {
            _automaticPatternHeightAdjustmentSettings = automaticPatternHeightAdjustmentSettings;
        }
        #endregion

        #region Public Static Functions
        public static void SendToInterestedListeners(ObjectPlacementPathAutomaticPatternHeightAdjustmentSettings automaticPatternHeightAdjustmentSettings)
        {
            var message = new ObjectPlacementPathAutomaticPatternHeightAdjustmentSettingsWereChangedMessage(automaticPatternHeightAdjustmentSettings);
            MessageListenerDatabase.Instance.SendMessageToInterestedListeners(message);
        }
        #endregion
    }
}
#endif