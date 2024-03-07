using CustomInspector;
using CustomInspector.Drawers;
using UnityEditor;
using UnityEngine;

[assembly: RegisterCustomAttributeDrawer(typeof(SceneDrawer), CustomDrawerOrder.Decorator, ApplyOnArrayElement = true)]

namespace CustomInspector.Drawers
{
    public class SceneDrawer : CustomAttributeDrawer<SceneAttribute>
    {
        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            var type = propertyDefinition.FieldType;
            if (type != typeof(string))
            {
                return "Scene attribute can only be used on field with string type";
            }

            return base.Initialize(propertyDefinition);
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new SceneElement(property);
        }

        private class SceneElement : CustomElement
        {
            private readonly CustomProperty _property;

            private SceneAsset _sceneAsset;

            public SceneElement(CustomProperty property)
            {
                _property = property;
            }

            protected override void OnAttachToPanel()
            {
                base.OnAttachToPanel();

                _property.ValueChanged += OnValueChanged;

                RefreshSceneAsset();
            }

            protected override void OnDetachFromPanel()
            {
                _property.ValueChanged -= OnValueChanged;

                base.OnDetachFromPanel();
            }

            public override float GetHeight(float width)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position)
            {
                EditorGUI.BeginChangeCheck();

                var asset = EditorGUI.ObjectField(position, _property.DisplayName, _sceneAsset,
                    typeof(SceneAsset), false);

                if (EditorGUI.EndChangeCheck())
                {
                    var path = AssetDatabase.GetAssetPath(asset);
                    _property.SetValue(path);
                }
            }

            private void OnValueChanged(CustomProperty property)
            {
                RefreshSceneAsset();
            }

            private void RefreshSceneAsset()
            {
                _sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_property.Value as string);
            }
        }
    }
}