using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Application.Common.Interfaces.Services;

namespace BookLAB.Infrastructure.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PolicyService(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork; 
        }

    }
}
