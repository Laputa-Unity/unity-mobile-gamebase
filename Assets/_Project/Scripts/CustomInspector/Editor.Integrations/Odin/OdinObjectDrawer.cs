using System;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using CustomInspector.Utilities;
using UnityEngine;

namespace CustomInspector.Editor.Integrations.Odin
{
    [DrawerPriority(0.0, 10000.0, 1.0)]
    public class OdinObjectDrawer<T> : OdinValueDrawer<T>, IDisposable
        where T : UnityEngine.Object
    {
        private bool _initialized;
        private CustomPropertyTree _propertyTree;

        public override bool CanDrawTypeFilter(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (!CustomOdinUtility.IsDrawnByCustom(type))
            {
                return false;
            }

            return true;
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            if (!property.IsTreeRoot)
            {
                return false;
            }

            if (property.Tree.UnitySerializedObject == null)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _propertyTree?.Dispose();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (CustomGuiHelper.IsEditorTargetPushed(ValueEntry.SmartValue))
            {
                GUILayout.Label("Recursive inline editors not supported");
                return;
            }

            if (!_initialized)
            {
                _initialized = true;
                var serializedObject = Property.Tree.UnitySerializedObject;
                _propertyTree = new CustomPropertyTreeForSerializedObject(serializedObject);
            }

            _propertyTree.Update();
            _propertyTree.RunValidationIfRequired();

            using (CustomGuiHelper.PushEditorTarget(ValueEntry.SmartValue))
            {
                _propertyTree.Draw();
            }

            if (_propertyTree.RepaintRequired)
            {
                GUIHelper.RequestRepaint();
            }
        }
    }
}