using CustomInspector.Resolvers;
using CustomInspector;
using CustomInspector.Processors;

[assembly: RegisterCustomPropertyDisableProcessor(typeof(DisableIfProcessor))]

namespace CustomInspector.Processors
{
    public class DisableIfProcessor : CustomPropertyDisableProcessor<DisableIfAttribute>
    {
        private ValueResolver<object> _conditionResolver;

        public override CustomExtensionInitializationResult Initialize(CustomPropertyDefinition propertyDefinition)
        {
            base.Initialize(propertyDefinition);

            _conditionResolver = ValueResolver.Resolve<object>(propertyDefinition, Attribute.Condition);
            if (_conditionResolver.TryGetErrorString(out var error))
            {
                return error;
            }

            return CustomExtensionInitializationResult.Ok;
        }

        public sealed override bool IsDisabled(CustomProperty property)
        {
            var val = _conditionResolver.GetValue(property);
            var equal = val?.Equals(Attribute.Value) ?? Attribute.Value == null;
            return equal != Attribute.Inverse;
        }
    }
}