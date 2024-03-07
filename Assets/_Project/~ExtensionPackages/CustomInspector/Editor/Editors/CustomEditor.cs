using UnityEditor;
using UnityEngine.UIElements;

namespace CustomInspector.Editors
{
    public abstract class CustomEditor : Editor
    {
        private CustomEditorCore _core;

        protected virtual void OnEnable()
        {
            _core = new CustomEditorCore(this);
        }

        protected virtual void OnDisable()
        {
            _core.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            return _core.CreateVisualElement();
        }
    }
}