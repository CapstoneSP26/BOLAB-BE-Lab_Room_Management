using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Commands.DeleteLabRoom
{
    public class DeleteLabRoomCommand : IRequest<ResultMessage<bool>>
    {
        public int LabRoomId { get; set; }
    }
}
