using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomValueDrawer(typeof(ObjectReferenceDrawer), CustomDrawerOrder.Fallback)]

namespace CustomInspector.Drawers
{
    public class ObjectReferenceDrawer : ValueDrawer<Object>
    {
        public override CustomElement CreateElement(CustomValue<Object> value, CustomElement next)
        {
            if (value.Property.IsRootProperty || value.Property.TryGetSerializedProperty(out _))
            {
                return next;
            }

            return new ObjectReferenceDrawerElement(value);
        }

        private class ObjectReferenceDrawerElement : CustomElement
        {
            private CustomValue<Object> _propertyValue;
            private readonly bool _allowSceneObjects;

            public ObjectReferenceDrawerElement(CustomValue<Object> propertyValue)
            {
                _propertyValue = propertyValue;
                _allowSceneObjects = propertyValue.Property.PropertyTree.TargetIsPersistent == false;
            }

            public override float GetHeight(float width)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position)
            {
                var value = _propertyValue.SmartValue;

                EditorGUI.BeginChangeCheck();

                value = EditorGUI.ObjectField(position, _propertyValue.Property.DisplayNameContent, value,
                    _propertyValue.Property.FieldType, _allowSceneObjects);

                if (EditorGUI.EndChangeCheck())
                {
                    _propertyValue.SetValue(value);
                }
            }
        }
    }
}