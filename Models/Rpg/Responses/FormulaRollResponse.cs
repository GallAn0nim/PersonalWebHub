using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Models;

namespace PersonalWebHub.Models.Rpg.Responses
{
    public class FormulaRollResponse
    {
        public RandomnessMethods RandomnessMethod { get; set; }
        public int NumberOfRolls { get; set; } = 1;
        public string Formula { get; set; }       
        public string Results { get; set; }
        
        public RollStatistic? RollsStatistic { get; set; } = null;
        public List<FormulaRollSummary> FormulaRolls { get; set; } = new List<FormulaRollSummary>();
    }
}
