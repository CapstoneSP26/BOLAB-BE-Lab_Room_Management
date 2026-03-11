using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Managements;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendance.Commands.ScanAttendanceQRCode
{
    public class ScanAttendanceQrCodeHandler : IRequestHandler<ScanAttendanceQrCodeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly QrManagements _qrManagements;
        private readonly IUserRepository _userRepository;
        public ScanAttendanceQrCodeHandler(IUnitOfWork unitOfWork, 
            QrManagements qrManagements,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _qrManagements = qrManagements;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ScanAttendanceQrCodeCommand request, CancellationToken cancellationToken)
        {
            Guid qrId = Guid.Parse(request.qrId);
            Guid scheduelId = Guid.Parse(request.scheduleId);
            Guid studentId = Guid.Parse(request.studentId);

            if (_qrManagements.CheckQrCodeExist(qrId)) return false;

            if (!await _unitOfWork.Repository<Schedule>().Entities.AnyAsync(s => s.Id == scheduelId)) return false;

            if (!await _userRepository.IfExisted(studentId)) return false;

            BookLAB.Domain.Entities.Attendance attendance = new BookLAB.Domain.Entities.Attendance
            {
                
            }
            return true;
        }
    }
}
