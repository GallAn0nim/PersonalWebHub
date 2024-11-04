using PersonalWebHub.Models.Rpg.Enum;

namespace PersonalWebHub.Models.Rpg.Models
{
    public class RollPlan
    {
        public RandomnessMethods RandomnessMethod { get; set; }
        public DiceRollInput DiceRollInput { get; set; } = new DiceRollInput();

        public List<string> CustomDicePattern { get; set; } = new List<string>();
     
        public bool CalculateGeneralStatistic { get; set; } = true;

        public bool AssignCustomDicePattern { get; set; } = false;

        public bool AssignDiceRange { get; set; } = true;
    }
}
