using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Models;
using PersonalWebHub.Models.Rpg.Requests;
using PersonalWebHub.Services.Rpg.Interfaces;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using PersonalWebHub.Models.Rpg.Responses;

namespace PersonalWebHub.Services.Rpg.Implementation
{
    public class DiceRollService(Random rnd, RandomNumberGenerator rng) : IDiceRollService
    {
        public DicesRollResponse RollDice(DicesRollRequest request)
        {
            var rollPlan = PrepareRollPlan(request);
            var diceRolls = ResolveDiceRolls(rollPlan.RandomnessMethod, rollPlan.DiceRollInput);
            diceRolls = ResolveRollPlan(rollPlan, diceRolls);
            var rollsStatistic = PrepareRollStatistic(rollPlan, diceRolls);
            var result = PrepareResponse(rollPlan, diceRolls, rollsStatistic);
            return result;
        }

        public FormulaRollResponse RollDiceFormula(FormulaRollRequest request)
        {
            request.RollFormula = CleanFormula(request.RollFormula);
            var matchingDices = FindDicesPatternsInFormula(request.RollFormula);
            var result = ResolveDicePatternsAndFormula(request, matchingDices);
            result.RollsStatistic = CalculateGeneralStatistic(result.Results);
            result = PrepareResponse(request, result);
            return result;
        }

        public RandomnessMethodsDistributionResponse RandomnessMethodsDistribution(
            RandomnessMethodsDistributionRequest randomnessMethodsDistributionRequest,
            RandomnessMethods randomnessMethod)
        {
            var rollResult = new List<int>();
            for (var i = 0; i < randomnessMethodsDistributionRequest.NumberOfRolls; i++)
            {
                rollResult.Add(GetRandomValue(randomnessMethod, randomnessMethodsDistributionRequest.Min,
                    randomnessMethodsDistributionRequest.Max));
            }

            var groupedNumbers = GroupRollResult(rollResult, randomnessMethodsDistributionRequest.NumberOfRolls);
            var result = PrepareResponse(randomnessMethodsDistributionRequest, randomnessMethod, groupedNumbers);
            return result;
        }

        private List<DiceRoll> ResolveDiceRolls(RandomnessMethods randomnessMethod, DiceRollInput diceRollInput)
        {
            var result = new List<DiceRoll>();

            for (var i = 0; i < diceRollInput.NumberOfRolls; i++)
            {
                var roll = new DiceRoll
                {
                    Dice = diceRollInput.Type.ToString(),
                    Result = GetRandomValue(randomnessMethod, diceRollInput.Min, diceRollInput.Max).ToString()
                };
                result.Add(roll);
            }

            return result;
        }

        private int GetRandomValue(RandomnessMethods randomnessMethod, int min, int max)
        {
            return randomnessMethod switch
            {
                RandomnessMethods.CSharpRandom => rnd.Next(min, max + 1),
                RandomnessMethods.CSharpCryptographyRandomNumberGenerator => GetCryptoRandomValue(min, max),
                _ => throw new InvalidOperationException("Invalid randomness method")
            };
        }

        private int GetCryptoRandomValue(int min, int max)
        {
            var range = max - min + 1;
            var randomNumber = new byte[4];
            const uint maxRandomValue = uint.MaxValue;
            var acceptableRange = maxRandomValue - (maxRandomValue % (uint)range);

            uint rngResult;
            do
            {
                rng.GetBytes(randomNumber);
                rngResult = BitConverter.ToUInt32(randomNumber, 0);
            } while (rngResult >= acceptableRange);

            return (int)(rngResult % (uint)range + min);
        }

