using FluentValidation;

namespace BookLAB.Application.Features.Users.Commands.ValidateImportUsers
{
    public class ValidateUserImportQueryValidator : AbstractValidator<ValidateUserImportQuery>
    {
        public ValidateUserImportQueryValidator()
        {
            RuleFor(x => x.Users)
                .NotNull()
                .WithMessage("Danh sách user không được để trống.")
                .Must(x => x.Count > 0)
                .WithMessage("Danh sách user phải có ít nhất 1 dòng.");
        }
    }
}
