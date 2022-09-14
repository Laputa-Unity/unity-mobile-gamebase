// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerTransition_2
    [Serializable]
    public abstract class MixerTransition<TMixer, TParameter> : ManualMixerTransition<TMixer>
        where TMixer : MixerState<TParameter>
    {
        /************************************************************************************************************************/

        [SerializeField, HideInInspector]
        private TParameter[] _Thresholds;

        /// <summary>[<see cref="SerializeField"/>]
        /// The parameter values at which each of the states are used and blended.
        /// </summary>
        public ref TParameter[] Thresholds => ref _Thresholds;

        /// <summary>The name of the serialized backing field of <see cref="Thresholds"/>.</summary>
        public const string ThresholdsField = nameof(_Thresholds);

        /************************************************************************************************************************/

        [SerializeField]
        private TParameter _DefaultParameter;

        /// <summary>[<see cref="SerializeField"/>]
        /// The initial parameter value to give the mixer when it is first created.
        /// </summary>
        public ref TParameter DefaultParameter => ref _DefaultParameter;

        /// <summary>The name of the serialized backing field of <see cref="DefaultParameter"/>.</summary>
        public const string DefaultParameterField = nameof(_DefaultParameter);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void InitializeState()
        {
            base.InitializeState();

            State.SetThresholds(_Thresholds);
            State.Parameter = _DefaultParameter;
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

#if UNITY_EDITOR
    /// <summary>[Editor-Only] Draws the Inspector GUI for a <see cref="Transition{TMixer, TParameter}"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">Transitions</see>
    /// and <see href="https://kybernetik.com.au/animancer/docs/manual/blending/mixers">Mixers</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerTransitionDrawer
    /// 
    public class MixerTransitionDrawer : ManualMixerTransition.Drawer
    {
        /************************************************************************************************************************/

        /// <summary>The number of horizontal pixels the "Threshold" label occupies.</summary>
        private readonly float ThresholdWidth;

        /************************************************************************************************************************/

        private static float _StandardThresholdWidth;

        /// <summary>
        /// The number of horizontal pixels the word "Threshold" occupies when drawn with the
        /// <see cref="EditorStyles.popup"/> style.
        /// </summary>
        protected static float StandardThresholdWidth
        {
            get
            {
                if (_StandardThresholdWidth == 0)
                    _StandardThresholdWidth = Editor.AnimancerGUI.CalculateWidth(EditorStyles.popup, "Threshold");
                return _StandardThresholdWidth;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="MixerTransitionDrawer"/> using the default <see cref="StandardThresholdWidth"/>.
        /// </summary>
        public MixerTransitionDrawer() : this(StandardThresholdWidth) { }

        /// <summary>
        /// Creates a new <see cref="MixerTransitionDrawer"/> using a custom width for its threshold labels.
        /// </summary>
        protected MixerTransitionDrawer(float thresholdWidth) => ThresholdWidth = thresholdWidth;

        /************************************************************************************************************************/

        /// <summary>
        /// The serialized <see cref="MixerTransition{TMixer, TParameter}.Thresholds"/> of the
        /// <see cref="ManualMixerTransition.Drawer.CurrentProperty"/>.
        /// </summary>
        protected static SerializedProperty CurrentThresholds { get; private set; }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void GatherSubProperties(SerializedProperty property)
        {
            base.GatherSubProperties(property);

            if (CurrentStates == null)
                return;

            CurrentThresholds = property.FindPropertyRelative(MixerTransition2D.ThresholdsField);

            if (CurrentThresholds == null)
                return;

            var count = Math.Max(CurrentStates.arraySize, CurrentThresholds.arraySize);
            CurrentStates.arraySize = count;
            CurrentThresholds.arraySize = count;
            if (CurrentSpeeds != null &&
                CurrentSpeeds.arraySize != 0)
                CurrentSpeeds.arraySize = count;
        }

        /************************************************************************************************************************/

        /// <summary>Splits the specified `area` into separate sections.</summary>
        protected void SplitListRect(Rect area, bool isHeader, out Rect animation, out Rect threshold, out Rect speed, out Rect sync)
        {
            SplitListRect(area, isHeader, out animation, out speed, out sync);

            threshold = animation;

            var xMin = threshold.xMin = EditorGUIUtility.labelWidth + Editor.AnimancerGUI.IndentSize;

            animation.xMax = xMin - Editor.AnimancerGUI.StandardSpacing;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoStateListHeaderGUI(Rect area)
        {
            SplitListRect(area, true, out var animationArea, out var thresholdArea, out var speedArea, out var syncArea);

            DoAnimationHeaderGUI(animationArea);

            using (ObjectPool.Disposable.AcquireContent(out var label, "Threshold",
                "The parameter values at which each child state will be fully active"))
                DoHeaderDropdownGUI(thresholdArea, CurrentThresholds, label, AddThresholdFunctionsToMenu);

            DoSpeedHeaderGUI(speedArea);

            DoSyncHeaderGUI(syncArea);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoElementGUI(Rect area, int index,
            SerializedProperty clip, SerializedProperty speed)
        {
            SplitListRect(area, false, out var animationArea, out var thresholdArea, out var speedArea, out var syncArea);

            DoElementGUI(animationArea, speedArea, syncArea, index, clip, speed);

            DoThresholdGUI(thresholdArea, index);
        }

        /************************************************************************************************************************/

        /// <summary>Draws the GUI of the threshold at the specified `index`.</summary>
        protected virtual void DoThresholdGUI(Rect area, int index)
        {
            var threshold = CurrentThresholds.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(area, threshold, GUIContent.none);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void OnAddElement(ReorderableList list)
        {
            var index = CurrentStates.arraySize;
            base.OnAddElement(list);
            CurrentThresholds.InsertArrayElementAtIndex(index);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void OnRemoveElement(ReorderableList list)
        {
            base.OnRemoveElement(list);
            Editor.Serialization.RemoveArrayElement(CurrentThresholds, list.index);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void OnReorderList(ReorderableList list, int oldIndex, int newIndex)
        {
            base.OnReorderList(list, oldIndex, newIndex);
            CurrentThresholds.MoveArrayElement(oldIndex, newIndex);
        }

        /************************************************************************************************************************/

        /// <summary>Adds functions to the `menu` relating to the thresholds.</summary>
        protected virtual void AddThresholdFunctionsToMenu(GenericMenu menu) { }

        /************************************************************************************************************************/
    }
#endif
}
