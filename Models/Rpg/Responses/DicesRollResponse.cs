using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Models;

namespace PersonalWebHub.Models.Rpg.Responses
{
    public class DicesRollResponse
    {
        public int NumberOfRolls { get; set; } = 1;

        public DiceTypes Type { get; set; }

        public RandomnessMethods RandomnessMethod { get; set; }
        
        public string Results { get;set; }

        public RollStatistic? RollsStatistic { get; set; } = null;
       
        public List<DiceRoll> DiceRolls { get; set; } = new List<DiceRoll>();
       
    }
}
