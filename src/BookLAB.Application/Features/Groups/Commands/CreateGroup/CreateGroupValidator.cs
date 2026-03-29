using FluentValidation;

namespace BookLAB.Application.Features.Groups.Commands.CreateGroup
{
    public class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty().WithMessage("Tên nhóm không được bỏ trống")
                .MaximumLength(150).WithMessage("Tên nhóm không được vượt quá 150 ký tự")
                .MinimumLength(2).WithMessage("Tên nhóm phải có ít nhất 2 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
