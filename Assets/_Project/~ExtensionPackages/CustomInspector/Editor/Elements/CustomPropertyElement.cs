using System;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Elements
{
    public class CustomPropertyElement : CustomElement
    {
        private readonly CustomProperty _property;

        [Serializable]
        public struct Props
        {
            public bool forceInline;
        }

        public CustomPropertyElement(CustomProperty property, Props props = default)
        {
            _property = property;

            foreach (var error in _property.ExtensionErrors)
            {
                AddChild(new CustomInfoBoxElement(error, CustomMessageType.Error));
            }

            var element = CreateElement(property, props);

            var drawers = property.AllDrawers;
            for (var index = drawers.Count - 1; index >= 0; index--)
            {
                element = drawers[index].CreateElementInternal(property, element);
            }

            AddChild(element);
        }

        public override float GetHeight(float width)
        {
            if (!_property.IsVisible)
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }

            return base.GetHeight(width);
        }

        public override void OnGUI(Rect position)
        {
            if (!_property.IsVisible)
            {
                return;
            }

            var oldShowMixedValue = EditorGUI.showMixedValue;
            var oldEnabled = GUI.enabled;

            GUI.enabled &= _property.IsEnabled;
            EditorGUI.showMixedValue = _property.IsValueMixed;

            using (CustomPropertyOverrideContext.BeginProperty())
            {
                base.OnGUI(position);
            }

            EditorGUI.showMixedValue = oldShowMixedValue;
            GUI.enabled = oldEnabled;
        }

        private static CustomElement CreateElement(CustomProperty property, Props props)
        {
            switch (property.PropertyType)
            {
                case CustomPropertyType.Array:
                {
                    return CreateArrayElement(property);
                }

                case CustomPropertyType.Reference:
                {
                    return CreateReferenceElement(property, props);
                }

                case CustomPropertyType.Generic:
                {
                    return CreateGenericElement(property, props);
                }

                default:
                {
                    return new CustomNoDrawerElement(property);
                }
            }
        }

        private static CustomElement CreateArrayElement(CustomProperty property)
        {
            return new CustomListElement(property);
        }

        private static CustomElement CreateReferenceElement(CustomProperty property, Props props)
        {
            if (property.TryGetAttribute(out InlinePropertyAttribute inlineAttribute))
            {
                return new CustomReferenceElement(property, new CustomReferenceElement.Props
                {
                    inline = true,
                    drawPrefixLabel = !props.forceInline,
                    labelWidth = inlineAttribute.LabelWidth,
                });
            }

            if (props.forceInline)
            {
                return new CustomReferenceElement(property, new CustomReferenceElement.Props
                {
                    inline = true,
                    drawPrefixLabel = false,
                });
            }

            return new CustomReferenceElement(property, new CustomReferenceElement.Props
            {
                inline = false,
                drawPrefixLabel = false,
            });
        }

        private static CustomElement CreateGenericElement(CustomProperty property, Props props)
        {
            if (property.TryGetAttribute(out InlinePropertyAttribute inlineAttribute))
            {
                return new CustomInlineGenericElement(property, new CustomInlineGenericElement.Props
                {
                    drawPrefixLabel = !props.forceInline,
                    labelWidth = inlineAttribute.LabelWidth,
                });
            }

            if (props.forceInline)
            {
                return new CustomInlineGenericElement(property, new CustomInlineGenericElement.Props
                {
                    drawPrefixLabel = false,
                });
            }

            return new CustomFoldoutElement(property);
        }
    }
}