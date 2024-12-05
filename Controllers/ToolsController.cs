using Microsoft.AspNetCore.Mvc;
using PersonalWebHub.Models.Tools;
using PersonalWebHub.Services.Tools;

namespace PersonalWebHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToolsController(IObfuscateService obfuscateService) : ControllerBase
{
    [HttpGet("ObfuscateText")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult ObfuscateText([FromQuery] ObfuscateTextRequest request)
    {
        try
        {
           var result = obfuscateService.Obfuscate(request);
            
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("DeobfuscateText")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult DeobfuscateText([FromQuery] DeobfuscateTextRequest request)
    {
        try
        {
            var result = obfuscateService.Deobfuscate(request);
            
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}