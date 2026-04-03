using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Commands.ImportSchedule;
using BookLAB.Application.Features.Schedules.Commands.ValidateImport;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Application.Features.Schedules.Queries.GetSchedules;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BookLAB.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(IMediator mediator, ILogger<SchedulesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Step 1: Validates the uploaded Excel file and returns a preview with potential errors
    /// </summary>
    /// <param name="file">The Excel file containing schedule data</param>
    /// <returns>A list of validated rows with status (Valid/Invalid) and error messages</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ImportValidationResult<ScheduleImportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateSchedules([FromBody] ValidateImportQuery query)
    {
        // MediatR dispatches to ValidateImportHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost("validate-file")]
    [ProducesResponseType(typeof(ImportValidationResult<ScheduleImportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateSchedulesFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File import không hợp lệ.");
        }

        var schedules = ParseSchedulesFromExcel(file);
        var result = await _mediator.Send(new ValidateImportQuery { Schedules = schedules });

        return Ok(result);
    }

    /// <summary>
    /// Step 2: Officially imports the validated data into the database
    /// </summary>
    /// <param name="command">The list of confirmed schedule items to be saved</param>
    /// <returns>A summary of the import operation (Total success/failure)</returns>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmImport([FromBody] ConfirmImportCommand command)
    {
        // MediatR dispatches to ConfirmImportHandler (using AddRangeAsync logic)
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest(result);
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

        var schedules = ParseSchedulesFromExcel(file);
        var validation = await _mediator.Send(new ValidateImportQuery { Schedules = schedules });

        if (!validation.CanCommit)
        {
            return BadRequest(validation);
        }

        var validSchedules = validation.Rows
            .Where(r => !r.IsCritical)
            .Select(r => r.Data)
            .ToList();

        var imported = await _mediator.Send(new ConfirmImportCommand { ValidSchedules = validSchedules });

        return Ok(new
        {
            Imported = imported,
            TotalRows = validation.Rows.Count,
            ValidRows = validSchedules.Count
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules([FromQuery] GetSchedulesQuery query)
    {
        // MediatR sẽ chuyển hướng query này đến GetSchedulesQueryHandler
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("schedule-attendance")]
    public async Task<IActionResult> GetScheduleInAttendance([FromQuery] GetSchedulesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user identity from claims
            if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
                return Unauthorized();


            GetSchedulesQuery command = new GetSchedulesQuery
            {
                SearchTerm = query.SearchTerm,
                Status = query.Status,
                LabRoomId = query.LabRoomId,
                FromDate = query.FromDate,
                ToDate = query.ToDate,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                IsDescending = query.IsDescending,
            };

            // Send the command through MediatR pipeline
            var result = await _mediator.Send(command, cancellationToken);

            // Return success response with the retrieved data
            return Ok(new
            {
                result = result
            });
        }
        catch (Exception ex)
        {
            // Log the error with details for debugging
            _logger.LogError(ex, "Something is wrong while getting unchecked booking requests: " + ex.Message);

            // Return internal server error response
            return Problem("Something is wrong while getting unchecked booking requests");
        }
    }

    private static List<ScheduleImportDto> ParseSchedulesFromExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault();

        if (worksheet == null || worksheet.LastRowUsed() == null)
        {
            return new List<ScheduleImportDto>();
        }

        var schedules = new List<ScheduleImportDto>();
        var lastRow = worksheet.LastRowUsed()!.RowNumber();

        var headerIndexes = BuildHeaderIndexMap(worksheet.Row(1));

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);

            var groupName = GetCellValue(row, headerIndexes, "groupname", 1);
            var subjectCode = GetCellValue(row, headerIndexes, "subjectcode", 2);
            var date = GetCellValue(row, headerIndexes, "date", 3);
            date = NormalizeExcelDate(date);
            var slotOrderRaw = GetCellValue(row, headerIndexes, "slotorder", 4);
            var slotTypeCode = GetCellValue(row, headerIndexes, "slottypecode", 5);
            var roomNo = GetCellValue(row, headerIndexes, "roomno", 6);
            var lecturer = GetCellValue(row, headerIndexes, "lecturer", 7);

            if (string.IsNullOrWhiteSpace(groupName) &&
                string.IsNullOrWhiteSpace(subjectCode) &&
                string.IsNullOrWhiteSpace(date) &&
                string.IsNullOrWhiteSpace(slotOrderRaw) &&
                string.IsNullOrWhiteSpace(slotTypeCode) &&
                string.IsNullOrWhiteSpace(roomNo) &&
                string.IsNullOrWhiteSpace(lecturer))
            {
                continue;
            }

            _ = int.TryParse(slotOrderRaw, out var slotOrder);

            schedules.Add(new ScheduleImportDto
            {
                GroupName = groupName,
                SubjectCode = subjectCode,
                Date = date,
                SlotOrder = slotOrder,
                SlotTypeCode = slotTypeCode,
                RoomNo = roomNo,
                Lecturer = lecturer
            });
        }

        return schedules;
    }

    private static Dictionary<string, int> BuildHeaderIndexMap(IXLRow headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var lastCell = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (var i = 1; i <= lastCell; i++)
        {
            var key = NormalizeHeader(headerRow.Cell(i).GetString());
            if (!string.IsNullOrWhiteSpace(key) && !map.ContainsKey(key))
            {
                map[key] = i;
            }
        }

        return map;
    }

    private static string GetCellValue(IXLRow row, Dictionary<string, int> headerIndexes, string expectedHeader, int fallbackIndex)
    {
        if (headerIndexes.TryGetValue(expectedHeader, out var index))
        {
            return row.Cell(index).GetString().Trim();
        }

        return row.Cell(fallbackIndex).GetString().Trim();
    }

    private static string NormalizeHeader(string raw)
    {
        return new string((raw ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }

    private static string NormalizeExcelDate(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var trimmed = raw.Trim();

        if (double.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out var oaDate) ||
            double.TryParse(trimmed, NumberStyles.Any, CultureInfo.CurrentCulture, out oaDate))
        {
            try
            {
                var date = DateTime.FromOADate(oaDate);
                return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return trimmed;
            }
        }

        if (DateTime.TryParse(trimmed, out var parsedDate))
        {
            return parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        return trimmed;
    }
}