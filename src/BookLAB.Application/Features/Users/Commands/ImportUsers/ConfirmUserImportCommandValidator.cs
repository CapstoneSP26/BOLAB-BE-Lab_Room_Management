using FluentValidation;

namespace BookLAB.Application.Features.Users.Commands.ImportUsers
{
    public class ConfirmUserImportCommandValidator : AbstractValidator<ConfirmUserImportCommand>
    {
        public ConfirmUserImportCommandValidator()
        {
            RuleFor(x => x.ValidUsers)
                .NotNull()
                .WithMessage("Danh sách user không được để trống.")
                .Must(x => x.Count > 0)
                .WithMessage("Danh sách user phải có ít nhất 1 dòng.");
        }
    }
}
