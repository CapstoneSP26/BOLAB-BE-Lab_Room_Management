using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.LabRooms.Commands.ImportLabRooms;
using BookLAB.Application.Features.LabRooms.Commands.ValidateImportLabRooms;
using BookLAB.Application.Features.LabRooms.Common;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRoomPolicies;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
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
    public class LabRoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LabRoomController> _logger;

        public LabRoomController(IMediator mediator, ILogger<LabRoomController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}/policies")]
        public async Task<IActionResult> GetPolicies(int id)
        {
            var query = new GetLabRoomPoliciesQuery { LabRoomId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedList<LabRoomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLabRooms([FromQuery] GetLabRoomsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("validate-import")]
        [ProducesResponseType(typeof(ImportValidationResult<LabRoomImportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateImport([FromBody] ValidateLabRoomImportQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("validate-import-file")]
        [ProducesResponseType(typeof(ImportValidationResult<LabRoomImportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateImportFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File import không hợp lệ.");
            }

            var labRooms = ParseLabRoomsFromExcel(file);
            var result = await _mediator.Send(new ValidateLabRoomImportQuery { LabRooms = labRooms });
            return Ok(result);
        }

        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmImport([FromBody] ConfirmLabRoomImportCommand command)
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

            var labRooms = ParseLabRoomsFromExcel(file);
            var validation = await _mediator.Send(new ValidateLabRoomImportQuery { LabRooms = labRooms });

            if (!validation.CanCommit)
            {
                return BadRequest(validation);
            }

            var validLabRooms = validation.Rows
                .Where(r => !r.IsCritical)
                .Select(r => r.Data)
                .ToList();

            var imported = await _mediator.Send(new ConfirmLabRoomImportCommand { ValidLabRooms = validLabRooms });

            return Ok(new
            {
                Imported = imported,
                TotalRows = validation.Rows.Count,
                ValidRows = validLabRooms.Count
            });
        }

        [HttpGet("import-template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DownloadImportTemplate()
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("LabRooms");

            sheet.Cell(1, 1).Value = "BuildingId";
            sheet.Cell(1, 2).Value = "RoomName";
            sheet.Cell(1, 3).Value = "RoomNo";
            sheet.Cell(1, 4).Value = "Location";
            sheet.Cell(1, 5).Value = "Capacity";
            sheet.Cell(1, 6).Value = "HasEquipment";
            sheet.Cell(1, 7).Value = "OverrideNumber";
            sheet.Cell(1, 8).Value = "Description";

            sheet.Cell(2, 1).Value = 1;
            sheet.Cell(2, 2).Value = "Lab C1";
            sheet.Cell(2, 3).Value = "C101";
            sheet.Cell(2, 4).Value = "Floor 1";
            sheet.Cell(2, 5).Value = 40;
            sheet.Cell(2, 6).Value = true;
            sheet.Cell(2, 7).Value = 0;
            sheet.Cell(2, 8).Value = "Computer lab";

            sheet.Cell(3, 1).Value = 2;
            sheet.Cell(3, 2).Value = "Lab D1";
            sheet.Cell(3, 3).Value = "D201";
            sheet.Cell(3, 4).Value = "Floor 2";
            sheet.Cell(3, 5).Value = 35;
            sheet.Cell(3, 6).Value = false;
            sheet.Cell(3, 7).Value = 0;
            sheet.Cell(3, 8).Value = "Practice room";

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "labroom-import-template.xlsx");
        }

        private static List<LabRoomImportDto> ParseLabRoomsFromExcel(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null || worksheet.LastRowUsed() == null)
            {
                return new List<LabRoomImportDto>();
            }

            var labRooms = new List<LabRoomImportDto>();
            var lastRow = worksheet.LastRowUsed()!.RowNumber();

            for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);

                var buildingRaw = row.Cell(1).GetString().Trim();
                var roomName = row.Cell(2).GetString().Trim();
                var roomNo = row.Cell(3).GetString().Trim();
                var location = row.Cell(4).GetString().Trim();
                var capacityRaw = row.Cell(5).GetString().Trim();
                var hasEquipmentRaw = row.Cell(6).GetString().Trim();
                var overrideRaw = row.Cell(7).GetString().Trim();
                var description = row.Cell(8).GetString().Trim();

                if (string.IsNullOrWhiteSpace(buildingRaw) &&
                    string.IsNullOrWhiteSpace(roomName) &&
                    string.IsNullOrWhiteSpace(roomNo) &&
                    string.IsNullOrWhiteSpace(location) &&
                    string.IsNullOrWhiteSpace(capacityRaw) &&
                    string.IsNullOrWhiteSpace(hasEquipmentRaw) &&
                    string.IsNullOrWhiteSpace(overrideRaw) &&
                    string.IsNullOrWhiteSpace(description))
                {
                    continue;
                }

                _ = int.TryParse(buildingRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var buildingId);
                _ = int.TryParse(capacityRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var capacity);
                _ = int.TryParse(overrideRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var overrideNumber);

                var hasEquipment = hasEquipmentRaw.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                    hasEquipmentRaw.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    hasEquipmentRaw.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    bool.TryParse(hasEquipmentRaw, out var parsedBool) && parsedBool;

                labRooms.Add(new LabRoomImportDto
                {
                    BuildingId = buildingId,
                    RoomName = roomName,
                    RoomNo = roomNo,
                    Location = string.IsNullOrWhiteSpace(location) ? null : location,
                    Capacity = capacity,
                    HasEquipment = hasEquipment,
                    OverrideNumber = overrideNumber,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description
                });
            }

            return labRooms;
        }

    }

}
