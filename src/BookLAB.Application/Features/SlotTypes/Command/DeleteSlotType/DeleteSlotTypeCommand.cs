using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.Command.DeleteSlotType
{
    public class DeleteSlotTypeCommand : IRequest<ResultMessage<bool>>
    {
        public int Id { get; set; }
    }
}
