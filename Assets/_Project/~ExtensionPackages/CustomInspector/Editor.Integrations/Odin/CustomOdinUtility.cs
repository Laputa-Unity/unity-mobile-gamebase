using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;

namespace CustomInspector.Editor.Integrations.Odin
{
    public static class CustomOdinUtility
    {
        private static readonly Dictionary<Assembly, bool> CustomDrawnAssemblies = new Dictionary<Assembly, bool>();
        private static HashSet<Type> _triDrawnTypes;

        public static bool IsDrawnByCustom(Type type)
        {
            if (_triDrawnTypes == null)
            {
                var list = TypeCache.GetTypesWithAttribute<DrawWithCustomInspectorAttribute>();
                var array = new Type[list.Count];
                list.CopyTo(array, 0);
                _triDrawnTypes = new HashSet<Type>(array);
            }

            if (_triDrawnTypes.Contains(type))
            {
                return true;
            }

            var asm = type.Assembly;
            if (CustomDrawnAssemblies.TryGetValue(asm, out var assemblyDrawnByCustom))
            {
                return assemblyDrawnByCustom;
            }

            assemblyDrawnByCustom = asm.IsDefined<DrawWithCustomInspectorAttribute>(false);
            CustomDrawnAssemblies[asm] = assemblyDrawnByCustom;
            return assemblyDrawnByCustom;
        }
    }
}