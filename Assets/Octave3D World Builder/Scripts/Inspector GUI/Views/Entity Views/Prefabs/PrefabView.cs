#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabView : EntityView
    {
        #region Private Variables
        [NonSerialized]
        private Prefab _prefab;
        #endregion

        #region Constructors
        public PrefabView(Prefab prefab)
        {
            _prefab = prefab;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderPrefabNameChangeField();
            _prefab.InstantiationSettings.View.Render();
            _prefab.ActivationSettings.View.Render();
            RenderOffsetFromGridSurfaceField();
            RenderOffsetFromObjectSurfaceField();
            RenderSelectPrefabInProjectWindowButton();

            EditorGUILayout.Separator();
            _prefab.TagAssociations.View.Render();
        }
        #endregion

        #region Private Methods
        private void RenderPrefabNameChangeField()
        {
            string newString = EditorGUILayout.TextField(GetContentForPrefabNameChangeField(), _prefab.Name);
            if (newString != _prefab.Name)
            {
                UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.PrefabCategoryDatabase.ActivePrefabCategory);
                _prefab.Name = newString;
            }
        }

        private GUIContent GetContentForPrefabNameChangeField()
        {
            var content = new GUIContent();
            content.text = "Name";
            content.tooltip = "Allows you to change the name of the prefab. Note: Changing this name does not affect the " +
                              "name of the prefab asset. Only the name associated with the object instances will be affected.";

            return content;
        }

        private void RenderSelectPrefabInProjectWindowButton()
        {
            if(GUILayout.Button(GetContentForSelectPrefabInProjectWindowButton(), GUILayout.Width(EditorGUILayoutEx.PreferedActionButtonWidth * 0.45f)))
            {
                EditorGUIUtility.PingObject(_prefab.UnityPrefab);
            }
        }

        private GUIContent GetContentForSelectPrefabInProjectWindowButton()
        {
            var content = new GUIContent();
            content.text = "Select";
            content.tooltip = "Selects the prefab in the project window.";

            return content;
        }

        private void RenderOffsetFromGridSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForOffsetFromGridSurfaceField(), _prefab.OffsetFromGridSurface);
            if (newFloat != _prefab.OffsetFromGridSurface)
            {
                UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.PrefabCategoryDatabase.ActivePrefabCategory);
                _prefab.OffsetFromGridSurface = newFloat;
            }
        }

        private GUIContent GetContentForOffsetFromGridSurfaceField()
        {
            var content = new GUIContent();
            content.text = "Offset from grid surface";
            content.tooltip = "Represents the prefab offset from the grid surface.";

            return content;
        }

        private void RenderOffsetFromObjectSurfaceField()
        {
            float newFloat = EditorGUILayout.FloatField(GetContentForOffsetFromObjectSurfaceField(), _prefab.OffsetFromObjectSurface);
            if (newFloat != _prefab.OffsetFromObjectSurface)
            {
                UndoEx.RecordForToolAction(Octave3DWorldBuilder.ActiveInstance.PrefabCategoryDatabase.ActivePrefabCategory);
                _prefab.OffsetFromObjectSurface = newFloat;
            }
        }

        private GUIContent GetContentForOffsetFromObjectSurfaceField()
        {
            var content = new GUIContent();
            content.text = "Offset from object surface";
            content.tooltip = "Represents the prefab offset from object surfaces.";

            return content;
        }
        #endregion
    }
}
#endif