using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.API.Validation;

/// <summary>
/// FPTU student code format: 2 uppercase letters + 5 digits (e.g. SE19886, CE18793).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public partial class FptStudentCodeAttribute : ValidationAttribute
{
    [GeneratedRegex(@"^[A-Z]{2}\d{5}$", RegexOptions.Compiled)]
    private static partial Regex CodePattern();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string code || string.IsNullOrWhiteSpace(code))
            return new ValidationResult("Student code is required.");

        if (!CodePattern().IsMatch(code.Trim().ToUpperInvariant()))
            return new ValidationResult("Student code must follow FPTU format (e.g. SE19886, CE18793).");

        return ValidationResult.Success;
    }
}
