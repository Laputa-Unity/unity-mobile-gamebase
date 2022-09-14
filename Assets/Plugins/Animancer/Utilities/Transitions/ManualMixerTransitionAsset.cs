// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ManualMixerTransitionAsset
    [CreateAssetMenu(menuName = Strings.MenuPrefix + "Mixer Transition/Manual", order = Strings.AssetMenuOrder + 2)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(ManualMixerTransitionAsset))]
    public class ManualMixerTransitionAsset : AnimancerTransitionAsset<ManualMixerTransition>
    {
        /// <inheritdoc/>
        [Serializable]
        public class UnShared :
            AnimancerTransitionAsset.UnShared<ManualMixerTransitionAsset, ManualMixerTransition, ManualMixerState>,
            ManualMixerState.ITransition
        { }
    }

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ManualMixerTransition_1
    [Serializable]
    public abstract class ManualMixerTransition<TMixer> : AnimancerTransition<TMixer>, IMotion, IAnimationClipCollection
        where TMixer : ManualMixerState
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.OptionalSpeed)]
        [AnimationSpeed]
        [DefaultValue(1f, -1f)]
        private float _Speed = 1;

        /// <summary>[<see cref="SerializeField"/>]
        /// Determines how fast the mixer plays (1x = normal speed, 2x = double speed).
        /// </summary>
        public override float Speed
        {
            get => _Speed;
            set => _Speed = value;
        }

        /************************************************************************************************************************/

        [SerializeField, HideInInspector]
        private Object[] _States;

        /// <summary>[<see cref="SerializeField"/>] Objects that define how to create each state in the mixer.</summary>
        /// <remarks>See <see cref="Initialize(Object[])"/> for more information.</remarks>
        public ref Object[] States => ref _States;

        /// <summary>The name of the serialized backing field of <see cref="States"/>.</summary>
        public const string StatesField = nameof(_States);

        /************************************************************************************************************************/

        [SerializeField, HideInInspector]
        private float[] _Speeds;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="AnimancerNode.Speed"/> to use for each state in the mixer.
        /// </summary>
        /// <remarks>If the size of this array doesn't match the <see cref="States"/>, it will be ignored.</remarks>
        public ref float[] Speeds => ref _Speeds;

        /// <summary>The name of the serialized backing field of <see cref="Speeds"/>.</summary>
        public const string SpeedsField = nameof(_Speeds);

        /// <summary>Are there at least enough <see cref="Speeds"/> for each of the<see cref="States"/>?</summary>
        public bool HasSpeeds => _Speeds != null && _Speeds.Length >= _States.Length;

        /************************************************************************************************************************/

        [SerializeField, HideInInspector]
        private bool[] _SynchronizeChildren;

        /// <summary>[<see cref="SerializeField"/>]
        /// The flags to be used in <see cref="MixerState.InitializeSynchronizedChildren"/>.
        /// </summary>
        /// <remarks>The array can be null or empty. Any elements not in the array will be treated as true.</remarks>
        public ref bool[] SynchronizeChildren => ref _SynchronizeChildren;

        /// <summary>The name of the serialized backing field of <see cref="SynchronizeChildren"/>.</summary>
        public const string SynchronizeChildrenField = nameof(_SynchronizeChildren);

        /************************************************************************************************************************/

        /// <summary>[<see cref="ITransitionDetailed"/>] Are any of the <see cref="States"/> looping?</summary>
        public override bool IsLooping
        {
            get
            {
                for (int i = _States.Length - 1; i >= 0; i--)
                {
                    if (AnimancerUtilities.TryGetIsLooping(_States[i], out var isLooping) &&
                        isLooping)
                        return true;
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public override float MaximumDuration
        {
            get
            {
                if (_States == null)
                    return 0;

                var duration = 0f;
                var hasSpeeds = HasSpeeds;

                for (int i = _States.Length - 1; i >= 0; i--)
                {
                    if (!AnimancerUtilities.TryGetLength(_States[i], out var length))
                        continue;

                    if (hasSpeeds)
                        length *= _Speeds[i];

                    if (duration < length)
                        duration = length;
                }

                return duration;
            }
        }

        /// <inheritdoc/>
        public virtual float AverageAngularSpeed
        {
            get
            {
                if (_States == null)
                    return default;

                var average = 0f;
                var hasSpeeds = HasSpeeds;

                var count = 0;
                for (int i = _States.Length - 1; i >= 0; i--)
                {
                    if (AnimancerUtilities.TryGetAverageAngularSpeed(_States[i], out var speed))
                    {
                        if (hasSpeeds)
                            speed *= _Speeds[i];

                        average += speed;
                        count++;
                    }
                }

                return average / count;
            }
        }

        /// <inheritdoc/>
        public virtual Vector3 AverageVelocity
        {
            get
            {
                if (_States == null)
                    return default;

                var average = new Vector3();
                var hasSpeeds = HasSpeeds;

                var count = 0;
                for (int i = _States.Length - 1; i >= 0; i--)
                {
                    if (AnimancerUtilities.TryGetAverageVelocity(_States[i], out var velocity))
                    {
                        if (hasSpeeds)
                            velocity *= _Speeds[i];

                        average += velocity;
                        count++;
                    }
                }

                return average / count;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Are all <see cref="States"/> assigned?</summary>
        public override bool IsValid
        {
            get
            {
                if (_States == null ||
                    _States.Length == 0)
                    return false;

                for (int i = _States.Length - 1; i >= 0; i--)
                    if (_States[i] == null)
                        return false;

                return true;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Initializes the <see cref="AnimancerTransition{TState}.State"/> immediately after it is created.</summary>
        public virtual void InitializeState()
        {
            var mixer = State;

            var auto = MixerState.AutoSynchronizeChildren;
            try
            {
                MixerState.AutoSynchronizeChildren = false;
                mixer.Initialize(_States);
            }
            finally
            {
                MixerState.AutoSynchronizeChildren = auto;
            }

            mixer.InitializeSynchronizedChildren(_SynchronizeChildren);

            if (_Speeds != null)
            {
#if UNITY_ASSERTIONS
                if (_Speeds.Length != 0 && _Speeds.Length != _States.Length)
                    Debug.LogError(
                        $"The number of serialized {nameof(Speeds)} ({_Speeds.Length})" +
                        $" does not match the number of {nameof(States)} ({_States.Length}).",
                        mixer.Root?.Component as Object);
#endif

                var children = mixer.ChildStates;
                var count = Math.Min(children.Count, _Speeds.Length);
                while (--count >= 0)
                    children[count].Speed = _Speeds[count];
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);

            if (!float.IsNaN(_Speed))
                state.Speed = _Speed;

            for (int i = 0; i < _States.Length; i++)
                if (_States[i] is Animancer.ITransition transition)
                    transition.Apply(state.GetChild(i));
        }

        /************************************************************************************************************************/

        /// <summary>Adds the <see cref="States"/> to the collection.</summary>
        void IAnimationClipCollection.GatherAnimationClips(ICollection<AnimationClip> clips) => clips.GatherFromSource(_States);

        /************************************************************************************************************************/
    }
}
