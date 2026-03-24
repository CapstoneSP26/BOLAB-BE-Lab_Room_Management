using FluentValidation;

namespace BookLAB.Application.Features.Groups.Commands.UpdateGroupMember
{
    public class UpdateGroupMemberValidator : AbstractValidator<UpdateGroupMemberCommand>
    {
        public UpdateGroupMemberValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("ID nhóm không được bỏ trống");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ID sinh viên không được bỏ trống");

            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("Mã môn học không được bỏ trống")
                .MaximumLength(20).WithMessage("Mã môn học không được vượt quá 20 ký tự");
        }
    }
}
