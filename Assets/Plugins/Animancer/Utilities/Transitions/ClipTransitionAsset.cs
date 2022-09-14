// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ClipTransitionAsset
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Clip Transition", order = Strings.AssetMenuOrder + 1)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(ClipTransitionAsset))]
    public class ClipTransitionAsset : AnimancerTransitionAsset<ClipTransition>
    {
        /// <inheritdoc/>
        [Serializable]
        public class UnShared :
            AnimancerTransitionAsset.UnShared<ClipTransitionAsset, ClipTransition, ClipState>,
            ClipState.ITransition
        { }
    }

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ClipTransition
    [Serializable]
    public class ClipTransition : AnimancerTransition<ClipState>, ClipState.ITransition, IMotion, IAnimationClipCollection
    {
        /************************************************************************************************************************/

        [SerializeField, Tooltip("The animation to play")]
        private AnimationClip _Clip;

        /// <summary>[<see cref="SerializeField"/>] The animation to play.</summary>
        public AnimationClip Clip
        {
            get => _Clip;
            set
            {
#if UNITY_ASSERTIONS
                if (value != null)
                    Validate.AssertNotLegacy(value);
#endif

                _Clip = value;
            }
        }

        /// <inheritdoc/>
        public override Object MainObject => _Clip;

        /// <summary>Returns the <see cref="Clip"/> to use as the <see cref="AnimancerState.Key"/>.</summary>
        public override object Key => _Clip;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.OptionalSpeed)]
        [AnimationSpeed]
        [DefaultValue(1f, -1f)]
        private float _Speed = 1;

        /// <inheritdoc/>
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

        /// <summary>
        /// If this transition will set the <see cref="AnimancerState.Time"/>, then it needs to use
        /// <see cref="FadeMode.FromStart"/>.
        /// </summary>
        public override FadeMode FadeMode => float.IsNaN(_NormalizedStartTime) ? FadeMode.FixedSpeed : FadeMode.FromStart;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool IsValid => _Clip != null && !_Clip.legacy;

        /// <summary>[<see cref="ITransitionDetailed"/>] Returns <see cref="Motion.isLooping"/>.</summary>
        public override bool IsLooping => _Clip != null && _Clip.isLooping;

        /// <inheritdoc/>
        public override float MaximumDuration => _Clip != null ? _Clip.length : 0;

        /// <inheritdoc/>
        public virtual float AverageAngularSpeed => _Clip != null ? _Clip.averageAngularSpeed : default;

        /// <inheritdoc/>
        public virtual Vector3 AverageVelocity => _Clip != null ? _Clip.averageSpeed : default;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ClipState CreateState() => State = new ClipState(_Clip);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            ApplyDetails(state, _Speed, _NormalizedStartTime);
        }

        /************************************************************************************************************************/

        /// <summary>[<see cref="IAnimationClipCollection"/>] Adds the <see cref="Clip"/> to the collection.</summary>
        public virtual void GatherAnimationClips(ICollection<AnimationClip> clips) => clips.Gather(_Clip);

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        [UnityEditor.CustomPropertyDrawer(typeof(ClipTransition), true)]
        public class Drawer : Editor.TransitionDrawer
        {
            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Drawer"/>.</summary>
            public Drawer() : base(nameof(_Clip)) { }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}
