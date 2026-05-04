using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.CreateSlotType
{
    public class CreateSlotTypeCommand : IRequest<ResultMessage<SlotTypeDto>>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SlotFrameTemp> SlotFrames { get; set; }
    }

    public class SlotFrameTemp
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int OrderIndex { get; set; }
    }
}