        private FormulaRollResponse ResolveDicePatternsAndFormula(FormulaRollRequest request, List<Match> matchingDices)
        {
            var result = new FormulaRollResponse();
            var formulaResults = new List<string?>();

            for (var i = 0; i < request.NumberOfRolls; i++)
            {
                List<DiceRoll> diceRolls = [];
                var modifiableFormula = request.RollFormula;

                foreach (var matchedDie in matchingDices)
                {
                    var singleMatchPlan = PrepareRollPlan(request, matchedDie);
                    var singleRollResult =
                        ResolveDiceRolls(singleMatchPlan.RandomnessMethod, singleMatchPlan.DiceRollInput);
                    singleRollResult = ResolveRollPlan(singleMatchPlan, singleRollResult);
                    var rollsStatistic = PrepareRollStatistic(singleMatchPlan, singleRollResult);
                    modifiableFormula = UpdateFormulaWithRollValues(modifiableFormula, matchedDie.Value,
                        matchedDie.Length, rollsStatistic.ResultSum);
                    diceRolls.AddRange(singleRollResult);
                }

                formulaResults.Add(CalculateFormula(modifiableFormula));

                result.FormulaRolls.Add(PrepareFormulaRollSummary(request, modifiableFormula, diceRolls));
            }

            result.Results = string.Join(", ", formulaResults.Select(d => d));
            return result;
        }

        private static List<RollOccurrence> GroupRollResult(List<int> rollResult, int numberOfRolls)
        {
            return rollResult
                .GroupBy(num => num)
                .Select(group => (new RollOccurrence
                {
                    Value = group.Key, Occurrence = group.Count(),
                    Probability = ((double)group.Count() * 100) / numberOfRolls
                }))
                .OrderBy(groupedNumbers => groupedNumbers.Value)
                .ToList();
        }

        private static FormulaRollResponse PrepareResponse(FormulaRollRequest rollPlan, FormulaRollResponse diceRolls)
        {
            diceRolls.RandomnessMethod = rollPlan.RandomnessMethod;
            diceRolls.NumberOfRolls = rollPlan.NumberOfRolls;
            diceRolls.Formula = rollPlan.RollFormula;

            return diceRolls;
        }

        private static RandomnessMethodsDistributionResponse PrepareResponse(
            RandomnessMethodsDistributionRequest randomnessMethodsDistributionRequest,
            RandomnessMethods randomnessMethod, List<RollOccurrence> groupedNumbers)
        {
            var result = new RandomnessMethodsDistributionResponse
            {
                NumberOfRolls = randomnessMethodsDistributionRequest.NumberOfRolls,
                Min = randomnessMethodsDistributionRequest.Min,
                Max = randomnessMethodsDistributionRequest.Max,
                RandomnessMethod = randomnessMethod,
                DiceRolls = groupedNumbers,
                ResultAvarage = groupedNumbers.Average(rollOccurrence => rollOccurrence.Value),
                PercentageAvarage = groupedNumbers.Average(rollOccurrence => rollOccurrence.Probability),
                PercentageDeviation = groupedNumbers.Max(rollOccurrence => rollOccurrence.Probability) -
                                      groupedNumbers.Min(rollOccurrence => rollOccurrence.Probability)
            };

            return result;
        }

        private static RollStatistic PrepareRollStatistic(RollPlan rollPlan, List<DiceRoll> diceRolls)
        {
            var rollsStatistic = new RollStatistic();
            if (rollPlan.CalculateGeneralStatistic)
            {
                rollsStatistic = CalculateGeneralStatistic(diceRolls);
            }

            return rollsStatistic;
        }

        private static List<DiceRoll> ResolveRollPlan(RollPlan rollPlan, List<DiceRoll> diceRolls)
        {
            if (rollPlan.AssignCustomDicePattern)
            {
                diceRolls = AssignCustomDicePattern(diceRolls, rollPlan.CustomDicePattern);
            }

            if (rollPlan.AssignDiceRange)
            {
                diceRolls = AssignDiceRange(rollPlan.DiceRollInput, diceRolls);
            }

            return diceRolls;
        }

