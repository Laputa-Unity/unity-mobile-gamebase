using CustomInspector.Processors;
using CustomInspector;
using UnityEngine;

[assembly: RegisterCustomPropertyDisableProcessor(typeof(DisableInEditModeProcessor))]

namespace CustomInspector.Processors
{
    public class DisableInEditModeProcessor : CustomPropertyDisableProcessor<DisableInEditModeAttribute>
    {
        public override bool IsDisabled(CustomProperty property)
        {
            return Application.isPlaying == Attribute.Inverse;
        }
    }
}