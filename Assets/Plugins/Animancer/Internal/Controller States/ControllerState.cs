// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Animancer
{
    /// <summary>[Pro-Only] An <see cref="AnimancerState"/> which plays a <see cref="RuntimeAnimatorController"/>.</summary>
    /// <remarks>
    /// You can control this state very similarly to an <see cref="Animator"/> via its <see cref="Playable"/> property.
    /// <para></para>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/animator-controllers">Animator Controllers</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerState
    /// 
    public class ControllerState : AnimancerState
    {
        /************************************************************************************************************************/

        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="ControllerState"/>.</summary>
        public interface ITransition : ITransition<ControllerState> { }

        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        private RuntimeAnimatorController _Controller;

        /// <summary>The <see cref="RuntimeAnimatorController"/> which this state plays.</summary>
        public RuntimeAnimatorController Controller
        {
            get => _Controller;
            set => ChangeMainObject(ref _Controller, value);
        }

        /// <summary>The <see cref="RuntimeAnimatorController"/> which this state plays.</summary>
        public override Object MainObject
        {
            get => Controller;
            set => Controller = (RuntimeAnimatorController)value;
        }

        /// <summary>The internal system which plays the <see cref="RuntimeAnimatorController"/>.</summary>
        public AnimatorControllerPlayable Playable
        {
            get
            {
                Validate.AssertPlayable(this);
                return _Playable;
            }
        }

        private new AnimatorControllerPlayable _Playable;

        /************************************************************************************************************************/

        private bool _KeepStateOnStop;

        /// <summary>
        /// If false, <see cref="Stop"/> will reset all layers to their default state. Default False.
        /// <para></para>
        /// The <see cref="DefaultStateHashes"/> will only be gathered the first time this property is set to false or
        /// <see cref="GatherDefaultStates"/> is called manually.
        /// </summary>
        public bool KeepStateOnStop
        {
            get => _KeepStateOnStop;
            set
            {
                _KeepStateOnStop = value;
                if (!value && DefaultStateHashes == null && _Playable.IsValid())
                    GatherDefaultStates();
            }
        }

        /// <summary>
        /// The <see cref="AnimatorStateInfo.shortNameHash"/> of the default state on each layer, used to reset to
        /// those states when <see cref="Stop"/> is called if <see cref="KeepStateOnStop"/> is true.
        /// </summary>
        public int[] DefaultStateHashes { get; set; }

        /************************************************************************************************************************/

#if UNITY_ASSERTIONS
        /// <summary>[Assert-Only] Animancer Events doesn't work properly on <see cref="ControllerState"/>s.</summary>
        protected override string UnsupportedEventsMessage =>
            "Animancer Events on " + nameof(ControllerState) + "s will probably not work as expected." +
            " The events will be associated with the entire Animator Controller and be triggered by any of the" +
            " states inside it. If you want to use events in an Animator Controller you will likely need to use" +
            " Unity's regular Animation Event system.";

        /// <summary>[Assert-Only]
        /// <see cref="PlayableExtensions.SetSpeed"/> does nothing on <see cref="ControllerState"/>s.
        /// </summary>
        protected override string UnsupportedSpeedMessage =>
            nameof(PlayableExtensions) + "." + nameof(PlayableExtensions.SetSpeed) + " does nothing on " + nameof(ControllerState) +
            "s so there is no way to directly control their speed." +
            " The Animator Controller Speed page explains a possible workaround for this issue:" +
            " https://kybernetik.com.au/animancer/docs/bugs/animator-controller-speed";
#endif

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="ControllerState"/>.</summary>
        public override void CopyIKFlags(AnimancerNode node) { }

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="ControllerState"/>.</summary>
        public override bool ApplyAnimatorIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log($"IK cannot be dynamically enabled on a {nameof(ControllerState)}." +
                        " You must instead enable it on the desired layer inside the Animator Controller.", _Controller);
#endif
            }
        }

        /************************************************************************************************************************/

        /// <summary>IK cannot be dynamically enabled on a <see cref="ControllerState"/>.</summary>
        public override bool ApplyFootIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log($"IK cannot be dynamically enabled on a {nameof(ControllerState)}." +
                        " You must instead enable it on the desired state inside the Animator Controller.", _Controller);
