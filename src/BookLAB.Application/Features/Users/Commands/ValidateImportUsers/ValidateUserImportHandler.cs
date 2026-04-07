using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookLAB.Application.Features.Users.Commands.ValidateImportUsers
{
    public class ValidateUserImportHandler : IRequestHandler<ValidateUserImportQuery, ImportValidationResult<UserImportDto, User>>
    {

        private readonly IUserImportService _userImportService;

        public ValidateUserImportHandler(IUserImportService userImportService)
        {
            _userImportService = userImportService;
        }

        public async Task<ImportValidationResult<UserImportDto, User>> Handle(ValidateUserImportQuery request, CancellationToken cancellationToken)
        {
            var response = await _userImportService.ValidateAsync(request.Users, request.CampusId, cancellationToken);
            return response.result;
        }
    }
}
