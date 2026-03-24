using BookLAB.Application.Common.Specifications;
using BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.SlotTypes.GetSlotTypes
{
    public class SlotTypeSpecification : BaseSpecification<SlotType>
    {
        public SlotTypeSpecification(GetSlotTypesQuery query) 
        {
            if (query.CampusId.HasValue)
            {
                AddCriteria(s => s.CampusId == query.CampusId.Value);
            }

            AddInclude(s => s.SlotFrames);
            ApplyOrderBy(s => s.Code);
        }
    }
}
