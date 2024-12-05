using Microsoft.AspNetCore.Mvc;
using PersonalWebHub.Models.AdventOfCode2024;
using PersonalWebHub.Services.AdventOfCode2024;

namespace PersonalWebHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdventOfCode2024Controller(IAdventOfCode2024Service adventOfCode2024Service) : ControllerBase
{
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Info()
    {
        return Ok(adventOfCode2024Service.GetGeneralInfo());
    }
    
    [HttpGet("day/{dayNumber:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult DayTaskDescription(int dayNumber)
    {
        return Ok(adventOfCode2024Service.GetTaskDescription(dayNumber));
    }
    
    [HttpGet("day/{dayNumber:int}/partTwo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult DayTaskDescriptionPartTwo(int dayNumber)
    {
        return Ok(adventOfCode2024Service.GetTaskDescription(dayNumber,true));
    }
    
    [HttpGet("day/{dayNumber:int}/params")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult DayTaskParams(int dayNumber)
    {
        return Ok(adventOfCode2024Service.GetTaskParams(dayNumber));
    }
    
    [HttpGet("day/{dayNumber:int}/solution")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult DayTaskSolution(int dayNumber)
    {
        return Ok(adventOfCode2024Service.GetTaskSolution(dayNumber));
    }
    
    [HttpGet("day/{dayNumber:int}/solution/partTwo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdventOfCode2024SolutionResponse))]
    public IActionResult DayTaskSolutionPartTwo(int dayNumber)
    {
        return Ok(adventOfCode2024Service.GetTaskSolution(dayNumber,true));
    }
}