#endif
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Public API
        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerState"/> to play the `controller`.</summary>
        public ControllerState(RuntimeAnimatorController controller, bool keepStateOnStop = false)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            _Controller = controller;
            _KeepStateOnStop = keepStateOnStop;
        }

        /************************************************************************************************************************/

        /// <summary>Creates and assigns the <see cref="AnimatorControllerPlayable"/> managed by this state.</summary>
        protected override void CreatePlayable(out Playable playable)
        {
            playable = _Playable = AnimatorControllerPlayable.Create(Root._Graph, _Controller);

            if (!_KeepStateOnStop)
                GatherDefaultStates();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Stores the values of all parameters, calls <see cref="AnimancerNode.DestroyPlayable"/>, then restores the
        /// parameter values.
        /// </summary>
        public override void RecreatePlayable()
        {
            if (!_Playable.IsValid())
            {
                CreatePlayable();
                return;
            }

            var parameterCount = _Playable.GetParameterCount();
            var values = new object[parameterCount];
            for (int i = 0; i < parameterCount; i++)
            {
                values[i] = AnimancerUtilities.GetParameterValue(_Playable, _Playable.GetParameter(i));
            }

            base.RecreatePlayable();

            for (int i = 0; i < parameterCount; i++)
            {
                AnimancerUtilities.SetParameterValue(_Playable, _Playable.GetParameter(i), values[i]);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The current state on layer 0, or the next state if it is currently in a transition.
        /// </summary>
        public AnimatorStateInfo StateInfo
        {
            get
            {
                Validate.AssertPlayable(this);
                return _Playable.IsInTransition(0) ?
                    _Playable.GetNextAnimatorStateInfo(0) :
                    _Playable.GetCurrentAnimatorStateInfo(0);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="AnimatorStateInfo.normalizedTime"/> * <see cref="AnimatorStateInfo.length"/> of the
        /// <see cref="StateInfo"/>.
        /// </summary>
        protected override float RawTime
        {
            get
            {
                var info = StateInfo;
                return info.normalizedTime * info.length;
            }
            set
            {
                Validate.AssertPlayable(this);
                _Playable.PlayInFixedTime(0, 0, value);
            }
        }

        /************************************************************************************************************************/

        /// <summary>The current <see cref="AnimatorStateInfo.length"/> (on layer 0).</summary>
        public override float Length => StateInfo.length;

        /************************************************************************************************************************/

        /// <summary>Indicates whether the current state on layer 0 will loop back to the start when it reaches the end.</summary>
        public override bool IsLooping => StateInfo.loop;

        /************************************************************************************************************************/

        /// <summary>Gathers the <see cref="DefaultStateHashes"/> from the current states.</summary>
        public void GatherDefaultStates()
        {
            Validate.AssertPlayable(this);
            var layerCount = _Playable.GetLayerCount();
            if (DefaultStateHashes == null || DefaultStateHashes.Length != layerCount)
                DefaultStateHashes = new int[layerCount];

            while (--layerCount >= 0)
                DefaultStateHashes[layerCount] = _Playable.GetCurrentAnimatorStateInfo(layerCount).shortNameHash;
        }

        /// <summary>
        /// Calls the base <see cref="AnimancerState.Stop"/> and if <see cref="KeepStateOnStop"/> is false it also
        /// calls <see cref="ResetToDefaultStates"/>.
        /// </summary>
        public override void Stop()
        {
            if (_KeepStateOnStop)
            {
                base.Stop();
            }
            else
            {
                ResetToDefaultStates();

                // Don't call base.Stop(); because it sets Time = 0; which uses PlayInFixedTime and interferes with
                // resetting to the default states.
                Weight = 0;
                IsPlaying = false;
                Events = null;
            }
        }

        /// <summary>
        /// Resets all layers to their default state.
        /// </summary>
        /// <exception cref="NullReferenceException"><see cref="DefaultStateHashes"/> is null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The size of <see cref="DefaultStateHashes"/> is larger than the number of layers in the
        /// <see cref="Controller"/>.
        /// </exception>
        public void ResetToDefaultStates()
        {
            Validate.AssertPlayable(this);
            for (int i = DefaultStateHashes.Length - 1; i >= 0; i--)
                _Playable.Play(DefaultStateHashes[i], i, 0);

            // Allowing the RawTime to be applied prevents the default state from being played because
            // Animator Controllers don't properly respond to multiple Play calls in the same frame.
            CancelSetTime();
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            if (_Controller != null)
                clips.Gather(_Controller.animationClips);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Destroy()
        {
            _Controller = null;
            base.Destroy();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Animator Controller Wrappers
        /************************************************************************************************************************/
        #region Cross Fade
        /************************************************************************************************************************/

        /// <summary>
        /// The default constant for fade duration parameters which causes it to use the
        /// <see cref="AnimancerPlayable.DefaultFadeDuration"/> instead.
        /// </summary>
        public const float DefaultFadeDuration = -1;

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the `fadeDuration` if it is zero or positive. Otherwise returns the
        /// <see cref="AnimancerPlayable.DefaultFadeDuration"/>.
        /// </summary>
        public static float GetFadeDuration(float fadeDuration)
            => fadeDuration >= 0 ? fadeDuration : AnimancerPlayable.DefaultFadeDuration;

        /************************************************************************************************************************/

        /// <summary>Starts a transition from the current state to the specified state using normalized times.</summary>
        /// <remarks>If `fadeDuration` is negative, it uses the <see cref="AnimancerPlayable.DefaultFadeDuration"/>.</remarks>
        public void CrossFade(int stateNameHash,
            float fadeDuration = DefaultFadeDuration,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
            => Playable.CrossFade(stateNameHash, GetFadeDuration(fadeDuration), layer, normalizedTime);

        /************************************************************************************************************************/

        /// <summary>Starts a transition from the current state to the specified state using normalized times.</summary>
        /// <remarks>If `fadeDuration` is negative, it uses the <see cref="AnimancerPlayable.DefaultFadeDuration"/>.</remarks>
        public void CrossFade(string stateName,
            float fadeDuration = DefaultFadeDuration,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
            => Playable.CrossFade(stateName, GetFadeDuration(fadeDuration), layer, normalizedTime);

        /************************************************************************************************************************/

        /// <summary>Starts a transition from the current state to the specified state using times in seconds.</summary>
        /// <remarks>If `fadeDuration` is negative, it uses the <see cref="AnimancerPlayable.DefaultFadeDuration"/>.</remarks>
        public void CrossFadeInFixedTime(int stateNameHash,
            float fadeDuration = DefaultFadeDuration,
            int layer = -1,
            float fixedTime = 0)
            => Playable.CrossFadeInFixedTime(stateNameHash, GetFadeDuration(fadeDuration), layer, fixedTime);

        /************************************************************************************************************************/

        /// <summary>Starts a transition from the current state to the specified state using times in seconds.</summary>
        /// <remarks>If `fadeDuration` is negative, it uses the <see cref="AnimancerPlayable.DefaultFadeDuration"/>.</remarks>
        public void CrossFadeInFixedTime(string stateName,
            float fadeDuration = DefaultFadeDuration,
            int layer = -1,
            float fixedTime = 0)
            => Playable.CrossFadeInFixedTime(stateName, GetFadeDuration(fadeDuration), layer, fixedTime);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Play
        /************************************************************************************************************************/

        /// <summary>Plays the specified state immediately, starting from a particular normalized time.</summary>
        public void Play(int stateNameHash,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
            => Playable.Play(stateNameHash, layer, normalizedTime);

        /************************************************************************************************************************/

        /// <summary>Plays the specified state immediately, starting from a particular normalized time.</summary>
        public void Play(string stateName,
            int layer = -1,
            float normalizedTime = float.NegativeInfinity)
            => Playable.Play(stateName, layer, normalizedTime);

        /************************************************************************************************************************/

        /// <summary>Plays the specified state immediately, starting from a particular time (in seconds).</summary>
        public void PlayInFixedTime(int stateNameHash,
            int layer = -1,
            float fixedTime = 0)
            => Playable.PlayInFixedTime(stateNameHash, layer, fixedTime);

        /************************************************************************************************************************/

        /// <summary>Plays the specified state immediately, starting from a particular time (in seconds).</summary>
        public void PlayInFixedTime(string stateName,
            int layer = -1,
            float fixedTime = 0)
            => Playable.PlayInFixedTime(stateName, layer, fixedTime);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Parameters
        /************************************************************************************************************************/

        /// <summary>Gets the value of the specified boolean parameter.</summary>
        public bool GetBool(int id) => Playable.GetBool(id);
        /// <summary>Gets the value of the specified boolean parameter.</summary>
        public bool GetBool(string name) => Playable.GetBool(name);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public void SetBool(int id, bool value) => Playable.SetBool(id, value);
        /// <summary>Sets the value of the specified boolean parameter.</summary>
        public void SetBool(string name, bool value) => Playable.SetBool(name, value);

        /// <summary>Gets the value of the specified float parameter.</summary>
        public float GetFloat(int id) => Playable.GetFloat(id);
        /// <summary>Gets the value of the specified float parameter.</summary>
        public float GetFloat(string name) => Playable.GetFloat(name);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public void SetFloat(int id, float value) => Playable.SetFloat(id, value);
        /// <summary>Sets the value of the specified float parameter.</summary>
        public void SetFloat(string name, float value) => Playable.SetFloat(name, value);

        /// <summary>Gets the value of the specified integer parameter.</summary>
        public int GetInteger(int id) => Playable.GetInteger(id);
        /// <summary>Gets the value of the specified integer parameter.</summary>
        public int GetInteger(string name) => Playable.GetInteger(name);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public void SetInteger(int id, int value) => Playable.SetInteger(id, value);
        /// <summary>Sets the value of the specified integer parameter.</summary>
        public void SetInteger(string name, int value) => Playable.SetInteger(name, value);

        /// <summary>Sets the specified trigger parameter to true.</summary>
        public void SetTrigger(int id) => Playable.SetTrigger(id);
        /// <summary>Sets the specified trigger parameter to true.</summary>
        public void SetTrigger(string name) => Playable.SetTrigger(name);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public void ResetTrigger(int id) => Playable.ResetTrigger(id);
        /// <summary>Resets the specified trigger parameter to false.</summary>
        public void ResetTrigger(string name) => Playable.ResetTrigger(name);

        /// <summary>Gets the details of one of the <see cref="Controller"/>'s parameters.</summary>
        public AnimatorControllerParameter GetParameter(int index) => Playable.GetParameter(index);
        /// <summary>Gets the number of parameters in the <see cref="Controller"/>.</summary>
        public int GetParameterCount() => Playable.GetParameterCount();

        /// <summary>Indicates whether the specified parameter is controlled by an <see cref="AnimationClip"/>.</summary>
        public bool IsParameterControlledByCurve(int id) => Playable.IsParameterControlledByCurve(id);
        /// <summary>Indicates whether the specified parameter is controlled by an <see cref="AnimationClip"/>.</summary>
        public bool IsParameterControlledByCurve(string name) => Playable.IsParameterControlledByCurve(name);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Misc
        /************************************************************************************************************************/
        // Layers.
        /************************************************************************************************************************/

        /// <summary>Gets the weight of the layer at the specified index.</summary>
        public float GetLayerWeight(int layerIndex) => Playable.GetLayerWeight(layerIndex);
        /// <summary>Sets the weight of the layer at the specified index.</summary>
        public void SetLayerWeight(int layerIndex, float weight) => Playable.SetLayerWeight(layerIndex, weight);

        /// <summary>Gets the number of layers in the <see cref="Controller"/>.</summary>
        public int GetLayerCount() => Playable.GetLayerCount();

        /// <summary>Gets the index of the layer with the specified name.</summary>
        public int GetLayerIndex(string layerName) => Playable.GetLayerIndex(layerName);
        /// <summary>Gets the name of the layer with the specified index.</summary>
        public string GetLayerName(int layerIndex) => Playable.GetLayerName(layerIndex);

        /************************************************************************************************************************/
        // States.
        /************************************************************************************************************************/

        /// <summary>Returns information about the current state.</summary>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex = 0) => Playable.GetCurrentAnimatorStateInfo(layerIndex);
        /// <summary>Returns information about the next state being transitioned towards.</summary>
        public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex = 0) => Playable.GetNextAnimatorStateInfo(layerIndex);

        /// <summary>Indicates whether the specified layer contains the specified state.</summary>
        public bool HasState(int layerIndex, int stateID) => Playable.HasState(layerIndex, stateID);

        /************************************************************************************************************************/
        // Transitions.
        /************************************************************************************************************************/

        /// <summary>Indicates whether the specified layer is currently executing a transition.</summary>
        public bool IsInTransition(int layerIndex = 0) => Playable.IsInTransition(layerIndex);

        /// <summary>Gets information about the current transition.</summary>
        public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex = 0) => Playable.GetAnimatorTransitionInfo(layerIndex);

        /************************************************************************************************************************/
        // Clips.
        /************************************************************************************************************************/

        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being played.</summary>
        public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex = 0) => Playable.GetCurrentAnimatorClipInfo(layerIndex);
        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being played.</summary>
        public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips) => Playable.GetCurrentAnimatorClipInfo(layerIndex, clips);
        /// <summary>Gets the number of <see cref="AnimationClip"/>s currently being played.</summary>
        public int GetCurrentAnimatorClipInfoCount(int layerIndex = 0) => Playable.GetCurrentAnimatorClipInfoCount(layerIndex);

        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex = 0) => Playable.GetNextAnimatorClipInfo(layerIndex);
        /// <summary>Gets information about the <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips) => Playable.GetNextAnimatorClipInfo(layerIndex, clips);
        /// <summary>Gets the number of <see cref="AnimationClip"/>s currently being transitioned towards.</summary>
        public int GetNextAnimatorClipInfoCount(int layerIndex = 0) => Playable.GetNextAnimatorClipInfoCount(layerIndex);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Parameter IDs
        /************************************************************************************************************************/

        /// <summary>A wrapper for the name and hash of an <see cref="AnimatorControllerParameter"/>.</summary>
        public readonly struct ParameterID
        {
            /************************************************************************************************************************/

            /// <summary>The name of this parameter.</summary>
            public readonly string Name;

            /// <summary>The name hash of this parameter.</summary>
            public readonly int Hash;

            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="ParameterID"/> with the specified <see cref="Name"/> and uses
            /// <see cref="Animator.StringToHash"/> to calculate the <see cref="Hash"/>.
            /// </summary>
            public ParameterID(string name)
            {
                Name = name;
                Hash = Animator.StringToHash(name);
            }

            /// <summary>
            /// Creates a new <see cref="ParameterID"/> with the specified <see cref="Hash"/> and leaves the
            /// <see cref="Name"/> null.
            /// </summary>
            public ParameterID(int hash)
            {
                Name = null;
                Hash = hash;
            }

            /// <summary>Creates a new <see cref="ParameterID"/> with the specified <see cref="Name"/> and <see cref="Hash"/>.</summary>
            /// <remarks>This constructor does not verify that the `hash` actually corresponds to the `name`.</remarks>
            public ParameterID(string name, int hash)
            {
                Name = name;
                Hash = hash;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Creates a new <see cref="ParameterID"/> with the specified <see cref="Name"/> and uses
            /// <see cref="Animator.StringToHash"/> to calculate the <see cref="Hash"/>.
            /// </summary>
            public static implicit operator ParameterID(string name) => new ParameterID(name);

            /// <summary>
            /// Creates a new <see cref="ParameterID"/> with the specified <see cref="Hash"/> and leaves the
            /// <see cref="Name"/> null.
            /// </summary>
            public static implicit operator ParameterID(int hash) => new ParameterID(hash);

            /************************************************************************************************************************/

            /// <summary>Returns the <see cref="Hash"/>.</summary>
            public static implicit operator int(ParameterID parameter) => parameter.Hash;

            /************************************************************************************************************************/

#if UNITY_EDITOR
            private static Dictionary<RuntimeAnimatorController, Dictionary<int, AnimatorControllerParameterType>>
                _ControllerToParameterHashAndType;
#endif

            /// <summary>[Editor-Conditional]
            /// Throws if the `controller` doesn't have a parameter with the specified <see cref="Hash"/>
            /// and `type`.
            /// </summary>
            /// <exception cref="ArgumentException"/>
            [System.Diagnostics.Conditional(Strings.UnityEditor)]
            public void ValidateHasParameter(RuntimeAnimatorController controller, AnimatorControllerParameterType type)
            {
#if UNITY_EDITOR
                Editor.AnimancerEditorUtilities.InitializeCleanDictionary(ref _ControllerToParameterHashAndType);

                // Get the parameter details.
                if (!_ControllerToParameterHashAndType.TryGetValue(controller, out var parameterDetails))
                {
                    var editorController = (AnimatorController)controller;
                    var parameters = editorController.parameters;
                    var count = parameters.Length;

                    // Animator Controllers loaded from Asset Bundles only contain their RuntimeAnimatorController data
                    // but not the editor AnimatorController data which we need to perform this validation.
                    if (count == 0 &&
                        editorController.layers.Length == 0)// Double check that the editor data is actually empty.
                    {
                        _ControllerToParameterHashAndType.Add(controller, null);
                        return;
                    }

                    parameterDetails = new Dictionary<int, AnimatorControllerParameterType>();

                    for (int i = 0; i < count; i++)
                    {
                        var parameter = parameters[i];
                        parameterDetails.Add(parameter.nameHash, parameter.type);
                    }

                    _ControllerToParameterHashAndType.Add(controller, parameterDetails);
                }

                if (parameterDetails == null)
                    return;

                // Check that there is a parameter with the correct hash and type.

                if (!parameterDetails.TryGetValue(Hash, out var parameterType))
                {
                    throw new ArgumentException($"{controller} has no {type} parameter matching {this}");
                }

                if (type != parameterType)
                {
                    throw new ArgumentException($"{controller} has a parameter matching {this}, but it is not a {type}");
                }
#endif
            }

            /************************************************************************************************************************/

            /// <summary>Returns a string containing the <see cref="Name"/> and <see cref="Hash"/>.</summary>
            public override string ToString()
            {
                return $"{nameof(ControllerState)}.{nameof(ParameterID)}" +
                    $"({nameof(Name)}: '{Name}'" +
                    $", {nameof(Hash)}: {Hash})";
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Inspector
        /************************************************************************************************************************/

        /// <summary>The number of parameters being wrapped by this state.</summary>
        public virtual int ParameterCount => 0;

        /// <summary>Returns the hash of a parameter being wrapped by this state.</summary>
        /// <exception cref="NotSupportedException">This state doesn't wrap any parameters.</exception>
        public virtual int GetParameterHash(int index) => throw new NotSupportedException();

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Returns a <see cref="Drawer"/> for this state.</summary>
        protected internal override Editor.IAnimancerNodeDrawer CreateDrawer() => new Drawer(this);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public sealed class Drawer : Editor.ParametizedAnimancerStateDrawer<ControllerState>
        {
            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="Drawer"/> to manage the Inspector GUI for the `state`.</summary>
            public Drawer(ControllerState state) : base(state) { }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            protected override void DoDetailsGUI()
            {
                GatherParameters();
                base.DoDetailsGUI();
            }

            /************************************************************************************************************************/

            private readonly List<AnimatorControllerParameter>
                Parameters = new List<AnimatorControllerParameter>();

            /// <summary>Fills the <see cref="Parameters"/> list with the current parameter details.</summary>
            private void GatherParameters()
            {
                Parameters.Clear();

                var count = Target.ParameterCount;
                if (count == 0)
                    return;

                for (int i = 0; i < count; i++)
                {
                    var hash = Target.GetParameterHash(i);
                    Parameters.Add(GetParameter(hash));
                }
            }

            /************************************************************************************************************************/

            private AnimatorControllerParameter GetParameter(int hash)
            {
                Validate.AssertPlayable(Target);
                var parameterCount = Target._Playable.GetParameterCount();
                for (int i = 0; i < parameterCount; i++)
                {
                    var parameter = Target._Playable.GetParameter(i);
                    if (parameter.nameHash == hash)
                        return parameter;
                }

                return null;
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override int ParameterCount => Parameters.Count;

            /// <inheritdoc/>
            public override string GetParameterName(int index) => Parameters[index].name;

            /// <inheritdoc/>
            public override AnimatorControllerParameterType GetParameterType(int index) => Parameters[index].type;

            /// <inheritdoc/>
            public override object GetParameterValue(int index)
            {
                Validate.AssertPlayable(Target);
                return AnimancerUtilities.GetParameterValue(Target._Playable, Parameters[index]);
            }

            /// <inheritdoc/>
            public override void SetParameterValue(int index, object value)
            {
                Validate.AssertPlayable(Target);
                AnimancerUtilities.SetParameterValue(Target._Playable, Parameters[index], value);
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

