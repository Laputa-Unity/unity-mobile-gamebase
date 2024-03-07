using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomInspector.Elements;
using UnityEngine;

namespace CustomInspector.Utilities
{
    internal class CustomDrawersUtilities
    {
        private static readonly GenericTypeMatcher GroupDrawerMatcher = typeof(CustomGroupDrawer<>);
        private static readonly GenericTypeMatcher ValueDrawerMatcher = typeof(ValueDrawer<>);
        private static readonly GenericTypeMatcher AttributeDrawerMatcher = typeof(CustomAttributeDrawer<>);
        private static readonly GenericTypeMatcher ValueValidatorMatcher = typeof(CustomValueValidator<>);
        private static readonly GenericTypeMatcher AttributeValidatorMatcher = typeof(CustomAttributeValidator<>);
        private static readonly GenericTypeMatcher HideProcessorMatcher = typeof(CustomPropertyHideProcessor<>);
        private static readonly GenericTypeMatcher DisableProcessorMatcher = typeof(CustomPropertyDisableProcessor<>);

        private static IDictionary<Type, CustomGroupDrawer> _allGroupDrawersCacheBackingField;
        private static IReadOnlyList<RegisterCustomAttributeDrawerAttribute> _allAttributeDrawerTypesBackingField;
        private static IReadOnlyList<RegisterCustomValueDrawerAttribute> _allValueDrawerTypesBackingField;
        private static IReadOnlyList<RegisterCustomAttributeValidatorAttribute> _allAttributeValidatorTypesBackingField;
        private static IReadOnlyList<RegisterCustomValueValidatorAttribute> _allValueValidatorTypesBackingField;
        private static IReadOnlyList<RegisterCustomPropertyHideProcessor> _allHideProcessorTypesBackingField;
        private static IReadOnlyList<RegisterCustomPropertyDisableProcessor> _allDisableProcessorTypesBackingField;

        private static IReadOnlyList<CustomTypeProcessor> _allTypeProcessorBackingField;

        private static IDictionary<Type, CustomGroupDrawer> AllGroupDrawersCache
        {
            get
            {
                if (_allGroupDrawersCacheBackingField == null)
                {
                    _allGroupDrawersCacheBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomGroupDrawerAttribute>()
                        let groupAttributeType = GroupDrawerMatcher.MatchOut(attr.DrawerType, out var t) ? t : null
                        where groupAttributeType != null
                        select new KeyValuePair<Type, RegisterCustomGroupDrawerAttribute>(groupAttributeType, attr)
                    ).ToDictionary(
                        it => it.Key,
                        it => (CustomGroupDrawer) Activator.CreateInstance(it.Value.DrawerType));
                }

                return _allGroupDrawersCacheBackingField;
            }
        }

