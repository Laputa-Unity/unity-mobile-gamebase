// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;

namespace Animancer.FSM
{
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/StateMachine_1
    partial class StateMachine<TState>
    {
        /// <summary>A <see cref="StateMachine{TState}"/> with a <see cref="DefaultState"/>.</summary>
        /// <remarks>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/fsm/changing-states#default-states">Default States</see>
        /// </remarks>
        /// https://kybernetik.com.au/animancer/api/Animancer.FSM/WithDefault
        /// 
        public class WithDefault : StateMachine<TState>
        {
            /************************************************************************************************************************/

            private TState _DefaultState;

            /// <summary>The starting state and main state to return to when nothing else is active.</summary>
            /// <remarks>
            /// If the <see cref="CurrentState"/> is <c>null</c> when setting this value, it calls
            /// <see cref="ForceSetState(TState)"/> to enter the specified state immediately.
            /// <para></para>
            /// For a character, this would typically be their <em>Idle</em> state.
            /// </remarks>
            public TState DefaultState
            {
                get => _DefaultState;
                set
                {
                    _DefaultState = value;
                    if (CurrentState == null && value != null)
                        ForceSetState(value);
                }
            }

            /************************************************************************************************************************/

            /// <summary>Calls <see cref="ForceSetState(TState)"/> with the <see cref="DefaultState"/>.</summary>
            /// <remarks>This delegate is cached to avoid allocating garbage when used in Animancer Events.</remarks>
            public readonly Action ForceSetDefaultState;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="WithDefault"/>.</summary>
            public WithDefault()
            {
                // Silly C# doesn't allow instance delegates to be assigned using field initializers.
                ForceSetDefaultState = () => ForceSetState(_DefaultState);
            }

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="WithDefault"/> and sets the <see cref="DefaultState"/>.</summary>
            public WithDefault(TState defaultState)
                : this()
            {
                _DefaultState = defaultState;
                ForceSetState(defaultState);
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Attempts to enter the <see cref="DefaultState"/> and returns true if successful.
            /// <para></para>
            /// This method returns true immediately if the specified <see cref="DefaultState"/> is already the
            /// <see cref="CurrentState"/>. To allow directly re-entering the same state, use
            /// <see cref="TryResetState(TState)"/> instead.
            /// </summary>
            public bool TrySetDefaultState() => TrySetState(DefaultState);

            /************************************************************************************************************************/

            /// <summary>
            /// Attempts to enter the <see cref="DefaultState"/> and returns true if successful.
            /// <para></para>
            /// This method does not check if the <see cref="DefaultState"/> is already the <see cref="CurrentState"/>.
            /// To do so, use <see cref="TrySetState(TState)"/> instead.
            /// </summary>
            public bool TryResetDefaultState() => TryResetState(DefaultState);

            /************************************************************************************************************************/
        }
    }
}
