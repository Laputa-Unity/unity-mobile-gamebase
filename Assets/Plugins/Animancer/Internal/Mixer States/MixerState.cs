// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <summary>[Pro-Only] Base class for <see cref="AnimancerState"/>s which blend other states together.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/blending/mixers">Mixers</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerState
    /// 
    public abstract partial class MixerState : AnimancerState
    {
        /************************************************************************************************************************/

        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="MixerState{TParameter}"/> for <see cref="Vector2"/>.</summary>
        public interface ITransition2D : ITransition<MixerState<Vector2>> { }

        /************************************************************************************************************************/
        #region Properties
        /************************************************************************************************************************/

        /// <summary>Mixers should keep child playables connected to the graph at all times.</summary>
        public override bool KeepChildrenConnected => true;

        /// <summary>A <see cref="MixerState"/> has no <see cref="AnimationClip"/>.</summary>
        public override AnimationClip Clip => null;

        /************************************************************************************************************************/

        /// <summary>Returns the collection of states connected to this mixer. Note that some elements may be null.</summary>
        /// <remarks>
        /// Getting an enumerator that automatically skips over null states is slower and creates garbage, so
        /// internally we use this property and perform null checks manually even though it increases the code
        /// complexity a bit.
        /// </remarks>
        public abstract IList<AnimancerState> ChildStates { get; }

        /// <inheritdoc/>
        public override int ChildCount => ChildStates.Count;

        /// <inheritdoc/>
        public override AnimancerState GetChild(int index) => ChildStates[index];

        /// <inheritdoc/>
        public override FastEnumerator<AnimancerState> GetEnumerator()
            => new FastEnumerator<AnimancerState>(ChildStates);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void OnSetIsPlaying()
        {
            var childStates = ChildStates;
            for (int i = childStates.Count - 1; i >= 0; i--)
            {
                var state = childStates[i];
                if (state == null)
                    continue;

                state.IsPlaying = IsPlaying;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Are any child states looping?</summary>
        public override bool IsLooping
        {
            get
            {
                var childStates = ChildStates;
                for (int i = childStates.Count - 1; i >= 0; i--)
                {
                    var state = childStates[i];
                    if (state == null)
                        continue;

                    if (state.IsLooping)
                        return true;
                }

                return false;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The weighted average <see cref="AnimancerState.Time"/> of each child state according to their
        /// <see cref="AnimancerNode.Weight"/>.
        /// </summary>
        /// <remarks>
        /// If there are any <see cref="SynchronizedChildren"/>, only those states will be included in the getter
        /// calculation.
        /// </remarks>
        protected override float RawTime
        {
            get
            {
                RecalculateWeights();

                if (!GetSynchronizedTimeDetails(out var totalWeight, out var normalizedTime, out var length))
                    GetTimeDetails(out totalWeight, out normalizedTime, out length);

                if (totalWeight == 0)
                    return base.RawTime;

                totalWeight *= totalWeight;
                return normalizedTime * length / totalWeight;
            }
            set
            {
                var states = ChildStates;
                var childCount = states.Count;

                if (value == 0)
                    goto ZeroTime;

                var length = Length;
                if (length == 0)
                    goto ZeroTime;

                value /= length;// Normalize.

                while (--childCount >= 0)
                {
                    var state = states[childCount];
                    if (state != null)
                        state.NormalizedTime = value;
                }

                return;

                // If the value is 0, we can set the child times slightly more efficiently.
                ZeroTime:
                while (--childCount >= 0)
                {
                    var state = states[childCount];
                    if (state != null)
                        state.Time = 0;
                }
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void MoveTime(float time, bool normalized)
        {
            base.MoveTime(time, normalized);

            var states = ChildStates;
            var count = states.Count;
            for (int i = 0; i < count; i++)
                states[i].MoveTime(time, normalized);
        }

        /************************************************************************************************************************/

        /// <summary>Gets the time details based on the <see cref="SynchronizedChildren"/>.</summary>
        private bool GetSynchronizedTimeDetails(out float totalWeight, out float normalizedTime, out float length)
        {
            totalWeight = 0;
            normalizedTime = 0;
            length = 0;

            if (_SynchronizedChildren != null)
            {
                for (int i = _SynchronizedChildren.Count - 1; i >= 0; i--)
                {
                    var state = _SynchronizedChildren[i];
                    var weight = state.Weight;
                    if (weight == 0)
                        continue;

                    var stateLength = state.Length;
                    if (stateLength == 0)
                        continue;

                    totalWeight += weight;
                    normalizedTime += state.Time / stateLength * weight;
                    length += stateLength * weight;
                }
            }

            return totalWeight > MinimumSynchronizeChildrenWeight;
        }

        /// <summary>Gets the time details based on all child states.</summary>
        private void GetTimeDetails(out float totalWeight, out float normalizedTime, out float length)
        {
            totalWeight = 0;
            normalizedTime = 0;
            length = 0;

            var states = ChildStates;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];
                if (state == null)
                    continue;

                var weight = state.Weight;
                if (weight == 0)
                    continue;

                var stateLength = state.Length;
                if (stateLength == 0)
                    continue;

                totalWeight += weight;
                normalizedTime += state.Time / stateLength * weight;
                length += stateLength * weight;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The weighted average <see cref="AnimancerState.Length"/> of each child state according to their
        /// <see cref="AnimancerNode.Weight"/>.
        /// </summary>
        public override float Length
        {
            get
            {
                RecalculateWeights();

                var length = 0f;
                var totalChildWeight = 0f;

                if (_SynchronizedChildren != null)
                {
                    for (int i = _SynchronizedChildren.Count - 1; i >= 0; i--)
                    {
                        var state = _SynchronizedChildren[i];
                        var weight = state.Weight;
                        if (weight == 0)
                            continue;

                        var stateLength = state.Length;
                        if (stateLength == 0)
                            continue;

                        totalChildWeight += weight;
                        length += stateLength * weight;
                    }
                }

                if (totalChildWeight > 0)
                    return length / totalChildWeight;

                var states = ChildStates;
                totalChildWeight = CalculateTotalWeight(states);
                if (totalChildWeight <= 0)
                    return 0;

                for (int i = states.Count - 1; i >= 0; i--)
                {
                    var state = states[i];
                    if (state != null)
                        length += state.Length * state.Weight;
                }

                return length / totalChildWeight;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Initialisation
        /************************************************************************************************************************/

        /// <summary>Creates and assigns the <see cref="AnimationMixerPlayable"/> managed by this state.</summary>
        protected override void CreatePlayable(out Playable playable)
        {
            playable = AnimationMixerPlayable.Create(Root._Graph, ChildStates.Count, false);
            RecalculateWeights();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates and returns a new <see cref="ClipState"/> to play the `clip` with this mixer as its parent.
        /// </summary>
        public ClipState CreateChild(int index, AnimationClip clip)
        {
            var state = new ClipState(clip);
            state.SetParent(this, index);
            state.IsPlaying = IsPlaying;
            return state;
        }

        /// <summary>
        /// Calls <see cref="AnimancerUtilities.CreateStateAndApply"/> and sets this mixer as the state's parent.
        /// </summary>
        public AnimancerState CreateChild(int index, Animancer.ITransition transition)
        {
            var state = transition.CreateStateAndApply(Root);
            state.SetParent(this, index);
            state.IsPlaying = IsPlaying;
            return state;
        }

        /// <summary>Calls <see cref="CreateChild(int, AnimationClip)"/> or <see cref="CreateChild(int, Animancer.ITransition)"/>.</summary>
        public AnimancerState CreateChild(int index, Object state)
        {
            if (state is AnimationClip clip)
            {
                return CreateChild(index, clip);
            }
            else if (state is ITransition transition)
            {
                return CreateChild(index, transition);
            }
            else return null;
        }

        /************************************************************************************************************************/

        /// <summary>Assigns the `state` as a child of this mixer.</summary>
        public void SetChild(int index, AnimancerState state) => state.SetParent(this, index);

        /************************************************************************************************************************/

        /// <summary>Connects the `state` to this mixer at its <see cref="AnimancerNode.Index"/>.</summary>
        protected internal override void OnAddChild(AnimancerState state)
        {
            OnAddChild(ChildStates, state);

            if (AutoSynchronizeChildren)
                Synchronize(state);

#if UNITY_ASSERTIONS
            if (_IsGeneratedName)
            {
                _IsGeneratedName = false;
                SetDebugName(null);
            }
#endif
        }

        /************************************************************************************************************************/

        /// <summary>Disconnects the `state` from this mixer at its <see cref="AnimancerNode.Index"/>.</summary>
        protected internal override void OnRemoveChild(AnimancerState state)
        {
            if (_SynchronizedChildren != null)
                _SynchronizedChildren.Remove(state);

            var states = ChildStates;
            Validate.AssertCanRemoveChild(state, states);
            states[state.Index] = null;
            Root?._Graph.Disconnect(_Playable, state.Index);

#if UNITY_ASSERTIONS
            if (_IsGeneratedName)
            {
                _IsGeneratedName = false;
                SetDebugName(null);
            }
#endif
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Destroy()
        {
            DestroyChildren();
            base.Destroy();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Destroys all <see cref="ChildStates"/> connected to this mixer. This operation cannot be undone.
        /// </summary>
        public void DestroyChildren()
        {
            var states = ChildStates;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];
                if (state != null)
                    state.Destroy();
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Jobs
        /************************************************************************************************************************/

        /// <summary>
        /// Creates an <see cref="AnimationScriptPlayable"/> to run the specified Animation Job instead of the usual
        /// <see cref="AnimationMixerPlayable"/>.
        /// </summary>
        /// <example><code>
        /// var job = new MyJob();// A struct that implements IAnimationJob.
        /// var mixer = new WhateverMixerType();
        /// mixer.CreatePlayable(animancer, job);
        /// // Use mixer.Initialize and CreateState to make the children as normal.
        /// </code>
        /// See also: <seealso cref="CreatePlayable{T}(out Playable, T, bool)"/>
        /// </example>
        public AnimationScriptPlayable CreatePlayable<T>(AnimancerPlayable root, T job, bool processInputs = false)
            where T : struct, IAnimationJob
        {
            SetRoot(null);

            Root = root;
            root.States.Register(Key, this);

            var playable = AnimationScriptPlayable.Create(root._Graph, job, ChildCount);

            if (!processInputs)
                playable.SetProcessInputs(false);

            for (int i = ChildCount - 1; i >= 0; i--)
                GetChild(i)?.SetRoot(root);

            return playable;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates an <see cref="AnimationScriptPlayable"/> to run the specified Animation Job instead of the usual
        /// <see cref="AnimationMixerPlayable"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/source/creating-custom-states">Creating Custom States</see>
        /// </remarks>
        /// 
        /// <example><code>
        /// public class MyMixer : LinearMixerState
        /// {
        ///     protected override void CreatePlayable(out Playable playable)
        ///     {
        ///         CreatePlayable(out playable, new MyJob());
        ///     }
        /// 
        ///     private struct MyJob : IAnimationJob
        ///     {
        ///         public void ProcessAnimation(AnimationStream stream)
        ///         {
        ///         }
        /// 
        ///         public void ProcessRootMotion(AnimationStream stream)
        ///         {
        ///         }
        ///     }
        /// }
        /// </code>
        /// See also: <seealso cref="CreatePlayable{T}(AnimancerPlayable, T, bool)"/>
        /// </example>
        protected void CreatePlayable<T>(out Playable playable, T job, bool processInputs = false)
            where T : struct, IAnimationJob
        {
            var scriptPlayable = AnimationScriptPlayable.Create(Root._Graph, job, ChildCount);

            if (!processInputs)
                scriptPlayable.SetProcessInputs(false);

            playable = scriptPlayable;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Gets the Animation Job data from the <see cref="AnimationScriptPlayable"/>.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// This mixer was not initialized using <see cref="CreatePlayable{T}(AnimancerPlayable, T, bool)"/>
        /// or <see cref="CreatePlayable{T}(out Playable, T, bool)"/>.
        /// </exception>
        public T GetJobData<T>()
            where T : struct, IAnimationJob
            => ((AnimationScriptPlayable)_Playable).GetJobData<T>();

        /// <summary>
        /// Sets the Animation Job data in the <see cref="AnimationScriptPlayable"/>.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// This mixer was not initialized using <see cref="CreatePlayable{T}(AnimancerPlayable, T, bool)"/>
        /// or <see cref="CreatePlayable{T}(out Playable, T, bool)"/>.
        /// </exception>
        public void SetJobData<T>(T value)
            where T : struct, IAnimationJob
            => ((AnimationScriptPlayable)_Playable).SetJobData<T>(value);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Updates
        /************************************************************************************************************************/

        /// <summary>Updates the time of this mixer and all of its child states.</summary>
        protected internal override void Update(out bool needsMoreUpdates)
        {
            base.Update(out needsMoreUpdates);

            if (RecalculateWeights())
            {
                // Apply the child weights immediately to ensure they are all in sync. Otherwise some of them might
                // have already updated before the mixer and would not apply it until next frame.
                var childStates = ChildStates;
                for (int i = childStates.Count - 1; i >= 0; i--)
                {
                    var state = childStates[i];
                    if (state == null)
                        continue;

                    state.ApplyWeight();
                }
            }

            ApplySynchronizeChildren(ref needsMoreUpdates);
        }

        /************************************************************************************************************************/

        /// <summary>Indicates whether the weights of all child states should be recalculated.</summary>
        public bool WeightsAreDirty { get; set; }

        /************************************************************************************************************************/

        /// <summary>
        /// If <see cref="WeightsAreDirty"/> this method recalculates the weights of all child states and returns true.
        /// </summary>
        public bool RecalculateWeights()
        {
            if (WeightsAreDirty)
            {
                ForceRecalculateWeights();

                Debug.Assert(!WeightsAreDirty,
                    $"{nameof(MixerState)}.{nameof(WeightsAreDirty)} was not set to false by {nameof(ForceRecalculateWeights)}().");

                return true;
            }
            else return false;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Recalculates the weights of all child states based on the current value of the
        /// <see cref="MixerState{TParameter}.Parameter"/> and the thresholds.
        /// <para></para>
        /// Overrides of this method must set <see cref="WeightsAreDirty"/> = false.
        /// </summary>
        protected virtual void ForceRecalculateWeights() { }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Synchronize Children
        /************************************************************************************************************************/

        /// <summary>Should newly added children be automatically added to the synchronization list? Default true.</summary>
        public static bool AutoSynchronizeChildren { get; set; } = true;

        /// <summary>The minimum total weight of all children for their times to be synchronized (default 0.01).</summary>
        public static float MinimumSynchronizeChildrenWeight { get; set; } = 0.01f;

        /************************************************************************************************************************/

        private List<AnimancerState> _SynchronizedChildren;

        /// <summary>A copy of the internal list of child states that will have their times synchronized.</summary>
        /// <remarks>
        /// If this mixer is a child of another mixer, its synchronized children will be managed by the parent.
        /// <para></para>
        /// The getter allocates a new array if <see cref="SynchronizedChildCount"/> is greater than zero.
        /// </remarks>
        public AnimancerState[] SynchronizedChildren
        {
            get => SynchronizedChildCount > 0 ? _SynchronizedChildren.ToArray() : Array.Empty<AnimancerState>();
            set
            {
                if (_SynchronizedChildren == null)
                    _SynchronizedChildren = new List<AnimancerState>();
                else
                    _SynchronizedChildren.Clear();

                for (int i = 0; i < value.Length; i++)
                    Synchronize(value[i]);
            }
        }

        /// <summary>The number of <see cref="SynchronizedChildren"/>.</summary>
        public int SynchronizedChildCount => _SynchronizedChildren != null ? _SynchronizedChildren.Count : 0;

        /************************************************************************************************************************/

        /// <summary>Is the `state` in the <see cref="SynchronizedChildren"/>?</summary>
        public bool IsSynchronized(AnimancerState state)
        {
            var synchronizer = GetParentMixer();
            return
                synchronizer._SynchronizedChildren != null &&
                synchronizer._SynchronizedChildren.Contains(state);
        }

        /************************************************************************************************************************/

        /// <summary>Adds the `state` to the <see cref="SynchronizedChildren"/>.</summary>
        /// <remarks>
        /// The `state` must be a child of this mixer.
        /// <para></para>
        /// If this mixer is a child of another mixer, the `state` will be added to the parent's
        /// <see cref="SynchronizedChildren"/> instead.
        /// </remarks>
        public void Synchronize(AnimancerState state)
        {
            if (state == null)
                return;

#if UNITY_ASSERTIONS
            if (!IsChildOf(state, this))
                throw new ArgumentException(
                    $"State is not a child of the mixer." +
                    $"\n - State: {state}" +
                    $"\n - Mixer: {this}",
                    nameof(state));
#endif

            var synchronizer = GetParentMixer();
            synchronizer.SynchronizeDirect(state);
        }

        /// <summary>The internal implementation of <see cref="Synchronize"/>.</summary>
        private void SynchronizeDirect(AnimancerState state)
        {
            if (state == null)
                return;

            if (state is MixerState mixer)
            {
                for (int i = 0; i < mixer._SynchronizedChildren.Count; i++)
                    Synchronize(mixer._SynchronizedChildren[i]);
                mixer._SynchronizedChildren.Clear();
                return;
            }

#if UNITY_ASSERTIONS
            if (OptionalWarning.MixerSynchronizeZeroLength.IsEnabled() && state.Length == 0)
                OptionalWarning.MixerSynchronizeZeroLength.Log(
                    $"Adding a state with zero {nameof(AnimancerState.Length)} to the synchronization list: '{state}'." +
                    $"\n\nSynchronization is based on the {nameof(NormalizedTime)}" +
                    $" which can't be calculated if the {nameof(Length)} is 0." +
                    $" Some state types can change their {nameof(Length)}, in which case you can just disable this warning." +
                    $" But otherwise, the indicated state probably shouldn't be added to the synchronization list.", Root?.Component);
#endif

            if (_SynchronizedChildren == null)
                _SynchronizedChildren = new List<AnimancerState>();

#if UNITY_ASSERTIONS
            if (_SynchronizedChildren.Contains(state))
                Debug.LogError($"{state} is already in the {nameof(SynchronizedChildren)} list.");
#endif

            _SynchronizedChildren.Add(state);
            RequireUpdate();
        }

        /************************************************************************************************************************/

        /// <summary>Removes the `state` from the <see cref="SynchronizedChildren"/>.</summary>
        public void DontSynchronize(AnimancerState state)
        {
            var synchronizer = GetParentMixer();
            if (synchronizer._SynchronizedChildren != null &&
                synchronizer._SynchronizedChildren.Remove(state))
                state._Playable.SetSpeed(state.Speed);
        }

        /************************************************************************************************************************/

        /// <summary>Removes all children of this mixer from the <see cref="SynchronizedChildren"/>.</summary>
        public void DontSynchronizeChildren()
        {
            var synchronizer = GetParentMixer();
            var SynchronizedChildren = synchronizer._SynchronizedChildren;
            if (SynchronizedChildren == null)
                return;

            if (synchronizer == this)
            {
                for (int i = SynchronizedChildren.Count - 1; i >= 0; i--)
                {
                    var state = SynchronizedChildren[i];
                    state._Playable.SetSpeed(state.Speed);
                }

                SynchronizedChildren.Clear();
            }
            else
            {
                for (int i = SynchronizedChildren.Count - 1; i >= 0; i--)
                {
                    var state = SynchronizedChildren[i];
                    if (IsChildOf(state, this))
                    {
                        state._Playable.SetSpeed(state.Speed);
                        SynchronizedChildren.RemoveAt(i);
                    }
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>Initializes the internal <see cref="SynchronizedChildren"/> list.</summary>
        /// <remarks>
        /// The array can be null or empty. Any elements not in the array will be treated as <c>true</c>.
        /// <para></para>
        /// This method can only be called before any <see cref="SynchronizedChildren"/> are added and also before this
        /// mixer is made the child of another mixer.
        /// </remarks>
        public void InitializeSynchronizedChildren(params bool[] synchronizeChildren)
        {
            AnimancerUtilities.Assert(GetParentMixer() == this,
                $"{nameof(InitializeSynchronizedChildren)} cannot be used on a mixer that is a child of another mixer.");
            AnimancerUtilities.Assert(_SynchronizedChildren == null,
                $"{nameof(InitializeSynchronizedChildren)} cannot be used on a mixer already has synchronized children.");

            int flagCount;
            if (synchronizeChildren != null)
            {
                flagCount = synchronizeChildren.Length;
                for (int i = 0; i < flagCount; i++)
                    if (synchronizeChildren[i])
                        SynchronizeDirect(GetChild(i));
            }
            else flagCount = 0;

            for (int i = flagCount; i < ChildCount; i++)
                SynchronizeDirect(GetChild(i));
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the highest <see cref="MixerState"/> in the hierarchy above this mixer or this mixer itself if
        /// there are none above it.
        /// </summary>
        public MixerState GetParentMixer()
        {
            var mixer = this;

            var parent = Parent;
            while (parent != null)
            {
                if (parent is MixerState parentMixer)
                    mixer = parentMixer;

                parent = parent.Parent;
            }

            return mixer;
        }

        /// <summary>Returns the highest <see cref="MixerState"/> in the hierarchy above the `state` (inclusive).</summary>
        public static MixerState GetParentMixer(IPlayableWrapper node)
        {
            MixerState mixer = null;

            while (node != null)
            {
                if (node is MixerState parentMixer)
                    mixer = parentMixer;

                node = node.Parent;
            }

            return mixer;
        }

        /************************************************************************************************************************/

        /// <summary>Is the `child` a child of the `parent`?</summary>
        public static bool IsChildOf(IPlayableWrapper child, IPlayableWrapper parent)
        {
            while (true)
            {
                child = child.Parent;
                if (child == parent)
                    return true;
                else if (child == null)
                    return false;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Synchronizes the <see cref="AnimancerState.NormalizedTime"/>s of the <see cref="SynchronizedChildren"/> by
        /// modifying their internal playable speeds.
        /// </summary>
        protected void ApplySynchronizeChildren(ref bool needsMoreUpdates)
        {
            if (_SynchronizedChildren == null || _SynchronizedChildren.Count <= 1)
                return;

            needsMoreUpdates = true;

            var deltaTime = AnimancerPlayable.DeltaTime * CalculateRealEffectiveSpeed();
            if (deltaTime == 0)
                return;

            var count = _SynchronizedChildren.Count;

            // Calculate the weighted average normalized time and normalized speed of all children.

            var totalWeight = 0f;
            var weightedNormalizedTime = 0f;
            var weightedNormalizedSpeed = 0f;

            for (int i = 0; i < count; i++)
            {
                var state = _SynchronizedChildren[i];

                var weight = state.Weight;
                if (weight == 0)
                    continue;

                var length = state.Length;
                if (length == 0)
                    continue;

                totalWeight += weight;

                weight /= length;

                weightedNormalizedTime += state.Time * weight;
                weightedNormalizedSpeed += state.Speed * weight;
            }

#if UNITY_ASSERTIONS
            if (!(totalWeight >= 0) || totalWeight == float.PositiveInfinity)// Reversed comparison includes NaN.
                throw new ArgumentOutOfRangeException(nameof(totalWeight), totalWeight, "Total weight must be a finite positive value");
            if (!weightedNormalizedTime.IsFinite())
                throw new ArgumentOutOfRangeException(nameof(weightedNormalizedTime), weightedNormalizedTime, "Time must be finite");
            if (!weightedNormalizedSpeed.IsFinite())
                throw new ArgumentOutOfRangeException(nameof(weightedNormalizedSpeed), weightedNormalizedSpeed, "Speed must be finite");
#endif

            // If the total weight is too small, pretend they are all at Weight = 1.
            if (totalWeight < MinimumSynchronizeChildrenWeight)
            {
                weightedNormalizedTime = 0;
                weightedNormalizedSpeed = 0;

                var nonZeroCount = 0;
                for (int i = 0; i < count; i++)
                {
                    var state = _SynchronizedChildren[i];

                    var length = state.Length;
                    if (length == 0)
                        continue;

                    length = 1f / length;

                    weightedNormalizedTime += state.Time * length;
                    weightedNormalizedSpeed += state.Speed * length;

                    nonZeroCount++;
                }

                totalWeight = nonZeroCount;
            }

            // Increment that time value according to delta time.
            weightedNormalizedTime += deltaTime * weightedNormalizedSpeed;
            weightedNormalizedTime /= totalWeight;

            var inverseDeltaTime = 1f / deltaTime;

            // Modify the speed of all children to go from their current normalized time to the average in one frame.
            for (int i = 0; i < count; i++)
            {
                var state = _SynchronizedChildren[i];
                var length = state.Length;
                if (length == 0)
                    continue;

                var normalizedTime = state.Time / length;
                var speed = (weightedNormalizedTime - normalizedTime) * length * inverseDeltaTime;
                state._Playable.SetSpeed(speed);
            }

            // After this, all the playables will update and advance according to their new speeds this frame.
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The multiplied <see cref="PlayableExtensions.GetSpeed"/> of this mixer and its parents down the
        /// hierarchy to determine the actual speed its output is being played at.
        /// </summary>
        /// <remarks>
        /// This can be different from the <see cref="AnimancerNode.EffectiveSpeed"/> because the
        /// <see cref="SynchronizedChildren"/> have their playable speed modified without setting their
        /// <see cref="AnimancerNode.Speed"/>.
        /// </remarks>
        public float CalculateRealEffectiveSpeed()
        {
            var speed = _Playable.GetSpeed() * Root.Speed;

            var parent = Parent;
            while (parent != null)
            {
                speed *= parent.Playable.GetSpeed();
                parent = parent.Parent;
            }

            return (float)speed;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Inverse Kinematics
        /************************************************************************************************************************/

        private bool _ApplyAnimatorIK;

        /// <inheritdoc/>
        public override bool ApplyAnimatorIK
        {
            get => _ApplyAnimatorIK;
            set => base.ApplyAnimatorIK = _ApplyAnimatorIK = value;
        }

        /************************************************************************************************************************/

        private bool _ApplyFootIK;

        /// <inheritdoc/>
        public override bool ApplyFootIK
        {
            get => _ApplyFootIK;
            set => base.ApplyFootIK = _ApplyFootIK = value;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Other Methods
        /************************************************************************************************************************/

        /// <summary>Calculates the sum of the <see cref="AnimancerNode.Weight"/> of all `states`.</summary>
        public float CalculateTotalWeight(IList<AnimancerState> states)
        {
            var total = 0f;

            for (int i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];
                if (state != null)
                    total += state.Weight;
            }

            return total;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sets <see cref="AnimancerState.Time"/> for all <see cref="ChildStates"/>.
        /// </summary>
        public void SetChildrenTime(float value, bool normalized = false)
        {
            var states = ChildStates;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];
                if (state == null)
                    continue;

                if (normalized)
                    state.NormalizedTime = value;
                else
                    state.Time = value;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sets the weight of all states after the `previousIndex` to 0.
        /// </summary>
        protected void DisableRemainingStates(int previousIndex)
        {
            var states = ChildStates;
            var childCount = states.Count;
            while (++previousIndex < childCount)
            {
                var state = states[previousIndex];
                if (state == null)
                    continue;

                state.Weight = 0;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the state at the specified `index` if it is not null, otherwise increments the index and checks
        /// again. Returns null if no state is found by the end of the <see cref="ChildStates"/>.
        /// </summary>
        protected AnimancerState GetNextState(ref int index)
        {
            var states = ChildStates;
            var childCount = states.Count;
            while (index < childCount)
            {
                var state = states[index];
                if (state != null)
                    return state;

                index++;
            }

            return null;
        }

        /************************************************************************************************************************/

        /// <summary>Divides the weight of all child states by the `totalWeight`.</summary>
        /// <remarks>
        /// If the `totalWeight` is equal to the total <see cref="AnimancerNode.Weight"/> of all child states, then the
        /// new total will become 1.
        /// </remarks>
        public void NormalizeWeights(float totalWeight)
        {
            if (totalWeight == 1)
                return;

            totalWeight = 1f / totalWeight;

            var states = ChildStates;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                var state = states[i];
                if (state == null)
                    continue;

                state.Weight *= totalWeight;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Gets a user-friendly key to identify the `state` in the Inspector.</summary>
        public virtual string GetDisplayKey(AnimancerState state) => $"[{state.Index}]";

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector3 AverageVelocity
        {
            get
            {
                var velocity = default(Vector3);

                RecalculateWeights();

                var childStates = ChildStates;
                for (int i = childStates.Count - 1; i >= 0; i--)
                {
                    var state = childStates[i];
                    if (state == null)
                        continue;

                    velocity += state.AverageVelocity * state.Weight;
                }

                return velocity;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Recalculates the <see cref="AnimancerState.Duration"/> of all child states so that they add up to 1.
        /// </summary>
        /// <exception cref="NullReferenceException">There are any states with no <see cref="Clip"/>.</exception>
        public void NormalizeDurations()
        {
            var childStates = ChildStates;

            int divideBy = 0;
            float totalDuration = 0f;

            // Count the number of states that exist and their total duration.
            var count = childStates.Count;
            for (int i = 0; i < count; i++)
            {
                var state = childStates[i];
                if (state == null)
                    continue;

                divideBy++;
                totalDuration += state.Duration;
            }

            // Calculate the average duration.
            totalDuration /= divideBy;

            // Set all states to that duration.
            for (int i = 0; i < count; i++)
            {
                var state = childStates[i];
                if (state == null)
                    continue;

                state.Duration = totalDuration;
            }
        }

        /************************************************************************************************************************/

#if UNITY_ASSERTIONS
        /// <summary>Has the <see cref="AnimancerNode.DebugName"/> been generated from the child states?</summary>
        private bool _IsGeneratedName;
#endif

        /// <summary>
        /// Returns a string describing the type of this mixer and the name of <see cref="Clip"/>s connected to it.
        /// </summary>
        public override string ToString()
        {
#if UNITY_ASSERTIONS
            if (DebugName != null)
                return DebugName;
#endif

            // Gather child names.
            var childNames = ObjectPool.AcquireList<string>();
            var allSimple = true;
            var states = ChildStates;
            var count = states.Count;
            for (int i = 0; i < count; i++)
            {
                var state = states[i];
                if (state == null)
                    continue;

                if (state.MainObject != null)
                {
                    childNames.Add(state.MainObject.name);
                }
                else
                {
                    childNames.Add(state.ToString());
                    allSimple = false;
                }
            }

            // If they all have a main object, check if they all have the same prefix so it doesn't need to be repeated.
            int prefixLength = 0;
            count = childNames.Count;
            if (count <= 1 || !allSimple)
            {
                prefixLength = 0;
            }
            else
            {
                var prefix = childNames[0];
                var shortest = prefixLength = prefix.Length;

                for (int iName = 0; iName < count; iName++)
                {
                    var childName = childNames[iName];

                    if (shortest > childName.Length)
                    {
                        shortest = prefixLength = childName.Length;
                    }

                    for (int iCharacter = 0; iCharacter < prefixLength; iCharacter++)
                    {
                        if (childName[iCharacter] != prefix[iCharacter])
                        {
                            prefixLength = iCharacter;
                            break;
                        }
                    }
                }

                if (prefixLength < 3 ||// Less than 3 characters probably isn't an intentional prefix.
                    prefixLength >= shortest)
                    prefixLength = 0;
            }

            // Build the mixer name.
            var mixerName = ObjectPool.AcquireStringBuilder();

            if (count > 0)
            {
                if (prefixLength > 0)
                    mixerName.Append(childNames[0], 0, prefixLength).Append('[');

                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                        mixerName.Append(", ");

                    var childName = childNames[i];
                    mixerName.Append(childName, prefixLength, childName.Length - prefixLength);
                }

                mixerName.Append(prefixLength > 0 ? "] (" : " (");
            }

            ObjectPool.Release(childNames);

            var type = GetType().FullName;
            if (type.EndsWith("State"))
                mixerName.Append(type, 0, type.Length - 5);
            else
                mixerName.Append(type);

            if (count > 0)
                mixerName.Append(')');

            var result = mixerName.ReleaseToString();

#if UNITY_ASSERTIONS
            _IsGeneratedName = true;
            SetDebugName(result);
#endif

            return result;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void AppendDetails(StringBuilder text, string separator)
        {
            base.AppendDetails(text, separator);

            text.Append(separator).Append("SynchronizedChildren: ");
            if (SynchronizedChildCount == 0)
            {
                text.Append("0");
            }
            else
            {
                text.Append(_SynchronizedChildren.Count);
                separator += Strings.Indent;
                for (int i = 0; i < _SynchronizedChildren.Count; i++)
                {
                    text.Append(separator)
                        .Append(_SynchronizedChildren[i]);
                }
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void GatherAnimationClips(ICollection<AnimationClip> clips) => clips.GatherFromSource(ChildStates);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

