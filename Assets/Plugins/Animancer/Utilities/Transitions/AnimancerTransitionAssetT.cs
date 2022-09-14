// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace Animancer
{
    /// <summary>A <see cref="ScriptableObject"/> based <see cref="ITransition"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/assets">Transition Assets</see>
    /// <para></para>
    /// When adding a <see cref="CreateAssetMenuAttribute"/> to any derived classes, you can use
    /// <see cref="Strings.MenuPrefix"/> and <see cref="Strings.AssetMenuOrder"/>.
    /// <para></para>
    /// If you are using <see cref="AnimancerEvent"/>s, consider using an
    /// <see cref="AnimancerTransitionAsset.UnShared{TAsset, TTransition, TState}"/> instead of referencing this asset
    /// directly in order to avoid common issues with shared events.
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/AnimancerTransitionAsset_1
    /// 
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(AnimancerTransitionAsset<ITransition>) + "_1")]
    public class AnimancerTransitionAsset<TTransition> : ScriptableObject, ITransition, IWrapper, IAnimationClipSource
        where TTransition : ITransition
    {
        /************************************************************************************************************************/

        [SerializeReference]
        private TTransition _Transition;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.
        /// </summary>
        /// <remarks>
        /// WARNING: the <see cref="AnimancerTransition{TState}.State"/> holds the most recently played state, so
        /// if you are sharing this transition between multiple objects it will only remember one of them.
        /// Consider using <see cref="AnimancerTransitionAsset.UnShared{TAsset, TTransition, TState}"/>.
        /// <para></para>
        /// You can use <see cref="AnimancerPlayable.StateDictionary.GetOrCreate(ITransition)"/> or
        /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/> to get or create the state for a
        /// specific object.
        /// </remarks>
        public ref TTransition Transition => ref _Transition;

        /// <summary>Returns the <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.</summary>
        public virtual ITransition GetTransition() => _Transition;

        /// <inheritdoc/>
        object IWrapper.WrappedObject => GetTransition();

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] Assigns a default <typeparamref name="TTransition"/> to the <see cref="Transition"/> field.</summary>
        protected virtual void Reset()
        {
            _Transition = Editor.TypeSelectionButton.CreateDefaultInstance<TTransition>();
        }
#endif

        /************************************************************************************************************************/

        /// <summary>Can this transition create a valid <see cref="AnimancerState"/>?</summary>
        public virtual bool IsValid => GetTransition().IsValid();

        /// <inheritdoc/>
        public virtual float FadeDuration => GetTransition().FadeDuration;

        /// <inheritdoc/>
        public virtual object Key => GetTransition().Key;

        /// <inheritdoc/>
        public virtual FadeMode FadeMode => GetTransition().FadeMode;

        /// <inheritdoc/>
        public virtual AnimancerState CreateState() => GetTransition().CreateState();

        /// <inheritdoc/>
        public virtual void Apply(AnimancerState state)
        {
            GetTransition().Apply(state);
            state.SetDebugName(name);
        }

        /************************************************************************************************************************/

        /// <summary>[<see cref="IAnimationClipSource"/>]
        /// Calls <see cref="AnimancerUtilities.GatherFromSource(ICollection{AnimationClip}, object)"/>.
        /// </summary>
        public virtual void GetAnimationClips(List<AnimationClip> clips) => clips.GatherFromSource(GetTransition());

        /************************************************************************************************************************/
    }
}

/************************************************************************************************************************/

