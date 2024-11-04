using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Requests;
using PersonalWebHub.Models.Rpg.Responses;

namespace PersonalWebHub.Services.Rpg.Interfaces
{
    public interface IDiceRollService
    {
        DicesRollResponse RollDice(DicesRollRequest request);

        FormulaRollResponse RollDiceFormula(FormulaRollRequest request);
        RandomnessMethodsDistributionResponse RandomnessMethodsDistribution(RandomnessMethodsDistributionRequest randomnessMethodsDistributionRequest, RandomnessMethods randomnessMethod);
    }
}
