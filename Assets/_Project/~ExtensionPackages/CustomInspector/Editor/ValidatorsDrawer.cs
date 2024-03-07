using System;
using System.Collections.Generic;
using CustomInspector.Elements;
using UnityEditor;

namespace CustomInspector
{
    internal class ValidatorsDrawer : CustomDrawer
    {
        public override CustomElement CreateElementInternal(CustomProperty property, CustomElement next)
        {
            if (!property.HasValidators)
            {
                return next;
            }

            var element = new CustomElement();
            element.AddChild(new CustomPropertyValidationResultElement(property));
            element.AddChild(next);
            return element;
        }

        public class CustomPropertyValidationResultElement : CustomElement
        {
            private readonly CustomProperty _property;
            private IReadOnlyList<CustomValidationResult> _validationResults;

            public CustomPropertyValidationResultElement(CustomProperty property)
            {
                _property = property;
            }

            public override float GetHeight(float width)
            {
                if (ChildrenCount == 0)
                {
                    return -EditorGUIUtility.standardVerticalSpacing;
                }

                return base.GetHeight(width);
            }

            public override bool Update()
            {
                var dirty = base.Update();

                dirty |= GenerateValidationResults();

                return dirty;
            }

            private bool GenerateValidationResults()
            {
                if (ReferenceEquals(_property.ValidationResults, _validationResults))
                {
                    return false;
                }

                _validationResults = _property.ValidationResults;

                RemoveAllChildren();

                foreach (var result in _validationResults)
                {
                    var infoBox = result.FixAction != null
                        ? new CustomInfoBoxElement(result.Message, result.MessageType,
                            inlineAction: () => ExecuteFix(result.FixAction),
                            inlineActionContent: result.FixActionContent)
                        : new CustomInfoBoxElement(result.Message, result.MessageType);

                    AddChild(infoBox);
                }

                return true;
            }

            private void ExecuteFix(Action fixAction)
            {
                _property.ModifyAndRecordForUndo(targetIndex => fixAction?.Invoke());
            }
        }
    }
}