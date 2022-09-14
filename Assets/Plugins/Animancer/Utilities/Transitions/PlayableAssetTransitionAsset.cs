// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/PlayableAssetTransitionAsset
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Playable Asset Transition", order = Strings.AssetMenuOrder + 9)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(PlayableAssetTransitionAsset))]
    public class PlayableAssetTransitionAsset : AnimancerTransitionAsset<PlayableAssetTransition>
    {
        /// <inheritdoc/>
        [Serializable]
        public class UnShared :
            AnimancerTransitionAsset.UnShared<PlayableAssetTransitionAsset, PlayableAssetTransition, PlayableAssetState>,
            PlayableAssetState.ITransition
        { }
    }

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/PlayableAssetTransition
    [Serializable]
    public class PlayableAssetTransition : AnimancerTransition<PlayableAssetState>,
        PlayableAssetState.ITransition, IAnimationClipCollection
    {
        /************************************************************************************************************************/

        [SerializeField, Tooltip("The asset to play")]
        private PlayableAsset _Asset;

        /// <summary>[<see cref="SerializeField"/>] The asset to play.</summary>
        public ref PlayableAsset Asset => ref _Asset;

        /// <inheritdoc/>
        public override Object MainObject => _Asset;

        /// <summary>
        /// The <see cref="Asset"/> will be used as the <see cref="AnimancerState.Key"/> for the created state to
        /// be registered with.
        /// </summary>
        public override object Key => _Asset;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.OptionalSpeed)]
        [AnimationSpeed]
        [DefaultValue(1f, -1f)]
        private float _Speed = 1;

        /// <summary>[<see cref="SerializeField"/>]
        /// Determines how fast the animation plays (1x = normal speed, 2x = double speed).
        /// </summary>
        public override float Speed
        {
            get => _Speed;
            set => _Speed = value;
        }

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

        [SerializeField]
        [Tooltip("The objects controlled by each of the tracks in the Asset")]
#if UNITY_2020_2_OR_NEWER
        [NonReorderable]
