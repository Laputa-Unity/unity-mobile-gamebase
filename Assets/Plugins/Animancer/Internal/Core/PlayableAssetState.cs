// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <summary>[Pro-Only] An <see cref="AnimancerState"/> which plays a <see cref="PlayableAsset"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/timeline">Timeline</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/PlayableAssetState
    /// 
    public sealed class PlayableAssetState : AnimancerState
    {
        /************************************************************************************************************************/

        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="PlayableAssetState"/>.</summary>
        public interface ITransition : ITransition<PlayableAssetState> { }

        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        /// <summary>The <see cref="PlayableAsset"/> which this state plays.</summary>
        private PlayableAsset _Asset;

        /// <summary>The <see cref="PlayableAsset"/> which this state plays.</summary>
        public PlayableAsset Asset
        {
            get => _Asset;
            set => ChangeMainObject(ref _Asset, value);
        }

        /// <summary>The <see cref="PlayableAsset"/> which this state plays.</summary>
        public override Object MainObject
        {
            get => _Asset;
            set => _Asset = (PlayableAsset)value;
        }

        /************************************************************************************************************************/

        private float _Length;

        /// <summary>The <see cref="PlayableAsset.duration"/>.</summary>
        public override float Length => _Length;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void OnSetIsPlaying()
        {
            var inputCount = _Playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                var playable = _Playable.GetInput(i);
                if (!playable.IsValid())
                    continue;

                if (IsPlaying)
                    playable.Play();
                else
                    playable.Pause();
            }
        }

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="PlayableAssetState"/>.</summary>
        public override void CopyIKFlags(AnimancerNode node) { }

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="PlayableAssetState"/>.</summary>
        public override bool ApplyAnimatorIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log(
                        $"IK cannot be dynamically enabled on a {nameof(PlayableAssetState)}.", Root?.Component);
#endif
            }
        }

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="PlayableAssetState"/>.</summary>
        public override bool ApplyFootIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log(
                        $"IK cannot be dynamically enabled on a {nameof(PlayableAssetState)}.", Root?.Component);
#endif
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Methods
        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="PlayableAssetState"/> to play the `asset`.</summary>
        /// <exception cref="ArgumentNullException">The `asset` is null.</exception>
        public PlayableAssetState(PlayableAsset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            _Asset = asset;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void CreatePlayable(out Playable playable)
        {
            playable = _Asset.CreatePlayable(Root._Graph, Root.Component.gameObject);
            playable.SetDuration(9223372.03685477);// https://github.com/KybernetikGames/animancer/issues/111

            _Length = (float)_Asset.duration;

            if (!_HasInitializedBindings)
                InitializeBindings();
        }

        /************************************************************************************************************************/

        private IList<Object> _Bindings;
        private bool _HasInitializedBindings;

        /************************************************************************************************************************/

        /// <summary>The objects controlled by each track in the asset.</summary>
        public IList<Object> Bindings
        {
            get => _Bindings;
            set
            {
                _Bindings = value;
                InitializeBindings();
            }
        }

        /************************************************************************************************************************/

        /// <summary>Sets the <see cref="Bindings"/>.</summary>
        public void SetBindings(params Object[] bindings)
        {
            Bindings = bindings;
        }

        /************************************************************************************************************************/

        private void InitializeBindings()
        {
            if (_Bindings == null || Root == null)
                return;

            _HasInitializedBindings = true;

            var bindingCount = _Bindings.Count;
            if (bindingCount == 0)
                return;

            var output = _Asset.outputs.GetEnumerator();
            var graph = Root._Graph;

            for (int i = 0; i < bindingCount; i++)
            {
                if (!output.MoveNext())
                    return;

                if (ShouldSkipBinding(output.Current, out var name, out var type))
                {
                    i--;
                    continue;
                }

                var binding = _Bindings[i];
                if (binding == null && type != null)
                    continue;

#if UNITY_ASSERTIONS
                if (type != null && !type.IsAssignableFrom(binding.GetType()))
                {
                    Debug.LogError(
                        $"Binding Type Mismatch: bindings[{i}] is '{binding}' but should be a {type.FullName} for {name}",
                        Root?.Component as Object);
                    continue;
                }

                Validate.AssertPlayable(this);
#endif

                var playable = _Playable.GetInput(i);

                if (type == typeof(Animator))
                {
                    var playableOutput = AnimationPlayableOutput.Create(graph, name, (Animator)binding);
                    playableOutput.SetSourcePlayable(playable);
                }
                else if (type == typeof(AudioSource))
                {
                    var playableOutput = AudioPlayableOutput.Create(graph, name, (AudioSource)binding);
                    playableOutput.SetSourcePlayable(playable);
                }
                else// ActivationTrack, SignalTrack, ControlTrack, PlayableTrack.
                {
                    var playableOutput = ScriptPlayableOutput.Create(graph, name);
                    playableOutput.SetUserData(binding);
                    playableOutput.SetSourcePlayable(playable);
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>Should the `binding` be skipped when determining how to map the <see cref="Bindings"/>?</summary>
        public static bool ShouldSkipBinding(PlayableBinding binding, out string name, out Type type)
        {
            name = binding.streamName;
            type = binding.outputTargetType;

            if (type == typeof(GameObject) && name == "Markers")
                return true;

            return false;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Destroy()
        {
            _Asset = null;
            base.Destroy();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

