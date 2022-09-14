#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ActivePrefabView : EntityView
    {
        #region Constructors
        public ActivePrefabView()
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Active Prefab View";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            Prefab activePrefab = PrefabQueries.GetActivePrefab();
            if(activePrefab != null)
            {
                RenderActivePrefabPreviewButtonAndNextPrevControls(activePrefab);
                activePrefab.View.Render();
            }
            else EditorGUILayout.HelpBox("There is no active prefab currently available.", UnityEditor.MessageType.None);
        }
        #endregion

        #region Private Methods
        private void RenderActivePrefabPreviewButtonAndNextPrevControls(Prefab activePrefab)
        {
            var buttonDrawData = new PrefabPreviewButtonRenderData();
            buttonDrawData.ExtractFromPrefab(activePrefab, 1.0f);

            EditorGUILayoutEx.BeginVertical(GUILayout.Width(buttonDrawData.ButtonWidth));
            GUILayout.Button(buttonDrawData.ButtonContent, "Box", GUILayout.Width(buttonDrawData.ButtonWidth), GUILayout.Height(buttonDrawData.ButtonHeight));
            RenderNextAndPreviousPrefabButtons(activePrefab, buttonDrawData.ButtonWidth * 0.5f);
            EditorGUILayoutEx.EndVertical();
        }

        private void RenderNextAndPreviousPrefabButtons(Prefab activePrefab, float buttonWidth)
        {
            EditorGUILayout.BeginHorizontal();
            RenderPreviousPrefabButton(buttonWidth);
            RenderNextPrefabButton(buttonWidth);
            EditorGUILayout.EndHorizontal();
        }

        private void RenderPreviousPrefabButton(float buttonWidth)
        {
            if (GUILayout.Button(GetContentForPreviousPrefabButton(), GUILayout.Width(buttonWidth)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;

                UndoEx.RecordForToolAction(activePrefabCategory);
                PrefabCategoryActions.ActivatePreviousPrefabInPrefabCategory(activePrefabCategory);
            }
        }

        private GUIContent GetContentForPreviousPrefabButton()
        {
            var content = new GUIContent();
            content.text = "Previous";
            content.tooltip = "Activates the previous prefab in the active prefab category.";

            return content;
        }

        private void RenderNextPrefabButton(float buttonWidth)
        {
            if (GUILayout.Button(GetContentForNextPrefabButton(), GUILayout.Width(buttonWidth)))
            {
                PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;

                UndoEx.RecordForToolAction(activePrefabCategory);
                PrefabCategoryActions.ActivateNextPrefabInPrefabCategory(activePrefabCategory);
            }
        }

        private GUIContent GetContentForNextPrefabButton()
        {
            var content = new GUIContent();
            content.text = "Next";
            content.tooltip = "Activates the next prefab in the active prefab category.";

            return content;
        }
        #endregion
    }
}
#endif