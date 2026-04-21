using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.UpdateSlotType
{
    public class UpdateSlotTypeCommand : IRequest<ResultMessage<SlotTypeDto>>
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SlotFrameTemp> SlotFrames { get; set; }
    }

    public class SlotFrameTemp
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int OrderIndex { get; set; }
    }
}
