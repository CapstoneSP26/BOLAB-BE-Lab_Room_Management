using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Dashboard.Queries.GetPdfStatistic
{
    public class GetPdfStatisticQuery : IRequest<byte[]>
    {
        public string TimeType { get; set; }
    }
}
