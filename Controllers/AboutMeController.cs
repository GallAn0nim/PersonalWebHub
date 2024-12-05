using Microsoft.AspNetCore.Mvc;
using PersonalWebHub.Models.AboutMe;
using PersonalWebHub.Services.AboutMe;

namespace PersonalWebHub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AboutMeController(IAboutMeService aboutMeService) : ControllerBase
{
    [HttpGet("socialMedia")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SocialMediaResponse>))]
    public async Task<IActionResult> SocialMedia([FromQuery] bool whySoSerious = false)
    {
        try
        {
            var response = await aboutMeService.GetSocialMedias(whySoSerious);
            return Ok(response);
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