// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>[Pro-Only] A <see cref="ControllerState"/> which manages one float parameter.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/animator-controllers">Animator Controllers</see>
    /// </remarks>
    /// <seealso cref="Float2ControllerState"/>
    /// <seealso cref="Float3ControllerState"/>
    /// https://kybernetik.com.au/animancer/api/Animancer/Float1ControllerState
    /// 
    public sealed class Float1ControllerState : ControllerState
    {
        /************************************************************************************************************************/

        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="Float1ControllerState"/>.</summary>
        public new interface ITransition : ITransition<Float1ControllerState> { }

        /************************************************************************************************************************/

        private ParameterID _ParameterID;

        /// <summary>The identifier of the parameter which <see cref="Parameter"/> will get and set.</summary>
        public new ParameterID ParameterID
        {
            get => _ParameterID;
            set
            {
                _ParameterID = value;
                _ParameterID.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// Gets and sets a float parameter in the <see cref="ControllerState.Controller"/> using the
        /// <see cref="ParameterID"/>.
        /// </summary>
        public float Parameter
        {
            get => Playable.GetFloat(_ParameterID.Hash);
            set => Playable.SetFloat(_ParameterID.Hash, value);
        }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="Float1ControllerState"/> to play the `controller`.</summary>
        public Float1ControllerState(RuntimeAnimatorController controller, ParameterID parameter,
            bool keepStateOnStop = false)
            : base(controller, keepStateOnStop)
        {
            _ParameterID = parameter;
            _ParameterID.ValidateHasParameter(controller, AnimatorControllerParameterType.Float);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ParameterCount => 1;

        /// <inheritdoc/>
        public override int GetParameterHash(int index) => _ParameterID;

        /************************************************************************************************************************/
    }
}

