using FluentValidation;
using PRN232.LMS.API.Models.Requests;

namespace PRN232.LMS.API.Validators;

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.StudentCode)
            .NotEmpty()
            .Matches(@"^[A-Z]{2}\d{5}$").WithMessage("Student code must follow FPTU format (e.g. SE19886).");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?\d{9,15}$").When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number is not valid.");
    }
}
