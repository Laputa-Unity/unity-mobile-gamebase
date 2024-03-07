using System;
using UnityEngine;

namespace CustomInspector
{
    public abstract class CustomPropertyOverrideContext
    {
        private static CustomPropertyOverrideContext Override { get; set; }
        public static CustomPropertyOverrideContext Current { get; private set; }

        public abstract bool TryGetDisplayName(CustomProperty property, out GUIContent displayName);

        public static EnterPropertyScope BeginProperty()
        {
            return new EnterPropertyScope().Init();
        }

        public static OverrideScope BeginOverride(CustomPropertyOverrideContext overrideContext)
        {
            return new OverrideScope(overrideContext);
        }

        public struct EnterPropertyScope : IDisposable
        {
            private CustomPropertyOverrideContext _previousContext;

            public EnterPropertyScope Init()
            {
                _previousContext = Current;
                Current = Override;
                Override = null;
                return this;
            }

            public void Dispose()
            {
                Override = Current;
                Current = _previousContext;
            }
        }

        public readonly struct OverrideScope : IDisposable
        {
            public OverrideScope(CustomPropertyOverrideContext context)
            {
                if (Override != null)
                {
                    Debug.LogError($"CustomPropertyContext already overriden with {Override.GetType()}");
                }

                Override = context;
            }

            public void Dispose()
            {
                Override = null;
            }
        }
    }
}