#if UNITY_EDITOR
namespace Animancer.Editor
{
    /// <summary>A custom editor for <see cref="AnimancerTransitionAsset"/>s.</summary>
    /// <remarks>
    /// This class contains several context menu functions for generating <see cref="AnimancerTransitionAsset"/>s based on
    /// Animator Controller States.
    /// </remarks>
    [CustomEditor(typeof(AnimancerTransitionAsset<>), true), CanEditMultipleObjects]
    internal class AnimancerTransitionAssetEditor : ScriptableObjectEditor
    {
        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransition"/> from the <see cref="MenuCommand.context"/>.</summary>
        [MenuItem("CONTEXT/" + nameof(AnimatorState) + "/Generate Transition")]
        [MenuItem("CONTEXT/" + nameof(BlendTree) + "/Generate Transition")]
        [MenuItem("CONTEXT/" + nameof(AnimatorStateTransition) + "/Generate Transition")]
        [MenuItem("CONTEXT/" + nameof(AnimatorStateMachine) + "/Generate Transitions")]
        private static void GenerateTransition(MenuCommand command)
        {
            var context = command.context;
            if (context is AnimatorState state)
            {
                Selection.activeObject = GenerateTransition(state);
            }
            else if (context is BlendTree blendTree)
            {
                Selection.activeObject = GenerateTransition(null, blendTree);
            }
            else if (context is AnimatorStateTransition transition)
            {
                Selection.activeObject = GenerateTransition(transition);
            }
            else if (context is AnimatorStateMachine stateMachine)// Layer or Sub-State Machine.
            {
                Selection.activeObject = GenerateTransitions(stateMachine);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransitionAsset"/> from the `state`.</summary>
        private static Object GenerateTransition(AnimatorState state)
            => GenerateTransition(state, state.motion);

        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransitionAsset"/> from the `motion`.</summary>
        private static Object GenerateTransition(Object originalAsset, Motion motion)
        {
            if (motion is BlendTree blendTree)
            {
                return GenerateTransition(originalAsset as AnimatorState, blendTree);
            }
            else if (motion is AnimationClip || motion == null)
            {
                var asset = CreateInstance<ClipTransitionAsset>();
                asset.Transition = new ClipTransition
                {
                    Clip = (AnimationClip)motion,
                };

                GetDetailsFromState(originalAsset as AnimatorState, asset.Transition);
                SaveTransition(originalAsset, asset);
                return asset;
            }
            else
            {
                Debug.LogError($"Unsupported {nameof(Motion)} Type: {motion.GetType()}");
                return null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Initializes the `transition` based on the `state`.</summary>
        private static void GetDetailsFromState(AnimatorState state, ITransitionDetailed transition)
        {
            if (state == null ||
                transition == null)
                return;

            transition.Speed = state.speed;

            var isForwards = state.speed >= 0;
            var defaultEndTime = AnimancerEvent.Sequence.GetDefaultNormalizedEndTime(state.speed);
            var endTime = defaultEndTime;

            var exitTransitions = state.transitions;
            for (int i = 0; i < exitTransitions.Length; i++)
            {
                var exitTransition = exitTransitions[i];
                if (exitTransition.hasExitTime)
                {
                    if (isForwards)
                    {
                        if (endTime > exitTransition.exitTime)
                            endTime = exitTransition.exitTime;
                    }
                    else
                    {
                        if (endTime < exitTransition.exitTime)
                            endTime = exitTransition.exitTime;
                    }
                }
            }

            if (endTime != defaultEndTime && AnimancerUtilities.TryGetWrappedObject(transition, out IHasEvents events))
            {
                if (events.SerializedEvents == null)
                    events.SerializedEvents = new AnimancerEvent.Sequence.Serializable();
                events.SerializedEvents.SetNormalizedEndTime(endTime);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransition"/> from the `blendTree`.</summary>
        private static Object GenerateTransition(AnimatorState state, BlendTree blendTree)
        {
            var asset = CreateTransition(blendTree);
            if (asset == null)
                return null;

            if (state != null)
                asset.name = state.name;

            AnimancerUtilities.TryGetWrappedObject(asset, out ITransitionDetailed detailed);
            GetDetailsFromState(state, detailed);
            SaveTransition(blendTree, asset);
            return asset;
        }

        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransition"/> from the `transition`.</summary>
        private static Object GenerateTransition(AnimatorStateTransition transition)
        {
            Object animancerTransition = null;

            if (transition.destinationStateMachine != null)
                animancerTransition = GenerateTransitions(transition.destinationStateMachine);

            if (transition.destinationState != null)
                animancerTransition = GenerateTransition(transition.destinationState);

            return animancerTransition;
        }

        /************************************************************************************************************************/

        /// <summary>Creates <see cref="AnimancerTransition"/>s from all states in the `stateMachine`.</summary>
        private static Object GenerateTransitions(AnimatorStateMachine stateMachine)
        {
            Object transition = null;

            foreach (var child in stateMachine.stateMachines)
                transition = GenerateTransitions(child.stateMachine);

            foreach (var child in stateMachine.states)
                transition = GenerateTransition(child.state);

            return transition;
        }

        /************************************************************************************************************************/

        /// <summary>Creates an <see cref="AnimancerTransition"/> from the `blendTree`.</summary>
        private static Object CreateTransition(BlendTree blendTree)
        {
            switch (blendTree.blendType)
            {
                case BlendTreeType.Simple1D:
                    var linearAsset = CreateInstance<LinearMixerTransitionAsset>();
                    InitializeChildren(ref linearAsset.Transition, blendTree);
                    return linearAsset;

                case BlendTreeType.SimpleDirectional2D:
                case BlendTreeType.FreeformDirectional2D:
                    var directionalAsset = CreateInstance<MixerTransition2DAsset>();
                    directionalAsset.Transition = new MixerTransition2D
                    {
                        Type = MixerTransition2D.MixerType.Directional
                    };
                    InitializeChildren(ref directionalAsset.Transition, blendTree);
                    return directionalAsset;

                case BlendTreeType.FreeformCartesian2D:
                    var cartesianAsset = CreateInstance<MixerTransition2DAsset>();
                    cartesianAsset.Transition = new MixerTransition2D
                    {
                        Type = MixerTransition2D.MixerType.Cartesian
                    };
                    InitializeChildren(ref cartesianAsset.Transition, blendTree);
                    return cartesianAsset;

                case BlendTreeType.Direct:
                    var manualAsset = CreateInstance<ManualMixerTransitionAsset>();
                    InitializeChildren<ManualMixerTransition, ManualMixerState>(ref manualAsset.Transition, blendTree);
                    return manualAsset;

                default:
                    Debug.LogError($"Unsupported {nameof(BlendTreeType)}: {blendTree.blendType}");
                    return null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Initializes the `transition` based on the <see cref="BlendTree.children"/>.</summary>
        private static void InitializeChildren(ref LinearMixerTransition transition, BlendTree blendTree)
        {
            var children = InitializeChildren<LinearMixerTransition, LinearMixerState>(ref transition, blendTree);
            transition.Thresholds = new float[children.Length];
            for (int i = 0; i < children.Length; i++)
                transition.Thresholds[i] = children[i].threshold;
        }

        /// <summary>Initializes the `transition` based on the <see cref="BlendTree.children"/>.</summary>
        private static void InitializeChildren(ref MixerTransition2D transition, BlendTree blendTree)
        {
            var children = InitializeChildren<MixerTransition2D, MixerState<Vector2>>(ref transition, blendTree);
            transition.Thresholds = new Vector2[children.Length];
            for (int i = 0; i < children.Length; i++)
                transition.Thresholds[i] = children[i].position;
        }

        /// <summary>Initializes the `transition` based on the <see cref="BlendTree.children"/>.</summary>
        private static ChildMotion[] InitializeChildren<TTransition, TState>(ref TTransition transition, BlendTree blendTree)
            where TTransition : ManualMixerTransition<TState>, new()
            where TState : ManualMixerState
        {
            transition = new TTransition();

            var children = blendTree.children;
            transition.States = new Object[children.Length];
            float[] speeds = new float[children.Length];
            var hasCustomSpeeds = false;

            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                transition.States[i] = child.motion is AnimationClip ?
                    child.motion :
                    (Object)GenerateTransition(blendTree, child.motion);

                if ((speeds[i] = child.timeScale) != 1)
                    hasCustomSpeeds = true;
            }

            if (hasCustomSpeeds)
                transition.Speeds = speeds;

            return children;
        }

        /************************************************************************************************************************/

        /// <summary>Saves the `transition` in the same folder as the `originalAsset`.</summary>
        private static void SaveTransition(Object originalAsset, Object transition)
        {
            if (string.IsNullOrEmpty(transition.name))
                transition.name = originalAsset.name;

            var path = AssetDatabase.GetAssetPath(originalAsset);
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, transition.name + ".asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(transition, path);

            Debug.Log($"Saved {path}", transition);
        }

        /************************************************************************************************************************/
    }
}
#endif