        private static FormulaRollSummary PrepareFormulaRollSummary(FormulaRollRequest request,
            string modifiableFormula, List<DiceRoll> diceRolls)
        {
            var formulaRollSummary = new FormulaRollSummary
            {
                Formula = request.RollFormula,
                Result = CalculateFormula(modifiableFormula),
                RollResults = string.Join(", ", diceRolls.Select(d => d.Result)),
                DiceRolls = diceRolls
            };

            return formulaRollSummary;
        }

        private static string CalculateFormula(string modifiableFormula)
        {
            var table = new DataTable();

            return table.Compute(modifiableFormula, null).ToString() ?? string.Empty;
        }

        private static string UpdateFormulaWithRollValues(string modifiableFormula, string matchedDieValue,
            int? matchedDieLength, int? rollsStatisticResultSum)
        {
            var firstIndex = modifiableFormula.IndexOf(matchedDieValue, StringComparison.OrdinalIgnoreCase);
            modifiableFormula = modifiableFormula[..firstIndex] + rollsStatisticResultSum +
                                modifiableFormula[(int)(firstIndex + matchedDieLength)!..];

            return modifiableFormula;
        }

        private static List<Match> FindDicesPatternsInFormula(string requestRollFormula)
        {
            const string pattern = @"(\d*)d(\d+)";
            var matches = Regex.Matches(requestRollFormula, pattern, RegexOptions.IgnoreCase).ToList();

            return matches;
        }

        private static string CleanFormula(string requestRollFormula)
        {
            var result = requestRollFormula.Replace(" ", "");
            result = result.Replace('d', 'D');
            return result;
        }

        private static List<DiceRoll> AssignDiceRange(DiceRollInput diceRollInput, List<DiceRoll> diceRolls)
        {
            foreach (var dice in diceRolls)
            {
                dice.Dice += $" ({diceRollInput.Min} - {diceRollInput.Max})";
            }

            return diceRolls;
        }

        private static DicesRollResponse PrepareResponse(RollPlan rollPlan, List<DiceRoll> diceRolls,
            RollStatistic rollsStatistic)
        {
            var result = new DicesRollResponse
            {
                DiceRolls = diceRolls,
                Type = rollPlan.DiceRollInput.Type,
                Results = string.Join(", ", diceRolls.Select(d => d.Result)),
                RandomnessMethod = rollPlan.RandomnessMethod,
                RollsStatistic = rollsStatistic,
                NumberOfRolls = rollPlan.DiceRollInput.NumberOfRolls
            };

            return result;
        }

        private static List<DiceRoll> AssignCustomDicePattern(List<DiceRoll> diceRolls, List<string> customDicePattern)
        {
            foreach (var dice in diceRolls)
            {
                dice.Result = customDicePattern[int.Parse(dice.Result) - 1];
                dice.Dice += $" ({string.Join(",", customDicePattern)})";
            }

            return diceRolls;
        }

        private static RollStatistic CalculateGeneralStatistic(List<DiceRoll> diceRolls)
        {
            var result = new RollStatistic
            {
                ResultMin = diceRolls.Min(roll => int.Parse(roll.Result)),
                ResultMax = diceRolls.Max(roll => int.Parse(roll.Result)),
                ResultSum = diceRolls.Sum(roll => int.Parse(roll.Result)),
                ResultAverage = diceRolls.Average(roll => int.Parse(roll.Result))
            };

            return result;
        }

        private static RollStatistic CalculateGeneralStatistic(string diceRolls)
        {
            var rolls = diceRolls.Split(',').ToList().Select(item => item.Trim()).Select(int.Parse).ToList();
            var result = new RollStatistic
            {
                ResultMin = rolls.Min(roll => roll),
                ResultMax = rolls.Max(roll => roll),
                ResultSum = rolls.Sum(roll => roll),
                ResultAverage = rolls.Average(roll => roll)
            };

            return result;
        }

