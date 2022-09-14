// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;
using Animancer.Units;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerTransitionAsset
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Controller Transition/Base", order = Strings.AssetMenuOrder + 5)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(ControllerTransitionAsset))]
    public class ControllerTransitionAsset : AnimancerTransitionAsset<ControllerTransition>
    {
        /// <inheritdoc/>
        [Serializable]
        public class UnShared :
            AnimancerTransitionAsset.UnShared<ControllerTransitionAsset, ControllerTransition, ControllerState>,
            ControllerState.ITransition
        { }
    }

    /************************************************************************************************************************/

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerTransition_1
    [Serializable]
    public abstract class ControllerTransition<TState> : AnimancerTransition<TState>, IAnimationClipCollection
        where TState : ControllerState
    {
        /************************************************************************************************************************/

        [SerializeField]
        private RuntimeAnimatorController _Controller;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="ControllerState.Controller"/> that will be used for the created state.
        /// </summary>
        public ref RuntimeAnimatorController Controller => ref _Controller;

        /// <inheritdoc/>
        public override Object MainObject => _Controller;

#if UNITY_EDITOR
        /// <summary>[Editor-Only] The name of the serialized backing field of <see cref="Controller"/>.</summary>
        public const string ControllerFieldName = nameof(_Controller);
#endif

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.NormalizedStartTime)]
        [AnimationTime(AnimationTimeAttribute.Units.Normalized)]
        [DefaultValue(0, float.NaN)]
        private float _NormalizedStartTime;

        /// <inheritdoc/>
        public override float NormalizedStartTime
        {
            get => _NormalizedStartTime;
            set => _NormalizedStartTime = value;
        }

        /************************************************************************************************************************/

        [SerializeField, Tooltip("If false, stopping this state will reset all its layers to their default state")]
        private bool _KeepStateOnStop;

        /// <summary>[<see cref="SerializeField"/>]
        /// If false, <see cref="Stop"/> will reset all layers to their default state.
        /// <para></para>
        /// If you set this value to false after the <see cref="Playable"/> is created, you must assign the
        /// <see cref="DefaultStateHashes"/> or call <see cref="GatherDefaultStates"/> yourself.
        /// </summary>
        public ref bool KeepStateOnStop => ref _KeepStateOnStop;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float MaximumDuration
        {
            get
            {
                if (_Controller == null)
                    return 0;

                var duration = 0f;

                var clips = _Controller.animationClips;
                for (int i = 0; i < clips.Length; i++)
                {
                    var length = clips[i].length;
                    if (duration < length)
                        duration = length;
                }

                return duration;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool IsValid => _Controller != null;

        /************************************************************************************************************************/

        /// <summary>Returns the <see cref="Controller"/>.</summary>
        public static implicit operator RuntimeAnimatorController(ControllerTransition<TState> transition)
            => transition?._Controller;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);

            var controllerState = State;
            if (controllerState != null)
            {
                controllerState.KeepStateOnStop = _KeepStateOnStop;

                if (!float.IsNaN(_NormalizedStartTime))
                {
                    if (!_KeepStateOnStop)
                    {
                        controllerState.Playable.Play(controllerState.DefaultStateHashes[0], 0, _NormalizedStartTime);
                    }
                    else
                    {
                        state.NormalizedTime = _NormalizedStartTime;
                    }
                }
            }
            else
            {
                if (!float.IsNaN(_NormalizedStartTime))
                    state.NormalizedTime = _NormalizedStartTime;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Adds all clips in the <see cref="Controller"/> to the collection.</summary>
        void IAnimationClipCollection.GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            if (_Controller != null)
                clips.Gather(_Controller.animationClips);
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerTransition
    [Serializable]
    public class ControllerTransition : ControllerTransition<ControllerState>, ControllerState.ITransition
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ControllerState CreateState() => State = new ControllerState(Controller, KeepStateOnStop);

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerTransition"/>.</summary>
        public ControllerTransition() { }

        /// <summary>Creates a new <see cref="ControllerTransition"/> with the specified Animator Controller.</summary>
        public ControllerTransition(RuntimeAnimatorController controller) => Controller = controller;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerTransition"/> with the specified Animator Controller.</summary>
        public static implicit operator ControllerTransition(RuntimeAnimatorController controller)
            => new ControllerTransition(controller);

        /************************************************************************************************************************/
        #region Drawer
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        [CustomPropertyDrawer(typeof(ControllerTransition<>), true)]
        [CustomPropertyDrawer(typeof(ControllerTransition), true)]
        public class Drawer : Editor.TransitionDrawer
        {
            /************************************************************************************************************************/

            private readonly string[] Parameters;
            private readonly string[] ParameterPrefixes;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Drawer"/> without any parameters.</summary>
            public Drawer() : this(null) { }

            /// <summary>Creates a new <see cref="Drawer"/> and sets the <see cref="Parameters"/>.</summary>
            public Drawer(params string[] parameters) : base(ControllerFieldName)
            {
                Parameters = parameters;
                if (parameters == null)
                    return;

                ParameterPrefixes = new string[parameters.Length];

                for (int i = 0; i < ParameterPrefixes.Length; i++)
                {
                    ParameterPrefixes[i] = "." + parameters[i];
                }
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            protected override void DoChildPropertyGUI(ref Rect area, SerializedProperty rootProperty,
                SerializedProperty property, GUIContent label)
            {
                var path = property.propertyPath;

                if (ParameterPrefixes != null)
                {
                    var controllerProperty = rootProperty.FindPropertyRelative(MainPropertyName);
                    var controller = controllerProperty.objectReferenceValue as AnimatorController;
                    if (controller != null)
                    {
                        for (int i = 0; i < ParameterPrefixes.Length; i++)
                        {
                            if (path.EndsWith(ParameterPrefixes[i]))
                            {
                                area.height = Editor.AnimancerGUI.LineHeight;
                                DoParameterGUI(area, controller, property);
                                return;
                            }
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();

                base.DoChildPropertyGUI(ref area, rootProperty, property, label);

                // When the controller changes, validate all parameters.
                if (EditorGUI.EndChangeCheck() &&
                    Parameters != null &&
                    path.EndsWith(MainPropertyPathSuffix))
                {
                    var controller = property.objectReferenceValue as AnimatorController;
                    if (controller != null)
                    {
                        for (int i = 0; i < Parameters.Length; i++)
                        {
                            property = rootProperty.FindPropertyRelative(Parameters[i]);
                            var parameterName = property.stringValue;
                            if (!HasFloatParameter(controller, parameterName))
                            {
                                parameterName = GetFirstFloatParameterName(controller);
                                if (!string.IsNullOrEmpty(parameterName))
                                    property.stringValue = parameterName;
                            }
                        }
                    }
                }
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Draws a dropdown menu to select the name of a parameter in the `controller`.
            /// </summary>
            protected void DoParameterGUI(Rect area, AnimatorController controller, SerializedProperty property)
            {
                var parameterName = property.stringValue;
                var parameters = controller.parameters;

                using (ObjectPool.Disposable.AcquireContent(out var label, property))
                {
                    label = EditorGUI.BeginProperty(area, label, property);

                    var xMax = area.xMax;
                    area.width = EditorGUIUtility.labelWidth;
                    EditorGUI.PrefixLabel(area, label);

                    area.x += area.width;
                    area.xMax = xMax;
                }

                var color = GUI.color;
                if (!HasFloatParameter(controller, parameterName))
                    GUI.color = Editor.AnimancerGUI.ErrorFieldColor;

                using (ObjectPool.Disposable.AcquireContent(out var label, parameterName))
                {
                    if (EditorGUI.DropdownButton(area, label, FocusType.Passive))
                    {
                        property = property.Copy();

                        var menu = new GenericMenu();

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var parameter = parameters[i];
                            Editor.Serialization.AddPropertyModifierFunction(menu, property, parameter.name,
                                parameter.type == AnimatorControllerParameterType.Float,
                                (targetProperty) =>
                                {
                                    targetProperty.stringValue = parameter.name;
                                });
                        }

                        if (menu.GetItemCount() == 0)
                            menu.AddDisabledItem(new GUIContent("No Parameters"));

                        menu.ShowAsContext();
                    }
                }

                GUI.color = color;

                EditorGUI.EndProperty();
            }

            /************************************************************************************************************************/

            private static bool HasFloatParameter(AnimatorController controller, string name)
            {
                if (string.IsNullOrEmpty(name))
                    return false;

                var parameters = controller.parameters;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (parameter.type == AnimatorControllerParameterType.Float && name == parameters[i].name)
                    {
                        return true;
                    }
                }

                return false;
            }

            /************************************************************************************************************************/

            private static string GetFirstFloatParameterName(AnimatorController controller)
            {
                var parameters = controller.parameters;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    if (parameter.type == AnimatorControllerParameterType.Float)
                    {
                        return parameter.name;
                    }
                }

                return "";
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        #endregion
        /************************************************************************************************************************/
    }
}
