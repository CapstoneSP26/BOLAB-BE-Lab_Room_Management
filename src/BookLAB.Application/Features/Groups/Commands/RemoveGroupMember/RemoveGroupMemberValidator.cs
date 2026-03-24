using FluentValidation;

namespace BookLAB.Application.Features.Groups.Commands.RemoveGroupMember
{
    public class RemoveGroupMemberValidator : AbstractValidator<RemoveGroupMemberCommand>
    {
        public RemoveGroupMemberValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("ID nhóm không được bỏ trống");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ID sinh viên không được bỏ trống");
        }
    }
}
