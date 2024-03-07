using CustomInspector.Processors;
using CustomInspector;
using UnityEngine;

[assembly: RegisterCustomPropertyHideProcessor(typeof(HideInEditModeProcessor))]

namespace CustomInspector.Processors
{
    public class HideInEditModeProcessor : CustomPropertyHideProcessor<HideInEditModeAttribute>
    {
        public override bool IsHidden(CustomProperty property)
        {
            return Application.isPlaying == Attribute.Inverse;
        }
    }
}