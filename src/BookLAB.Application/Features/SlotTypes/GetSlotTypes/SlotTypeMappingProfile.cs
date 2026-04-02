using AutoMapper;
using BookLAB.Application.Features.SlotTypes.GetSlotTypes;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;

public class SlotTypeMappingProfile : AutoMapper.Profile
{
    public SlotTypeMappingProfile()
    {
        CreateMap<SlotFrame, SlotFrameDto>()
            .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.StartTimeSlot))
            .ForMember(d => d.EndTime, opt => opt.MapFrom(s => s.EndTimeSlot));

        CreateMap<SlotType, SlotTypeDto>()
            .ForMember(d => d.Code, opt => opt.MapFrom(s => s.Code))
            // Sắp xếp các khung giờ theo thứ tự Slot 1, 2, 3...
            .ForMember(d => d.SlotFrames, opt => opt.MapFrom(s => s.SlotFrames.OrderBy(f => f.OrderIndex)));
    }
}