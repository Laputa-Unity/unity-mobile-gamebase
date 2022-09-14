// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using UnityEngine;
using System;

#if UNITY_EDITOR
using Animancer.Editor;
using UnityEditor;
#endif

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/AnimancerTransitionAsset
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Animancer Transition", order = Strings.AssetMenuOrder + 0)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(AnimancerTransitionAsset))]
    public class AnimancerTransitionAsset : AnimancerTransitionAsset<ITransition>
    {
        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void Reset()
        {
            Transition = new ClipTransition();
        }
#endif

        /************************************************************************************************************************/

        /// <summary>
        /// An <see cref="AnimancerTransitionAsset{TTransition}"/> wrapper which stores its own <see cref="State"/> and
        /// <see cref="Events"/> to allow multiple objects to reference the same transition asset without interfering
        /// with each other.
        /// </summary>
        /// <remarks>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/assets#unshared">
        /// Transition Assets - UnShared</see>
        /// </remarks>
        /// https://kybernetik.com.au/animancer/api/Animancer/UnShared_3
        /// 
        [Serializable]
        public class UnShared<TAsset, TTransition, TState> : ITransition<TState>, ITransitionWithEvents, IWrapper
            where TAsset : AnimancerTransitionAsset<TTransition>
            where TTransition : ITransition<TState>, IHasEvents
            where TState : AnimancerState
        {
            /************************************************************************************************************************/

            [SerializeField]
            private TAsset _Asset;

            /// <summary>The <see cref="AnimancerTransitionAsset"/> wrapped by this object.</summary>
            public ref TAsset Asset => ref _Asset;

            /// <inheritdoc/>
            object IWrapper.WrappedObject => _Asset;

            /// <summary>The <see cref="ITransition"/> wrapped by this object.</summary>
            public ref TTransition Transition => ref _Asset.Transition;

            /************************************************************************************************************************/

            /// <summary>Can this transition create a valid <see cref="AnimancerState"/>?</summary>
            public virtual bool IsValid => _Asset.IsValid();

            /************************************************************************************************************************/

            private AnimancerState _BaseState;

            /// <summary>
            /// The state that was created by this object. Specifically, this is the state that was most recently
            /// passed into <see cref="Apply"/> (usually by <see cref="AnimancerPlayable.Play(ITransition)"/>).
            /// <para></para>
            /// You can use <see cref="AnimancerPlayable.StateDictionary.GetOrCreate(ITransition)"/> or
            /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/> to get or create the state for a
            /// specific object.
            /// <para></para>
            /// <see cref="State"/> is simply a shorthand for casting this to <typeparamref name="TState"/>.
            /// </summary>
            public AnimancerState BaseState
            {
                get => _BaseState;
                private set
                {
                    _BaseState = value;
                    if (_State != value)
                        _State = null;
                }
            }

            /************************************************************************************************************************/

            private TState _State;

            /// <summary>
            /// The state that was created by this object. Specifically, this is the state that was most recently
            /// passed into <see cref="Apply"/> (usually by <see cref="AnimancerPlayable.Play(ITransition)"/>).
            /// </summary>
            /// 
            /// <remarks>
            /// You can use <see cref="AnimancerPlayable.StateDictionary.GetOrCreate(ITransition)"/> or
            /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/> to get or create the state for a
            /// specific object.
            /// <para></para>
            /// This property is shorthand for casting the <see cref="BaseState"/> to <typeparamref name="TState"/>.
            /// </remarks>
            /// 
            /// <exception cref="InvalidCastException">
            /// The <see cref="BaseState"/> is not actually a <typeparamref name="TState"/>. This should only
            /// happen if a different type of state was created by something else and registered using the
            /// <see cref="Key"/>, causing this <see cref="AnimancerPlayable.Play(ITransition)"/> to pass that
            /// state into <see cref="Apply"/> instead of calling <see cref="CreateState"/> to make the correct type of
            /// state.
            /// </exception>
            public TState State
            {
                get
                {
                    if (_State == null)
                        _State = (TState)BaseState;

                    return _State;
                }
                protected set
                {
                    BaseState = _State = value;
                }
            }

            /************************************************************************************************************************/

            private AnimancerEvent.Sequence _Events;

            /// <inheritdoc/>
            /// <remarks>This property stores a copy of the events from the <see cref="Transition"/></remarks>
            public virtual AnimancerEvent.Sequence Events
            {
                get
                {
                    if (_Events == null)
                        _Events = new AnimancerEvent.Sequence(SerializedEvents.GetEventsOptional());

                    return _Events;
                }
            }

            /// <inheritdoc/>
            public virtual ref AnimancerEvent.Sequence.Serializable SerializedEvents => ref _Asset.Transition.SerializedEvents;

            /************************************************************************************************************************/

            /// <summary>Wraps <see cref="ITransition.Apply"/> and assigns the local <see cref="Events"/>.</summary>
            public virtual void Apply(AnimancerState state)
            {
                BaseState = state;
                _Asset.Apply(state);

                if (_Events == null)
                {
                    _Events = SerializedEvents.GetEventsOptional();
                    if (_Events == null)
                        return;

                    _Events = new AnimancerEvent.Sequence(_Events);
                }

                state.Events = _Events;
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public virtual object Key => _Asset.Key;

            /// <inheritdoc/>
            public virtual float FadeDuration => _Asset.FadeDuration;

            /// <inheritdoc/>
            public virtual FadeMode FadeMode => _Asset.FadeMode;

            /// <inheritdoc/>
            public virtual TState CreateState() => State = (TState)_Asset.CreateState();

            /// <inheritdoc/>
            AnimancerState ITransition.CreateState() => BaseState = _Asset.CreateState();

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

#if UNITY_EDITOR

        /// <summary>[Editor-Only]
        /// A <see cref="PropertyDrawer"/> for <see cref="UnShared{TAsset, TTransition, TState}"/>.
        /// </summary>
        /// <remarks>
        /// This class doesn't inherit from <see cref="TransitionDrawer"/> (which would let it draw the button to open the
        /// <see cref="TransitionPreviewWindow"/>) because it would only show the Transition Asset reference field without
        /// any of the actual values (fade duration, speed, etc.) and trying to redirect everything necessary to preview
        /// the asset's transition would require significant additional complexity.
        /// <para></para>
        /// This issue can be avoided using the
        /// <see href="https://kybernetik.com.au/inspector-gadgets/docs/script-inspector/object-reference-fields#nested-inspector">
        /// Nested Inspectors in Inspector Gadgets</see> by opening the asset's Inspector and previewing it directly.
        /// </remarks>
        [CustomPropertyDrawer(typeof(UnShared<,,>), true)]
        public class UnSharedTransitionDrawer : PropertyDrawer
        {
            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = AnimancerGUI.LineHeight;

                if (property.propertyType == SerializedPropertyType.ManagedReference &&
                    property.isExpanded)
                    height += AnimancerGUI.LineHeight + AnimancerGUI.StandardSpacing;

                return height;
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.ManagedReference)
                {
                    using (new TypeSelectionButton(area, property, true))
                        EditorGUI.PropertyField(area, property, label, true);
                }
                else
                {
                    var transitionProperty = property.FindPropertyRelative("_Asset");
                    EditorGUI.PropertyField(area, transitionProperty, label, false);
                }
            }

            /************************************************************************************************************************/
        }

#endif
    }
}
