using System;
using CustomInspector.Utilities;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Elements
{
    internal class CustomInlineGenericElement : CustomPropertyCollectionBaseElement
    {
        private readonly Props _props;
        private readonly CustomProperty _property;

        [Serializable]
        public struct Props
        {
            public bool drawPrefixLabel;
            public float labelWidth;
        }

        public CustomInlineGenericElement(CustomProperty property, Props props = default)
        {
            _property = property;
            _props = props;

            DeclareGroups(property.ValueType);

            foreach (var childProperty in property.ChildrenProperties)
            {
                AddProperty(childProperty);
            }
        }

        public override void OnGUI(Rect position)
        {
            if (_props.drawPrefixLabel)
            {
                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                position = EditorGUI.PrefixLabel(position, controlId, _property.DisplayNameContent);
            }

            using (CustomGuiHelper.PushLabelWidth(_props.labelWidth))
            {
                base.OnGUI(position);
            }
        }
    }
}