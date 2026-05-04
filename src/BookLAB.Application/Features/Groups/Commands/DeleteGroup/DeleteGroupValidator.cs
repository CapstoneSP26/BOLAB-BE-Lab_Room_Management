using FluentValidation;

namespace BookLAB.Application.Features.Groups.Commands.DeleteGroup
{
    public class DeleteGroupValidator : AbstractValidator<DeleteGroupCommand>
    {
        public DeleteGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("ID nhóm không được bỏ trống");
        }
    }
}
