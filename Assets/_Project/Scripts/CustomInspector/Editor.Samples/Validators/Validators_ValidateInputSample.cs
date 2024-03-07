using CustomInspector;
using UnityEngine;

public class Validators_ValidateInputSample : ScriptableObject
{
    [ValidateInput(nameof(ValidateTexture))]
    public Texture tex;

    [ValidateInput(nameof(ValidateNumber))]
    public int number;

    private CustomValidationResult ValidateTexture()
    {
        if (tex == null) return CustomValidationResult.Error("Tex is null");
        if (!tex.isReadable) return CustomValidationResult.Warning("Tex must be readable");
        return CustomValidationResult.Valid;
    }

    private CustomValidationResult ValidateNumber()
    {
        if (number == 1)
        {
            return CustomValidationResult.Valid;
        }

        return CustomValidationResult.Error("Number must be equal 1")
            .WithFix(() => number = 1, "Set to 1");
    }
}