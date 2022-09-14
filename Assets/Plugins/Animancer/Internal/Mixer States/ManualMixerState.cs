// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Animancer
{
    /// <summary>[Pro-Only]
    /// An <see cref="AnimancerState"/> which blends multiple child states. Unlike other mixers, this class does not
    /// perform any automatic weight calculations, it simple allows you to control the weight of all states manually.
    /// </summary>
    /// <remarks>
    /// This mixer type is similar to the Direct Blend Type in Mecanim Blend Trees.
    /// The official <see href="https://learn.unity.com/tutorial/5c5152bcedbc2a001fd5c696">Direct Blend Trees</see>
    /// tutorial explains their general concepts and purpose which apply to <see cref="ManualMixerState"/>s as well.
    /// <para></para>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/blending/mixers">Mixers</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/ManualMixerState
    /// 
    public partial class ManualMixerState : MixerState
    {
        /************************************************************************************************************************/

        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="ManualMixerState"/>.</summary>
        public interface ITransition : ITransition<ManualMixerState> { }

        /************************************************************************************************************************/
        #region Properties
        /************************************************************************************************************************/

        /// <summary>The states managed by this mixer.</summary>
        private AnimancerState[] _States = Array.Empty<AnimancerState>();

        /// <summary>Returns the array of <see cref="_States"/>.</summary>
        public override IList<AnimancerState> ChildStates => _States;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ChildCount => _States.Length;

        /// <inheritdoc/>
        public override AnimancerState GetChild(int index) => _States[index];

        /// <inheritdoc/>
        public override FastEnumerator<AnimancerState> GetEnumerator()
            => new FastEnumerator<AnimancerState>(_States, _States.Length);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Initialisation
        /************************************************************************************************************************/

        /// <summary>
        /// Initializes this mixer with the specified number of children which can be set individually by
        /// <see cref="MixerState.CreateChild(int, AnimationClip)"/> and <see cref="MixerState.SetChild"/>.
        /// </summary>
        /// <remarks><see cref="AnimancerState.Destroy"/> will be called on any existing children.</remarks>
        public virtual void Initialize(int childCount)
        {
#if UNITY_ASSERTIONS
            if (childCount <= 1 && OptionalWarning.MixerMinChildren.IsEnabled())
                OptionalWarning.MixerMinChildren.Log(
                    $"{this} is being initialized with {nameof(childCount)} <= 1." +
                    $" The purpose of a mixer is to mix multiple child states.", Root?.Component);
#endif

            for (int i = _States.Length - 1; i >= 0; i--)
            {
                var state = _States[i];
                if (state == null)
                    continue;

                state.Destroy();
            }

            _States = new AnimancerState[childCount];

            if (_Playable.IsValid())
            {
                _Playable.SetInputCount(childCount);
            }
            else if (Root != null)
            {
                CreatePlayable();
            }
        }

        /************************************************************************************************************************/

        /// <summary>Initializes this mixer with one state per clip.</summary>
        public void Initialize(params AnimationClip[] clips)
        {
#if UNITY_ASSERTIONS
            if (clips == null)
                throw new ArgumentNullException(nameof(clips));
#endif

            var count = clips.Length;
            Initialize(count);

            for (int i = 0; i < count; i++)
            {
                var clip = clips[i];
                if (clip != null)
                    CreateChild(i, clip);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Initializes this mixer by calling <see cref="MixerState.CreateChild(int, Object)"/> for each of the
        /// `states`.
        /// </summary>
        public void Initialize(params Object[] states)
        {
#if UNITY_ASSERTIONS
            if (states == null)
                throw new ArgumentNullException(nameof(states));
#endif

            var count = states.Length;
            Initialize(count);

            for (int i = 0; i < count; i++)
            {
                var state = states[i];
                if (state != null)
                    CreateChild(i, state);
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

