using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Drawers;

[assembly: RegisterCustomAttributeDrawer(typeof(OnValueChangedDrawer), CustomDrawerOrder.System)]

namespace CustomInspector.Drawers
{
    public class OnValueChangedDrawer : CustomAttributeDrawer<OnValueChangedAttribute>
    {
        private ActionResolver _actionResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            base.Initialize(propertyDefinition);

            _actionResolver = ActionResolver.Resolve(propertyDefinition, Attribute.Method);
            if (_actionResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public override CustomElement CreateElement(CustomProperty property, CustomElement next)
        {
            return new OnValueChangedListenerElement(property, next, _actionResolver);
        }

        private class OnValueChangedListenerElement : CustomElement
        {
            private readonly CustomProperty _property;
            private readonly ActionResolver _actionResolver;

            public OnValueChangedListenerElement(CustomProperty property, CustomElement next, ActionResolver actionResolver)
            {
                _property = property;
                _actionResolver = actionResolver;

                AddChild(next);
            }

            protected override void OnAttachToPanel()
            {
                base.OnAttachToPanel();

                _property.ValueChanged += OnValueChanged;
                _property.ChildValueChanged += OnValueChanged;
            }

            protected override void OnDetachFromPanel()
            {
                _property.ChildValueChanged -= OnValueChanged;
                _property.ValueChanged -= OnValueChanged;

                base.OnDetachFromPanel();
            }

            private void OnValueChanged(CustomProperty obj)
            {
                _property.PropertyTree.ApplyChanges();
                _actionResolver.InvokeForAllTargets(_property);
                _property.PropertyTree.Update();
            }
        }
    }
}