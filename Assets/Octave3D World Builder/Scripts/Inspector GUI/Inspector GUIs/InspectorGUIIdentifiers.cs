#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class InspectorGUIIdentifiers
    {
        #region Private Static Variables
        private static readonly InspectorGUIIdentifier[] _identifiers;
        private static readonly SceneRenderPathType[] _sceneRenderPathTypes;
        private static readonly int _count;
        #endregion

        #region Constructors
        static InspectorGUIIdentifiers()
        {
            _count = Enum.GetValues(typeof(InspectorGUIIdentifier)).Length;

            _identifiers = new InspectorGUIIdentifier[_count];
            InitializeIdentifiersArray();

            _sceneRenderPathTypes = new SceneRenderPathType[_count];
            InitializeSceneRenderPathTypesArray();
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<InspectorGUIIdentifier> GetAll()
        {
            return new List<InspectorGUIIdentifier>(_identifiers);
        }

        public static SceneRenderPathType GetSceneRenderPathTypeFromIdentifier(InspectorGUIIdentifier guiIdentifier)
        {
            return _sceneRenderPathTypes[(int)guiIdentifier];
        }
        #endregion

        #region Private Static Functions
        private static void InitializeIdentifiersArray()
        {
            _identifiers[(int)InspectorGUIIdentifier.ObjectErase] = InspectorGUIIdentifier.ObjectErase;
            _identifiers[(int)InspectorGUIIdentifier.ObjectPlacement] = InspectorGUIIdentifier.ObjectPlacement;
            _identifiers[(int)InspectorGUIIdentifier.ObjectSelection] = InspectorGUIIdentifier.ObjectSelection;
            _identifiers[(int)InspectorGUIIdentifier.ObjectSnapping] = InspectorGUIIdentifier.ObjectSnapping;
        }

        private static void InitializeSceneRenderPathTypesArray()
        {
            _sceneRenderPathTypes[(int)InspectorGUIIdentifier.ObjectErase] = SceneRenderPathType.ObjectErase;
            _sceneRenderPathTypes[(int)InspectorGUIIdentifier.ObjectPlacement] = SceneRenderPathType.ObjectPlacement;
            _sceneRenderPathTypes[(int)InspectorGUIIdentifier.ObjectSelection] = SceneRenderPathType.ObjectSelection;
            _sceneRenderPathTypes[(int)InspectorGUIIdentifier.ObjectSnapping] = SceneRenderPathType.ObjectPlacement;
        }
        #endregion
    }
}
#endif