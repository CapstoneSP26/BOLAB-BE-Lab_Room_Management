using FluentValidation;

namespace BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms
{
    public class ConfirmLabRoomImportCommandValidator : AbstractValidator<ConfirmLabRoomImportCommand>
    {
        public ConfirmLabRoomImportCommandValidator()
        {
            RuleFor(x => x.ValidLabRooms)
                .NotNull()
                .WithMessage("Danh sách phòng không được để trống.")
                .Must(x => x.Count > 0)
                .WithMessage("Danh sách phòng phải có ít nhất 1 dòng.");
        }
    }
}
