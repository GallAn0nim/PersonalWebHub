using PersonalWebHub.Models.Rpg.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonalWebHub.Models.Rpg.Requests
{
    public class FormulaRollRequest
    {        
        [Range(1, int.MaxValue, ErrorMessage = "Number of rolls must be greater than 0.")]
        [DefaultValue(1)]
        public int NumberOfRolls { get; set; } = 1;
               
        [DefaultValue(RandomnessMethods.CSharpRandom)]
        public RandomnessMethods RandomnessMethods { get; set; } = RandomnessMethods.CSharpRandom;

        [Required]
        [DefaultValue("1d20+11")]
        public string RollFormula { get; set; } = "1d20+11";
    }
}
