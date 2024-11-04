using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Models;

namespace PersonalWebHub.Models.Rpg.Responses
{
    public class RandomnessMethodsDistributionResponse
    {
        public int NumberOfRolls { get; set; }
        public RandomnessMethods RandomnessMethod { get; set; }
        public int Min {  get; set; }
        public int Max { get; set; }

        public double ResultAvarage { get; set; }
        public double PercentageAvarage { get; set; }
        public double PercentageDeviation { get; set; }
        public List<RollOccurrence> DiceRolls { get; set; } = new List<RollOccurrence> { };       
    }
}
