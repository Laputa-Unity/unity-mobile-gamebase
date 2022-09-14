// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animancer.FSM
{
    /// <summary>A simple keyless Finite State Machine system.</summary>
    /// <remarks>
    /// This class doesn't keep track of any states other than the currently active one.
    /// See <see cref="StateMachine{TKey, TState}"/> for a system that allows states to be pre-registered and accessed
    /// using a separate key.
    /// <para></para>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/fsm">Finite State Machines</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/StateMachine_1
    /// 
    [HelpURL(StateExtensions.APIDocumentationURL + nameof(StateMachine<TState>) + "_1")]
    public partial class StateMachine<TState> where TState : class, IState
    {
        /************************************************************************************************************************/

        /// <summary>The currently active state.</summary>
        public TState CurrentState { get; private set; }

        /************************************************************************************************************************/

        /// <summary>The <see cref="StateChange{TState}.PreviousState"/>.</summary>
        public TState PreviousState => StateChange<TState>.PreviousState;

        /// <summary>The <see cref="StateChange{TState}.NextState"/>.</summary>
        public TState NextState => StateChange<TState>.NextState;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="StateMachine{TState}"/>, leaving the <see cref="CurrentState"/> null.</summary>
        public StateMachine() { }

        /// <summary>Creates a new <see cref="StateMachine{TState}"/> and immediately enters the `state`.</summary>
        /// <remarks>This calls <see cref="IState.OnEnterState"/> but not <see cref="IState.CanEnterState"/>.</remarks>
        public StateMachine(TState state)
        {
#if UNITY_ASSERTIONS
            if (state == null)// AllowNullStates won't be true yet since this is the constructor.
                throw new ArgumentNullException(nameof(state), NullNotAllowed);
#endif

            using (new StateChange<TState>(this, null, state))
            {
                CurrentState = state;
                state.OnEnterState();
            }
        }

        /************************************************************************************************************************/

        /// <summary>Is it currently possible to enter the specified `state`?</summary>
        /// <remarks>
        /// This requires <see cref="IState.CanExitState"/> on the <see cref="CurrentState"/> and
        /// <see cref="IState.CanEnterState"/> on the specified `state` to both return true.
        /// </remarks>
        public bool CanSetState(TState state)
        {
#if UNITY_ASSERTIONS
            if (state == null && !AllowNullStates)
                throw new ArgumentNullException(nameof(state), NullNotAllowed);
#endif

            using (new StateChange<TState>(this, CurrentState, state))
            {
                if (CurrentState != null && !CurrentState.CanExitState)
                    return false;

                if (state != null && !state.CanEnterState)
                    return false;

                return true;
            }
        }

        /// <summary>Returns the first of the `states` which can currently be entered.</summary>
        /// <remarks>
        /// This requires <see cref="IState.CanExitState"/> on the <see cref="CurrentState"/> and
        /// <see cref="IState.CanEnterState"/> on one of the `states` to both return true.
        /// <para></para>
        /// States are checked in ascending order (i.e. from <c>[0]</c> to <c>[states.Count - 1]</c>).
        /// </remarks>
        public TState CanSetState(IList<TState> states)
        {
            // We call CanSetState so that it will check CanExitState for each individual pair in case it does
            // something based on the next state.

            var count = states.Count;
            for (int i = 0; i < count; i++)
            {
                var state = states[i];
                if (CanSetState(state))
                    return state;
            }

            return null;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method returns true immediately if the specified `state` is already the <see cref="CurrentState"/>.
        /// To allow directly re-entering the same state, use <see cref="TryResetState(TState)"/> instead.
        /// </summary>
        public bool TrySetState(TState state)
        {
            if (CurrentState == state)
            {
#if UNITY_ASSERTIONS
                if (state == null && !AllowNullStates)
                    throw new ArgumentNullException(nameof(state), NullNotAllowed);
#endif

                return true;
            }

            return TryResetState(state);
        }

        /// <summary>
        /// Attempts to enter any of the specified `states` and returns true if successful.
        /// <para></para>
        /// This method returns true and does nothing else if the <see cref="CurrentState"/> is in the list.
        /// To allow directly re-entering the same state, use <see cref="TryResetState(IList{TState})"/> instead.
        /// </summary>
        /// <remarks>States are checked in ascending order (i.e. from <c>[0]</c> to <c>[states.Count - 1]</c>).</remarks>
        public bool TrySetState(IList<TState> states)
        {
            var count = states.Count;
            for (int i = 0; i < count; i++)
                if (TrySetState(states[i]))
                    return true;

            return false;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method does not check if the `state` is already the <see cref="CurrentState"/>. To do so, use
        /// <see cref="TrySetState(TState)"/> instead.
        /// </summary>
        public bool TryResetState(TState state)
        {
            if (!CanSetState(state))
                return false;

            ForceSetState(state);
            return true;
        }

        /// <summary>
        /// Attempts to enter any of the specified `states` and returns true if successful.
        /// <para></para>
        /// This method does not check if the `state` is already the <see cref="CurrentState"/>. To do so, use
        /// <see cref="TrySetState(IList{TState})"/> instead.
        /// </summary>
        /// <remarks>States are checked in ascending order (i.e. from <c>[0]</c> to <c>[states.Count - 1]</c>).</remarks>
        public bool TryResetState(IList<TState> states)
        {
            var count = states.Count;
            for (int i = 0; i < count; i++)
                if (TryResetState(states[i]))
                    return true;

            return false;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls <see cref="IState.OnExitState"/> on the <see cref="CurrentState"/> then changes to the
        /// specified `state` and calls <see cref="IState.OnEnterState"/> on it.
        /// <para></para>
        /// This method does not check <see cref="IState.CanExitState"/> or
        /// <see cref="IState.CanEnterState"/>. To do that, you should use <see cref="TrySetState"/> instead.
        /// </summary>
        public void ForceSetState(TState state)
        {
#if UNITY_ASSERTIONS
            if (state == null)
            {
                if (!AllowNullStates)
                    throw new ArgumentNullException(nameof(state), NullNotAllowed);
            }
            else if (state is IOwnedState<TState> owned && owned.OwnerStateMachine != this)
            {
                throw new InvalidOperationException(
                    $"Attempted to use a state in a machine that is not its owner." +
                    $"\n    State: {state}" +
                    $"\n    Machine: {this}");
            }
#endif

            using (new StateChange<TState>(this, CurrentState, state))
            {
                CurrentState?.OnExitState();

                CurrentState = state;

                state?.OnEnterState();
            }
        }

        /************************************************************************************************************************/

        /// <summary>Returns a string describing the type of this state machine and its <see cref="CurrentState"/>.</summary>
        public override string ToString() => $"{GetType().Name} -> {CurrentState}";

        /************************************************************************************************************************/

#if UNITY_ASSERTIONS
        /// <summary>[Assert-Only] Should the <see cref="CurrentState"/> be allowed to be set to null? Default is false.</summary>
        /// <remarks>Can be set by <see cref="SetAllowNullStates"/>.</remarks>
        public bool AllowNullStates { get; private set; }

        /// <summary>[Assert-Only] The error given when attempting to set the <see cref="CurrentState"/> to null.</summary>
        private const string NullNotAllowed =
            "This " + nameof(StateMachine<TState>) + " does not allow its state to be set to null." +
            " Use " + nameof(SetAllowNullStates) + " to allow it if this is intentional.";
#endif

        /// <summary>[Assert-Conditional] Sets <see cref="AllowNullStates"/>.</summary>
        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        public void SetAllowNullStates(bool allow = true)
        {
#if UNITY_ASSERTIONS
            AllowNullStates = allow;
#endif
        }

        /************************************************************************************************************************/
    }
}