        public static IReadOnlyList<CustomTypeProcessor> AllTypeProcessors
        {
            get
            {
                if (_allTypeProcessorBackingField == null)
                {
                    _allTypeProcessorBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomTypeProcessorAttribute>()
                        orderby attr.Order
                        select (CustomTypeProcessor) Activator.CreateInstance(attr.ProcessorType)
                    ).ToList();
                }

                return _allTypeProcessorBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomValueDrawerAttribute> AllValueDrawerTypes
        {
            get
            {
                if (_allValueDrawerTypesBackingField == null)
                {
                    _allValueDrawerTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomValueDrawerAttribute>()
                        where ValueDrawerMatcher.Match(attr.DrawerType)
                        select attr
                    ).ToList();
                }

                return _allValueDrawerTypesBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomAttributeDrawerAttribute> AllAttributeDrawerTypes
        {
            get
            {
                if (_allAttributeDrawerTypesBackingField == null)
                {
                    _allAttributeDrawerTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomAttributeDrawerAttribute>()
                        where AttributeDrawerMatcher.Match(attr.DrawerType)
                        select attr
                    ).ToList();
                }

                return _allAttributeDrawerTypesBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomValueValidatorAttribute> AllValueValidatorTypes
        {
            get
            {
                if (_allValueValidatorTypesBackingField == null)
                {
                    _allValueValidatorTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomValueValidatorAttribute>()
                        where ValueValidatorMatcher.Match(attr.ValidatorType)
                        select attr
                    ).ToList();
                }

                return _allValueValidatorTypesBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomAttributeValidatorAttribute> AllAttributeValidatorTypes
        {
            get
            {
                if (_allAttributeValidatorTypesBackingField == null)
                {
                    _allAttributeValidatorTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomAttributeValidatorAttribute>()
                        where AttributeValidatorMatcher.Match(attr.ValidatorType)
                        select attr
                    ).ToList();
                }

                return _allAttributeValidatorTypesBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomPropertyHideProcessor> AllHideProcessors
        {
            get
            {
                if (_allHideProcessorTypesBackingField == null)
                {
                    _allHideProcessorTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomPropertyHideProcessor>()
                        where HideProcessorMatcher.Match(attr.ProcessorType)
                        select attr
                    ).ToList();
                }

                return _allHideProcessorTypesBackingField;
            }
        }

        public static IReadOnlyList<RegisterCustomPropertyDisableProcessor> AllDisableProcessors
        {
            get
            {
                if (_allDisableProcessorTypesBackingField == null)
                {
                    _allDisableProcessorTypesBackingField = (
                        from asm in CustomReflectionUtilities.Assemblies
                        from attr in asm.GetCustomAttributes<RegisterCustomPropertyDisableProcessor>()
                        where DisableProcessorMatcher.Match(attr.ProcessorType)
                        select attr
                    ).ToList();
                }

                return _allDisableProcessorTypesBackingField;
            }
        }

        public static CustomPropertyCollectionBaseElement TryCreateGroupElementFor(DeclareGroupBaseAttribute attribute)
        {
            if (!AllGroupDrawersCache.TryGetValue(attribute.GetType(), out var attr))
            {
                return null;
            }

            return attr.CreateElementInternal(attribute);
        }

        public static IEnumerable<ValueDrawer> CreateValueDrawersFor(Type valueType)
        {
            return
                from drawer in AllValueDrawerTypes
                where ValueDrawerMatcher.Match(drawer.DrawerType, valueType)
                select CreateInstance<ValueDrawer>(drawer.DrawerType, valueType, it =>
                {
                    it.ApplyOnArrayElement = drawer.ApplyOnArrayElement;
                    it.Order = drawer.Order;
                });
        }

        public static IEnumerable<CustomAttributeDrawer> CreateAttributeDrawersFor(
            Type valueType, IReadOnlyList<Attribute> attributes)
        {
            return
                from attribute in attributes
                from drawer in AllAttributeDrawerTypes
                where AttributeDrawerMatcher.Match(drawer.DrawerType, attribute.GetType())
                select CreateInstance<CustomAttributeDrawer>(drawer.DrawerType, valueType, it =>
                {
                    it.ApplyOnArrayElement = drawer.ApplyOnArrayElement;
                    it.Order = drawer.Order;
                    it.RawAttribute = attribute;
                });
        }

        public static IEnumerable<CustomValueValidator> CreateValueValidatorsFor(Type valueType)
        {
            return
                from validator in AllValueValidatorTypes
                where ValueValidatorMatcher.Match(validator.ValidatorType, valueType)
                select CreateInstance<CustomValueValidator>(validator.ValidatorType, valueType, it =>
                {
                    //
                    it.ApplyOnArrayElement = validator.ApplyOnArrayElement;
                });
        }

        public static IEnumerable<CustomAttributeValidator> CreateAttributeValidatorsFor(
            Type valueType, IReadOnlyList<Attribute> attributes)
        {
            return
                from attribute in attributes
                from validator in AllAttributeValidatorTypes
                where AttributeValidatorMatcher.Match(validator.ValidatorType, attribute.GetType())
                select CreateInstance<CustomAttributeValidator>(validator.ValidatorType, valueType, it =>
                {
                    it.ApplyOnArrayElement = validator.ApplyOnArrayElement;
                    it.RawAttribute = attribute;
                });
        }

        public static IEnumerable<CustomPropertyHideProcessor> CreateHideProcessorsFor(
            Type valueType, IReadOnlyList<Attribute> attributes)
        {
            return
                from attribute in attributes
                from processor in AllHideProcessors
                where HideProcessorMatcher.Match(processor.ProcessorType, attribute.GetType())
                select CreateInstance<CustomPropertyHideProcessor>(
                    processor.ProcessorType, valueType, it =>
                    {
                        it.ApplyOnArrayElement = processor.ApplyOnArrayElement;
                        it.RawAttribute = attribute;
                    });
        }

        public static IEnumerable<CustomPropertyDisableProcessor> CreateDisableProcessorsFor(
            Type valueType, IReadOnlyList<Attribute> attributes)
        {
            return
                from attribute in attributes
                from processor in AllDisableProcessors
                where DisableProcessorMatcher.Match(processor.ProcessorType, attribute.GetType())
                select CreateInstance<CustomPropertyDisableProcessor>(
                    processor.ProcessorType, valueType, it =>
                    {
                        it.ApplyOnArrayElement = processor.ApplyOnArrayElement;
                        it.RawAttribute = attribute;
                    });
        }

        private static T CreateInstance<T>(Type type, Type argType, Action<T> setup)
        {
            if (type.IsGenericType)
            {
                type = type.MakeGenericType(argType);
            }

            var instance = (T) Activator.CreateInstance(type);
            setup(instance);
            return instance;
        }

        private class GenericTypeMatcher
        {
            private readonly Dictionary<Type, (bool, Type)> _cache = new Dictionary<Type, (bool, Type)>();
            private readonly Type _expectedGenericType;

            private GenericTypeMatcher(Type expectedGenericType)
            {
                _expectedGenericType = expectedGenericType;
            }

            public static implicit operator GenericTypeMatcher(Type expectedGenericType)
            {
                return new GenericTypeMatcher(expectedGenericType);
            }

            public bool Match(Type type, Type targetType)
            {
                return MatchOut(type, out var constraint) &&
                       constraint.IsAssignableFrom(targetType);
            }

            public bool Match(Type type)
            {
                return MatchOut(type, out _);
            }

            public bool MatchOut(Type type, out Type targetType)
            {
                if (_cache.TryGetValue(type, out var cachedResult))
                {
                    targetType = cachedResult.Item2;
                    return cachedResult.Item1;
                }

                var succeed = MatchInternal(type, out targetType);
                _cache[type] = (succeed, targetType);
                return succeed;
            }

            private bool MatchInternal(Type type, out Type targetType)
            {
                targetType = null;

                if (type.IsAbstract)
                {
                    Debug.LogError($"{type.Name} must be non abstract");
                    return false;
                }

                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    Debug.LogError($"{type.Name} must have a parameterless constructor");
                    return false;
                }

                Type genericArgConstraints = null;
                if (type.IsGenericType)
                {
                    var genericArg = type.GetGenericArguments().SingleOrDefault();

                    if (genericArg == null ||
                        genericArg.GenericParameterAttributes != GenericParameterAttributes.None)
                    {
                        Debug.LogError(
                            $"{type.Name} must contains only one generic arg with simple constant e.g. <where T : bool>");
                        return false;
                    }

                    genericArgConstraints = genericArg.GetGenericParameterConstraints().SingleOrDefault();
                }

                var drawerType = type.BaseType;

                while (drawerType != null)
                {
                    if (drawerType.IsGenericType &&
                        drawerType.GetGenericTypeDefinition() == _expectedGenericType)
                    {
                        targetType = drawerType.GetGenericArguments()[0];

                        if (targetType.IsGenericParameter)
                        {
                            if (genericArgConstraints == null)
                            {
                                Debug.LogError(
                                    $"{type.Name} must contains only one generic arg with simple constant e.g. <where T : bool>");
                                return false;
                            }

                            targetType = genericArgConstraints;
                        }

                        return true;
                    }

                    drawerType = drawerType.BaseType;
                }

                Debug.LogError($"{type.Name} must implement {_expectedGenericType}");
                return false;
            }
        }
    }
}