using CustomInspector.Processors;
using CustomInspector;
using UnityEngine;

[assembly: RegisterCustomPropertyDisableProcessor(typeof(DisableInPlayModeProcessor))]

namespace CustomInspector.Processors
{
    public class DisableInPlayModeProcessor : CustomPropertyDisableProcessor<DisableInPlayModeAttribute>
    {
        public override bool IsDisabled(CustomProperty property)
        {
            return Application.isPlaying != Attribute.Inverse;
        }
    }
}