#endif
        private Object[] _Bindings;

        /// <summary>[<see cref="SerializeField"/>] The objects controlled by each of the tracks in the Asset.</summary>
        public ref Object[] Bindings => ref _Bindings;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float MaximumDuration => _Asset != null ? (float)_Asset.duration : 0;

        /// <inheritdoc/>
        public override bool IsValid => _Asset != null;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override PlayableAssetState CreateState()
        {
            State = new PlayableAssetState(_Asset);
            State.SetBindings(_Bindings);
            return State;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            ApplyDetails(state, _Speed, _NormalizedStartTime);
        }

        /************************************************************************************************************************/

        /// <summary>Gathers all the animations associated with this object.</summary>
        void IAnimationClipCollection.GatherAnimationClips(ICollection<AnimationClip> clips) => clips.GatherFromAsset(_Asset);

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        [UnityEditor.CustomPropertyDrawer(typeof(PlayableAssetTransition), true)]
        public class Drawer : Editor.TransitionDrawer
        {
            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Drawer"/>.</summary>
            public Drawer() : base(nameof(_Asset)) { }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                _CurrentAsset = null;

                var height = base.GetPropertyHeight(property, label);

                if (property.isExpanded)
                {
                    var bindings = property.FindPropertyRelative(nameof(_Bindings));
                    bindings.isExpanded = true;
                    height -= Editor.AnimancerGUI.StandardSpacing + Editor.AnimancerGUI.LineHeight;
                }

                return height;
            }

            /************************************************************************************************************************/

            private static PlayableAsset _CurrentAsset;

            /// <inheritdoc/>
            protected override void DoChildPropertyGUI(ref Rect area, UnityEditor.SerializedProperty rootProperty,
                UnityEditor.SerializedProperty property, GUIContent label)
            {
                var path = property.propertyPath;
                if (path.EndsWith($".{nameof(_Asset)}"))
                {
                    _CurrentAsset = property.objectReferenceValue as PlayableAsset;
                }
                else if (path.EndsWith($".{nameof(_Bindings)}"))
                {
                    IEnumerator<PlayableBinding> outputEnumerator;
                    var outputCount = 0;
                    var firstBindingIsAnimation = false;
                    if (_CurrentAsset != null)
                    {
                        var outputs = _CurrentAsset.outputs;
                        _CurrentAsset = null;
                        outputEnumerator = outputs.GetEnumerator();

                        while (outputEnumerator.MoveNext())
                        {
                            if (PlayableAssetState.ShouldSkipBinding(outputEnumerator.Current, out _, out _))
                                continue;

                            if (outputCount == 0 && outputEnumerator.Current.outputTargetType == typeof(Animator))
                                firstBindingIsAnimation = true;

                            outputCount++;
                        }

                        outputEnumerator = outputs.GetEnumerator();
                    }
                    else outputEnumerator = null;

                    // Bindings.
                    property.Next(true);
                    // Array.
                    property.Next(true);
                    // Array Size.

                    var color = GUI.color;
                    var miniButton = Editor.AnimancerGUI.MiniButton;
                    var sizeArea = area;
                    var bindingCount = property.intValue;
                    if (bindingCount != outputCount && !(bindingCount == 0 && outputCount == 1 && firstBindingIsAnimation))
                    {
                        GUI.color = Editor.AnimancerGUI.WarningFieldColor;

                        var labelText = label.text;

                        var countLabel = outputCount.ToString();
                        var fixSizeWidth = Editor.AnimancerGUI.CalculateWidth(miniButton, countLabel);
                        var fixSizeArea = Editor.AnimancerGUI.StealFromRight(ref sizeArea, fixSizeWidth, Editor.AnimancerGUI.StandardSpacing);
                        if (GUI.Button(fixSizeArea, countLabel, miniButton))
                            property.intValue = outputCount;

                        label.text = labelText;
                    }
                    UnityEditor.EditorGUI.PropertyField(sizeArea, property, label, false);
                    GUI.color = color;

                    UnityEditor.EditorGUI.indentLevel++;

                    bindingCount = property.intValue;
                    for (int i = 0; i < bindingCount; i++)
                    {
                        Editor.AnimancerGUI.NextVerticalArea(ref area);
                        property.Next(false);

                        if (outputEnumerator != null && outputEnumerator.MoveNext())
                        {
                            CheckIfSkip:
                            if (PlayableAssetState.ShouldSkipBinding(outputEnumerator.Current, out var name, out var type))
                            {
                                outputEnumerator.MoveNext();
                                goto CheckIfSkip;
                            }

                            label.text = name;

                            var targetObject = property.serializedObject.targetObject;
                            var allowSceneObjects = targetObject != null && !UnityEditor.EditorUtility.IsPersistent(targetObject);

                            label = UnityEditor.EditorGUI.BeginProperty(area, label, property);
                            var fieldArea = area;
                            var obj = property.objectReferenceValue;
                            var objExists = obj != null;

                            if (objExists)
                            {
                                if (i == 0 && type == typeof(Animator))
                                {
                                    DoRemoveButton(ref fieldArea, label, property, ref obj,
                                        "This Animation Track is the first Track" +
                                        " so it will automatically control the Animancer output and likely does not need a binding.");
                                }
                                else if (type == null)
                                {
                                    DoRemoveButton(ref fieldArea, label, property, ref obj,
                                        "This Animation Track does not need a binding.");
                                    type = typeof(Object);
                                }
                                else if (!type.IsAssignableFrom(obj.GetType()))
                                {
                                    DoRemoveButton(ref fieldArea, label, property, ref obj,
                                        "This binding has the wrong type for this Animation Track.");
                                }
                            }

                            if (type != null || objExists)
                            {
                                property.objectReferenceValue =
                                    UnityEditor.EditorGUI.ObjectField(fieldArea, label, obj, type, allowSceneObjects);
                            }
                            else
                            {
                                UnityEditor.EditorGUI.LabelField(fieldArea, label);
                            }

                            UnityEditor.EditorGUI.EndProperty();
                        }
                        else
                        {
                            GUI.color = Editor.AnimancerGUI.WarningFieldColor;

                            UnityEditor.EditorGUI.PropertyField(area, property, false);
                        }

                        GUI.color = color;
                    }

                    UnityEditor.EditorGUI.indentLevel--;
                    return;
                }

                base.DoChildPropertyGUI(ref area, rootProperty, property, label);
            }

            /************************************************************************************************************************/

            private static void DoRemoveButton(ref Rect area, GUIContent label, UnityEditor.SerializedProperty property,
                ref Object obj, string tooltip)
            {
                label.tooltip = tooltip;
                GUI.color = Editor.AnimancerGUI.WarningFieldColor;
                var miniButton = Editor.AnimancerGUI.MiniButton;

                var text = label.text;
                label.text = "x";

                var xWidth = Editor.AnimancerGUI.CalculateWidth(miniButton, label);
                var xArea = Editor.AnimancerGUI.StealFromRight(
                    ref area, xWidth, Editor.AnimancerGUI.StandardSpacing);
                if (GUI.Button(xArea, label, miniButton))
                    property.objectReferenceValue = obj = null;

                label.text = text;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
