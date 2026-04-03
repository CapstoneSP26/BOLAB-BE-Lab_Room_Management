using FluentValidation;

namespace BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms
{
    public class ValidateLabRoomImportQueryValidator : AbstractValidator<ValidateLabRoomImportQuery>
    {
        public ValidateLabRoomImportQueryValidator()
        {
            RuleFor(x => x.LabRooms)
                .NotNull()
                .WithMessage("Danh sách phòng không được để trống.")
                .Must(x => x.Count > 0)
                .WithMessage("Danh sách phòng phải có ít nhất 1 dòng.");
        }
    }
}