        private static RollPlan PrepareRollPlan(DicesRollRequest request)
        {
            if (request.NumberOfRolls <= 0)
            {
                throw new ArgumentException("Invalid NumberOfRolls. Number of rolls must be greater then 0.");
            }

            var result = PrepareBasePlan(request);

            switch (request.Type)
            {
                case DiceTypes.D2 or DiceTypes.D4 or DiceTypes.D6 or DiceTypes.D8 or DiceTypes.D10 or DiceTypes.D12
                    or DiceTypes.D20 or DiceTypes.D100:
                    result = SetStandardDiceRange(result, request.Type);
                    break;

                case DiceTypes.CustomSize:
                    result = SetCustomSizeRange(result, request.Min, request.Max);
                    break;

                case DiceTypes.CustomDice:
                    result = SetCustomDicePlan(result);
                    break;
            }

            return result;
        }

        private static RollPlan PrepareBasePlan(DicesRollRequest request)
        {
            var result = new RollPlan
            {
                RandomnessMethod = request.RandomnessMethod,
                DiceRollInput =
                {
                    NumberOfRolls = request.NumberOfRolls,
                    Type = request.Type
                },
                CustomDicePattern = request.CustomDicePattern?.Split(',').ToList()!,
                AssignCustomDicePattern = false,
                CalculateGeneralStatistic = true,
                AssignDiceRange = true
            };

            return result;
        }

        private static RollPlan SetCustomSizeRange(RollPlan result, int? requestMin, int? requestMax)
        {
            if (requestMin == null || requestMax == null || requestMin >= requestMax)
                throw new ArgumentException("Invalid custom dice size. Min and Max must be properly defined.");

            result.DiceRollInput.Min = (int)requestMin;
            result.DiceRollInput.Max = (int)requestMax;

            return result;
        }

        private static RollPlan SetStandardDiceRange(RollPlan result, DiceTypes type)
        {
            result.DiceRollInput.Min = 1;
            result.DiceRollInput.Max =
                int.Parse(type.ToString().Replace("D", "", StringComparison.InvariantCultureIgnoreCase));
            return result;
        }

        private static RollPlan SetCustomDicePlan(RollPlan result)
        {
            if (result.CustomDicePattern == null)
                throw new ArgumentException("CustomDicePattern needs to be defined for Custom dice.");

            result.DiceRollInput.Min = 1;
            result.DiceRollInput.Max = result.CustomDicePattern.Count;
            result.AssignCustomDicePattern = true;
            result.CalculateGeneralStatistic = result.CustomDicePattern.All(item => !Regex.IsMatch(item, @"[a-zA-Z]"));

            return result;
        }

        private static RollPlan PrepareRollPlan(FormulaRollRequest request, Match match)
        {
            var result = new RollPlan();
            var rollsString = match.Groups[1].Value;
            var rolls = string.IsNullOrEmpty(rollsString) ? 1 : int.Parse(rollsString);

            if (rolls <= 0)
            {
                throw new ArgumentException("Invalid NumberOfRolls. Number of rolls must be greater then 0.");
            }

            result.RandomnessMethod = request.RandomnessMethod;
            result.CalculateGeneralStatistic = true;
            result.AssignDiceRange = true;
            result.DiceRollInput.NumberOfRolls = rolls;
            result.DiceRollInput.Min = 1;

            result.DiceRollInput.Type = Enum.TryParse("D" + match.Groups[2].Value, true, out DiceTypes diceType)
                ? diceType
                : DiceTypes.CustomSize;

            result.DiceRollInput.Max = result.DiceRollInput.Type switch
            {
                DiceTypes.D2 or DiceTypes.D4 or DiceTypes.D6 or DiceTypes.D8 or DiceTypes.D10 or DiceTypes.D12
                    or DiceTypes.D20 or DiceTypes.D100 => int.Parse(result.DiceRollInput.Type.ToString()
                        .Replace("D", "", StringComparison.InvariantCultureIgnoreCase)),
                DiceTypes.CustomSize => int.Parse(match.Groups[2].Value),
                _ => result.DiceRollInput.Max
            };

            return result;
        }
    }
}