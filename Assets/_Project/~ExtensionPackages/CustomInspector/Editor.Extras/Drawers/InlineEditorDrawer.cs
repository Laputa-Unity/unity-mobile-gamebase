using CustomInspector.Elements;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: RegisterCustomAttributeDrawer(typeof(InlineEditorDrawer), CustomDrawerOrder.Decorator,
    ApplyOnArrayElement = true)]

namespace CustomInspector.Drawers
{
    public class InlineEditorDrawer : CustomAttributeDrawer<InlineEditorAttribute>
    {
        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            if (!typeof(Object).IsAssignableFrom(propertyDefinition.FieldType))
            {
                return "[InlineEditor] valid only on Object fields";
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            var element = new CustomBoxGroupElement(new CustomBoxGroupElement.Props
            {
                titleMode = CustomBoxGroupElement.TitleMode.Hidden,
            });
            element.AddChild(new ObjectReferenceFoldoutDrawerElement(property));
            element.AddChild(new InlineEditorElement(property));
            return element;
        }

        private class ObjectReferenceFoldoutDrawerElement : CustomElement
        {
            private readonly CustomProperty _property;

            public ObjectReferenceFoldoutDrawerElement(CustomProperty property)
            {
                _property = property;
            }

            public override float GetHeight(float width)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position)
            {
                var prefixRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    xMax = position.xMin + EditorGUIUtility.labelWidth,
                };
                var pickerRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    xMin = prefixRect.xMax,
                };

                CustomEditorGUI.Foldout(prefixRect, _property);

                EditorGUI.BeginChangeCheck();

                var allowSceneObjects = _property.PropertyTree.TargetIsPersistent == false;

                var value = (Object) _property.Value;
                value = EditorGUI.ObjectField(pickerRect, GUIContent.none, value,
                    _property.FieldType, allowSceneObjects);

                if (EditorGUI.EndChangeCheck())
                {
                    _property.SetValue(value);
                }
            }
        }
    }
}