using PersonalWebHub.Models.Rpg.Enum;
using System.ComponentModel;

namespace PersonalWebHub.Models.Rpg.Models
{
    public class DiceRollInput
    {
        public int NumberOfRolls { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public DiceTypes Type { get; set; }
    }
}
