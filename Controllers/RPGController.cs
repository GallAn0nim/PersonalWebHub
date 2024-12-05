using Microsoft.AspNetCore.Mvc;
using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Requests;
using PersonalWebHub.Models.Rpg.Responses;
using PersonalWebHub.Services.Rpg;

namespace PersonalWebHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RpgController(IDiceRollService diceRollService) : ControllerBase
    {
        [HttpGet("dices")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DiceTypes))]
        public IActionResult Dices()
        {
            var diceNames = Enum.GetNames(typeof(DiceTypes)).ToList();

            return Ok(diceNames);
        }

        [HttpGet("randomnessMethods")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RandomnessMethods))]
        public IActionResult RandomnessMethodsList()
        {
            var randomnessMethodsNames = Enum.GetNames(typeof(RandomnessMethods)).ToList();

            return Ok(randomnessMethodsNames);
        }

        [HttpGet("randomnessMethods/{randomnessMethod}/distribution")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RandomnessMethodsDistributionResponse))]
        public IActionResult RandomnessMethodDistribution([FromRoute] RandomnessMethods randomnessMethod,
            [FromQuery] RandomnessMethodsDistributionRequest randomnessMethodsDistributionRequest)
        {
            try
            {
                var response =
                    diceRollService.RandomnessMethodsDistribution(randomnessMethodsDistributionRequest,
                        randomnessMethod);
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

        [HttpGet("dices/roll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DicesRollResponse))]
        public IActionResult DicesRoll([FromQuery] DicesRollRequest request)
        {
            try
            {
                var response = diceRollService.RollDice(request);
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

        [HttpGet("dices/formula/roll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FormulaRollResponse))]
        public IActionResult FormulaRoll([FromQuery] FormulaRollRequest request)
        {
            try
            {
                var response = diceRollService.RollDiceFormula(request);
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
}