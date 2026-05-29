using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampusesController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CampusesController(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("location")]
        public async Task<Location> GetCampusLocation()
        {
            var campus = await _unitOfWork.Repository<Campus>().GetByIdAsync(_currentUserService.CampusId);
            if (campus == null)
            {
                return null;
            }

            return new Location
            {
                Latitude = campus.Latitude,
                Longitude = campus.Longitude
            };
        }
    }
}
