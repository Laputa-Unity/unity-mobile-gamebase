using System;
using System.Reflection;
using CustomInspector.Elements;
using CustomInspector.Resolvers;
using CustomInspector.Utilities;
using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(ButtonDrawer), CustomDrawerOrder.Drawer)]

namespace CustomInspector.Drawers
{
    public class ButtonDrawer : CustomAttributeDrawer<ButtonAttribute>
    {
        private ValueResolver<string> _nameResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            var isValidMethod = propertyDefinition.TryGetMemberInfo(out var memberInfo) && memberInfo is MethodInfo;
            if (!isValidMethod)
            {
                return "[Button] valid only on methods";
            }

            _nameResolver = ValueResolver.ResolveString(propertyDefinition, Attribute.Name);
            if (_nameResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new CustomButtonElement(property, Attribute, _nameResolver);
        }

        private class CustomButtonElement : CustomHeaderGroupBaseElement
        {
            private readonly CustomProperty _property;
            private readonly ButtonAttribute _attribute;
            private readonly ValueResolver<string> _nameResolver;
            private readonly object[] _invocationArgs;

            public CustomButtonElement(CustomProperty property, ButtonAttribute attribute,
                ValueResolver<string> nameResolver)
            {
                _property = property;
                _attribute = attribute;
                _nameResolver = nameResolver;

                var mi = property.TryGetMemberInfo(out var memberInfo)
                    ? (MethodInfo) memberInfo
                    : throw new Exception("CustomButtonElement requires MethodInfo");

                var parameters = mi.GetParameters();

                _invocationArgs = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var pIndex = i;
                    var pInfo = parameters[pIndex];

                    if (pInfo.HasDefaultValue)
                    {
                        _invocationArgs[pIndex] = pInfo.DefaultValue;
                    }

                    var pCustomDefinition = CustomPropertyDefinition.CreateForGetterSetter(
                        pIndex, pInfo.Name, pInfo.ParameterType,
                        ((self, targetIndex) => _invocationArgs[pIndex]),
                        ((self, targetIndex, value) => _invocationArgs[pIndex] = value));

                    var pCustomProperty = new CustomProperty(_property.PropertyTree, _property, pCustomDefinition, null);

                    AddChild(new CustomPropertyElement(pCustomProperty));
                }
            }

            protected override float GetHeaderHeight(float width)
            {
                return GetButtonHeight();
            }

            protected override void DrawHeader(Rect position)
            {
                if (_invocationArgs.Length > 0)
                {
                    CustomEditorGUI.DrawBox(position, CustomEditorStyles.TabOnlyOne);
                }

                var name = _nameResolver.GetValue(_property);

                if (string.IsNullOrEmpty(name))
                {
                    name = _property.DisplayName;
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = _property.RawName;
                }

                var buttonRect = new Rect(position)
                {
                    height = GetButtonHeight(),
                };

                if (GUI.Button(buttonRect, name))
                {
                    InvokeButton(_property, _invocationArgs);
                }
            }

            private float GetButtonHeight()
            {
                return _attribute.ButtonSize != 0
                    ? _attribute.ButtonSize
                    : EditorGUIUtility.singleLineHeight;
            }
        }

        private static void InvokeButton(CustomProperty property, object[] parameters)
        {
            if (property.TryGetMemberInfo(out var memberInfo) && memberInfo is MethodInfo methodInfo)
            {
                property.ModifyAndRecordForUndo(targetIndex =>
                {
                    try
                    {
                        var parentValue = property.Parent.GetValue(targetIndex);
                        methodInfo.Invoke(parentValue, parameters);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            }
        }
    }
}