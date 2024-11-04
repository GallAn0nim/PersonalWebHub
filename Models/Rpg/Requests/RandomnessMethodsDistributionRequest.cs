using PersonalWebHub.Models.Rpg.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PersonalWebHub.Models.Rpg.Requests
{
    public class RandomnessMethodsDistributionRequest
    {

        [Range(1, int.MaxValue, ErrorMessage = "Number of rolls must be greater than 0.")]
        [DefaultValue(1000000)]
        public int NumberOfRolls { get; set; } = 1000000;
        [DefaultValue(1)]
        public int Min { get; set; } = 1;
        [DefaultValue(20)]
        public int Max { get; set; } = 20;
    }
}
