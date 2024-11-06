using PersonalWebHub.Models.Rpg.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonalWebHub.Models.Rpg.Requests
{
    public class DicesRollRequest
    {
       
        [DefaultValue(DiceTypes.D6)]
        public DiceTypes Type { get; set; } = DiceTypes.D6;

        
        [Range(1, int.MaxValue, ErrorMessage = "Number of rolls must be greater than 0.")]
        [DefaultValue(1)]
        public int NumberOfRolls { get; set; } = 1;

       
        [DefaultValue(RandomnessMethods.CSharpRandom)]
        public RandomnessMethods RandomnessMethod { get; set; } = RandomnessMethods.CSharpRandom;

       
        [DefaultValue(null)]
        public int? Min { get; set; } = null;

        [DefaultValue(null)]
        public int? Max { get; set; } = null;

        [DefaultValue(null)]        
        public string? CustomDicePattern { get; set; } = null;
    }
}
