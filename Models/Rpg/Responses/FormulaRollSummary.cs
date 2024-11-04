using PersonalWebHub.Models.Rpg.Models;

namespace PersonalWebHub.Models.Rpg.Responses;

public class FormulaRollSummary
{
    public string Formula { get; set; }
    public string Result { get; set; }
    public string RollResults { get; set; }
    public List<DiceRoll> DiceRolls { get; set; } = new List<DiceRoll>();
}