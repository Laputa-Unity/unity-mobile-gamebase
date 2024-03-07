using CustomInspectorUnityInternalBridge;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace CustomInspector.Editors
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(ScriptedImporter), editorForChildClasses: true)]
    public sealed class CustomScriptedImporterEditor : ScriptedImporterEditor
    {
        private CustomEditorCore _core;

        public override void OnEnable()
        {
            base.OnEnable();

            _core = new CustomEditorCore(this);
        }

        public override void OnDisable()
        {
            _core.Dispose();

            base.OnDisable();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(_core.CreateVisualElement());
            root.Add(new IMGUIContainer(() => DoImporterDefaultGUI()));

            return root;
        }

        private void DoImporterDefaultGUI()
        {
            if (extraDataType != null)
            {
                EditorProxy.DoDrawDefaultInspector(extraDataSerializedObject);
            }

            ApplyRevertGUI();
        }
    }
}