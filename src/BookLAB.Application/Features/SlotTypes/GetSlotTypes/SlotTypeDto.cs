namespace BookLAB.Application.Features.SlotTypes.GetSlotTypes
{
    public record SlotTypeDto
    {
        public int Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public List<SlotFrameDto> SlotFrames { get; init; } = new();
    }

    public record SlotFrameDto
    {
        public int Id { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
        public int OrderIndex { get; init; }
    }
}
