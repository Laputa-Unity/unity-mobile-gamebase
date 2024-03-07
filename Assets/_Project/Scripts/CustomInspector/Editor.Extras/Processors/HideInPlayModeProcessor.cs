using CustomInspector.Processors;
using CustomInspector;
using UnityEngine;

[assembly: RegisterCustomPropertyHideProcessor(typeof(HideInPlayModeProcessor))]

namespace CustomInspector.Processors
{
    public class HideInPlayModeProcessor : CustomPropertyHideProcessor<HideInPlayModeAttribute>
    {
        public override bool IsHidden(CustomProperty property)
        {
            return Application.isPlaying != Attribute.Inverse;
        }
    }
}