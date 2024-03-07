using System;
using Sirenix.OdinInspector.Editor.Validation;

namespace CustomInspector.Editor.Integrations.Odin
{
    internal static class CustomOdinExtensions
    {
        public static void CopyValidationResultsTo(this CustomPropertyTree tree, ValidationResult result)
        {
            tree.EnumerateValidationResults(result.AddIfError);
            tree.EnumerateValidationResults(result.AddIfWarning);
        }

        private static void AddIfError(this ValidationResult result,
            CustomProperty property, CustomValidationResult triResult)
        {
            if (triResult.MessageType != CustomMessageType.Error)
            {
                return;
            }

#if ODIN_INSPECTOR_3_1
            result.AddError(triResult.Message);
#else
            if (result.ResultType == ValidationResultType.Error)
            {
                result.Message += Environment.NewLine + triResult.Message;
            }
            else
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = triResult.Message;
            }
#endif
        }

        private static void AddIfWarning(this ValidationResult result,
            CustomProperty property, CustomValidationResult triResult)
        {
            if (triResult.MessageType != CustomMessageType.Warning)
            {
                return;
            }

#if ODIN_INSPECTOR_3_1
            result.AddWarning(triResult.Message);
#else
            if (result.ResultType == ValidationResultType.Error)
            {
                // Do not override errors
            }
            else if (result.ResultType == ValidationResultType.Warning)
            {
                result.Message += Environment.NewLine + triResult.Message;
            }
            else
            {
                result.ResultType = ValidationResultType.Warning;
                result.Message = triResult.Message;
            }
#endif
        }
    }
}