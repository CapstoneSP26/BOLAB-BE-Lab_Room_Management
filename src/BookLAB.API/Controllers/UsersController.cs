using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Commands.ImportUsers;
using BookLAB.Application.Features.Users.Commands.ValidateImportUsers;
using BookLAB.Application.Features.Users.Common;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookLAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("validate-import")]
        [ProducesResponseType(typeof(ImportValidationResult<UserImportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateImport([FromBody] ValidateUserImportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("validate-import-file")]
        [ProducesResponseType(typeof(ImportValidationResult<UserImportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateImportFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File import không hợp lệ.");
            }

            var users = ParseUsersFromExcel(file);
            var result = await _mediator.Send(new ValidateUserImportQuery { Users = users });
            return Ok(result);
        }

        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmImport([FromBody] ConfirmUserImportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("import-file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmImportFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File import không hợp lệ.");
            }

            var users = ParseUsersFromExcel(file);
            var validation = await _mediator.Send(new ValidateUserImportQuery { Users = users });

            if (!validation.CanCommit)
            {
                return BadRequest(validation);
            }

            var validUsers = validation.Rows
                .Where(r => !r.IsCritical)
                .Select(r => r.Data)
                .ToList();

            var imported = await _mediator.Send(new ConfirmUserImportCommand { ValidUsers = validUsers });

            return Ok(new
            {
                Imported = imported,
                TotalRows = validation.Rows.Count,
                ValidRows = validUsers.Count
            });
        }

        [HttpGet("import-template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadImportTemplate()
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Users");

            sheet.Cell(1, 1).Value = "FullName";
            sheet.Cell(1, 2).Value = "Email";
            sheet.Cell(1, 3).Value = "UserCode";
            sheet.Cell(1, 4).Value = "CampusId";
            sheet.Cell(1, 5).Value = "RoleName";

            sheet.Cell(2, 1).Value = "Nguyen Van A";
            sheet.Cell(2, 2).Value = "nguyenvana@example.edu";
            sheet.Cell(2, 3).Value = "NVA001";
            sheet.Cell(2, 4).Value = 1;
            sheet.Cell(2, 5).Value = "Lecturer";

            sheet.Cell(3, 1).Value = "Tran Thi B";
            sheet.Cell(3, 2).Value = "tranthib@example.edu";
            sheet.Cell(3, 3).Value = "TTB001";
            sheet.Cell(3, 4).Value = 1;
            sheet.Cell(3, 5).Value = "Manager";

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "user-import-template.xlsx");
        }

        private static List<UserImportDto> ParseUsersFromExcel(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null || worksheet.LastRowUsed() == null)
            {
                return new List<UserImportDto>();
            }

            var users = new List<UserImportDto>();
            var lastRow = worksheet.LastRowUsed()!.RowNumber();

            for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);

                var fullName = row.Cell(1).GetString().Trim();
                var email = row.Cell(2).GetString().Trim();
                var userCode = row.Cell(3).GetString().Trim();
                var campusRaw = row.Cell(4).GetString().Trim();
                var roleName = row.Cell(5).GetString().Trim();

                if (string.IsNullOrWhiteSpace(fullName) &&
                    string.IsNullOrWhiteSpace(email) &&
                    string.IsNullOrWhiteSpace(userCode) &&
                    string.IsNullOrWhiteSpace(campusRaw) &&
                    string.IsNullOrWhiteSpace(roleName))
                {
                    continue;
                }

                _ = int.TryParse(campusRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var campusId);

                users.Add(new UserImportDto
                {
                    FullName = fullName,
                    Email = email,
                    UserCode = userCode,
                    CampusId = campusId,
                    RoleName = roleName
                });
            }

            return users;
        }
    }
}
