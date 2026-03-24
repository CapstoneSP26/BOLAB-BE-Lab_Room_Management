using FluentValidation;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingByName
{
    public class GetBuildingByNameQueryValidator : AbstractValidator<GetBuildingByNameQuery>
    {
        public GetBuildingByNameQueryValidator()
        {
            RuleFor(x => x.BuildingName)
                .NotEmpty().WithMessage("Building name is required")
                .NotNull().WithMessage("Building name cannot be null")
                .MinimumLength(1).WithMessage("Building name must have at least 1 character");
        }
    }
}
