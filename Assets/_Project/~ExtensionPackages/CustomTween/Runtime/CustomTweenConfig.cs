using JetBrains.Annotations;
using UnityEngine;

namespace CustomTween {
    /// Global CustomTween configuration.
    [PublicAPI]
    public static partial class CustomTweenConfig {
        internal static CustomTweenManager Instance {
            get {
                #if UNITY_EDITOR
                Assert.IsFalse(Constants.noInstance, Constants.editModeWarning);
                #endif
                return CustomTweenManager.Instance;
            }
        }

        /// <summary>
        /// If <see cref="CustomTweenManager"/> instance is already created, <see cref="SetTweensCapacity"/> will allocate garbage,
        ///     so it's recommended to use it when no active gameplay is happening. For example, on game launch or when loading a level.
        /// <para>To set initial capacity before <see cref="CustomTweenManager"/> is created, call <see cref="SetTweensCapacity"/> from a method
        /// with <see cref="RuntimeInitializeOnLoadMethodAttribute"/> and <see cref="RuntimeInitializeLoadType.BeforeSplashScreen"/>. See example below.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        /// static void beforeSplashScreen() {
        ///     CustomTweenConfig.SetTweensCapacity(42);
        /// }
        /// </code>
        /// </example>
        public static void SetTweensCapacity(int capacity) {
            Assert.IsTrue(capacity >= 0);
            var instance = CustomTweenManager.Instance; // should use CustomTweenManager.Instance because Instance property has a built-in null check 
            if (instance == null) {
                CustomTweenManager.customInitialCapacity = capacity;
            } else {
                instance.SetTweensCapacity(capacity);
            }
        }

        public static Ease defaultEase {
            get => Instance.defaultEase;
            set {
                if (value == Ease.Custom || value == Ease.Default) {
                    Debug.LogError("defaultEase can't be Ease.Custom or Ease.Default.");
                    return;
                }
                Instance.defaultEase = value;
            }
        }
        
        public static bool warnTweenOnDisabledTarget {
            set => Instance.warnTweenOnDisabledTarget = value;
        }
        
        public static bool warnZeroDuration {
            internal get => Instance.warnZeroDuration;
            set => Instance.warnZeroDuration = value;
        }

        public static bool warnStructBoxingAllocationInCoroutine {
            set => Instance.warnStructBoxingAllocationInCoroutine = value;
        }

        public static bool validateCustomCurves {
            set => Instance.validateCustomCurves = value;
        }

        public static bool warnBenchmarkWithAsserts {
            set => Instance.warnBenchmarkWithAsserts = value;
        }

        internal const bool defaultUseUnscaledTimeForShakes = false;

        public static bool warnEndValueEqualsCurrent {
            set => Instance.warnEndValueEqualsCurrent = value;
        }
    }
}