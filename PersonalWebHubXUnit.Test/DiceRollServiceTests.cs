using System.Security.Cryptography;
using Moq;
using PersonalWebHub.Models.Rpg.Enum;
using PersonalWebHub.Models.Rpg.Requests;
using PersonalWebHub.Services.Rpg;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace PersonalWebHubXUnit.Test
{
    public class DiceRollServiceTests
    {
        private readonly Mock<Random> _randomMock;
        private readonly Mock<RandomNumberGenerator> _rngMock;
        private readonly IDiceRollService _diceRollService;

        public DiceRollServiceTests()
        {
            _randomMock = new Mock<Random>();
            _rngMock = new Mock<RandomNumberGenerator>();
            _diceRollService = new DiceRollService(_randomMock.Object, _rngMock.Object);
        }

        [Theory]
        [InlineData(DiceTypes.D2, 1, 2)]
        [InlineData(DiceTypes.D4, 1, 4)]
        [InlineData(DiceTypes.D6, 1, 6)]
        [InlineData(DiceTypes.D8, 1, 8)]
        [InlineData(DiceTypes.D10, 1, 10)]
        [InlineData(DiceTypes.D12, 1, 12)]
        [InlineData(DiceTypes.D20, 1, 20)]
        [InlineData(DiceTypes.D100, 1, 100)]
        [InlineData(DiceTypes.CustomSize, 1, 30)]
        [InlineData(DiceTypes.CustomSize, 5, 50)]
        public void RollDice_ShouldReturnValidResponse_ForAllDiceTypes(DiceTypes diceType, int min, int max)
        {
            // Arrange
            var request = new DicesRollRequest
            {
                NumberOfRolls = 3,
                Type = diceType,
                Min = min,
                Max = max,
                RandomnessMethod = RandomnessMethods.CSharpRandom
            };
            _randomMock.Setup(r => r.Next(min, max + 1)).Returns(min);

            // Act
            var response = _diceRollService.RollDice(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.NumberOfRolls);
            Assert.Equal(diceType, response.Type);
            Assert.Equal(RandomnessMethods.CSharpRandom, response.RandomnessMethod);
            Assert.NotEmpty(response.Results);
            Assert.Equal(3, response.DiceRolls.Count);
            Assert.All(response.DiceRolls, roll => { Assert.Equal(min.ToString(), roll.Result); });
            var expectedResults = string.Join(", ", min.ToString(), min.ToString(), min.ToString());
            Assert.Equal(expectedResults, response.Results);
            Assert.NotNull(response.RollsStatistic);
            Assert.Equal(min * 3, response.RollsStatistic.ResultSum);
            Assert.Equal(min, response.RollsStatistic.ResultMin);
            Assert.Equal(min, response.RollsStatistic.ResultMax);
            Assert.Equal(min, response.RollsStatistic.ResultAverage);
        }

        [Theory]
        [InlineData(new[] { "1", "2", "3" }, 3)]
        [InlineData(new[] { "10", "20", "30", "40" }, 4)]
        [InlineData(new[] { "North", "South", "East", "West" }, 5)]
        [InlineData(new[] { "5", "10", "15", "Critical" }, 4)]
        [InlineData(new[] { "Sword", "2", "Shield", "4" }, 6)]
        public void RollDice_ShouldReturnValidResponse_ForCustomDicePatterns(string[] customPattern, int numberOfRolls)
        {
            // Arrange
            var request = new DicesRollRequest
            {
                NumberOfRolls = numberOfRolls,
                Type = DiceTypes.CustomDice,
                CustomDicePattern = string.Join(",", customPattern),
                RandomnessMethod = RandomnessMethods.CSharpRandom
            };
            _randomMock.Setup(r => r.Next(1, customPattern.Length + 1)).Returns(1);

            // Act
            var response = _diceRollService.RollDice(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(numberOfRolls, response.NumberOfRolls);
            Assert.Equal(RandomnessMethods.CSharpRandom, response.RandomnessMethod);
            Assert.Equal(numberOfRolls, response.DiceRolls.Count);
            Assert.All(response.DiceRolls, roll => { Assert.Contains(roll.Result, customPattern); });
            var expectedResults = string.Join(", ", Enumerable.Repeat(customPattern[0], numberOfRolls));
            Assert.Equal(expectedResults, response.Results);
        }

        [Theory]
        [InlineData("1d6+3", 7, 4)]
        [InlineData("2d6+3", 12, 4, 5)]
        [InlineData("1d8+2", 8, 6)]
        [InlineData("2d6+1d8+3", 18, 4, 5, 6)]
        [InlineData("2d6-2", 7, 4, 5)]
        [InlineData("1d10-3", 4, 7)]
        public void RollDiceFormula_ShouldHandleFormulaAndReturnValidResponse(string rollFormula, int expectedResult,
            params int[] rolls)
        {
            // Arrange
            var request = new FormulaRollRequest
            {
                NumberOfRolls = 1,
                RollFormula = rollFormula
            };

            var rollQueue = new Queue<int>(rolls);
            _randomMock.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(() => rollQueue.Dequeue());

            // Act
            var response = _diceRollService.RollDiceFormula(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(1, response.NumberOfRolls);
            Assert.Equal(rollFormula.ToUpper(), response.Formula);
            Assert.Equal(RandomnessMethods.CSharpRandom, response.RandomnessMethod);
            Assert.Contains(expectedResult.ToString(), response.Results);
        }

        [Theory]
        [InlineData(100, 1, 6, 3)]
        [InlineData(50, 2, 5, 4)]
        [InlineData(200, 10, 20, 15)]
        [InlineData(75, 1, 10, 7)]
        public void RandomnessMethodsDistribution_ShouldReturnDistributionResponse(int numberOfRolls, int min, int max,
            int expectedRoll)
        {
            // Arrange
            var request = new RandomnessMethodsDistributionRequest
            {
                NumberOfRolls = numberOfRolls,
                Min = min,
                Max = max
            };
            _randomMock.Setup(r => r.Next(min, max + 1)).Returns(expectedRoll);

            // Act
            var response = _diceRollService.RandomnessMethodsDistribution(request, RandomnessMethods.CSharpRandom);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(numberOfRolls, response.NumberOfRolls);
            Assert.Equal(min, response.Min);
            Assert.Equal(max, response.Max);
            Assert.Equal(RandomnessMethods.CSharpRandom, response.RandomnessMethod);
            Assert.All(response.DiceRolls, roll =>
            {
                Assert.InRange(roll.Value, min, max);
                Assert.Equal(expectedRoll, roll.Value);
            });
            Assert.NotNull(response.DiceRolls);
            Assert.Equal(expectedRoll, response.DiceRolls.Min(roll => roll.Value));
            Assert.Equal(expectedRoll, response.DiceRolls.Max(roll => roll.Value));
        }

        [Theory]
        [InlineData(DiceTypes.D2, 1, 2)]
        [InlineData(DiceTypes.D4, 1, 4)]
        [InlineData(DiceTypes.D6, 1, 6)]
        [InlineData(DiceTypes.D8, 1, 8)]
        [InlineData(DiceTypes.D10, 1, 10)]
        [InlineData(DiceTypes.D12, 1, 12)]
        [InlineData(DiceTypes.D20, 1, 20)]
        [InlineData(DiceTypes.D100, 1, 100)]
        [InlineData(DiceTypes.CustomSize, 1, 30)]
        [InlineData(DiceTypes.CustomSize, 5, 50)]
        public void RollDice_ShouldReturnValidResponse_ForAllDiceTypes_WithCryptoRandom(DiceTypes diceType, int min,
            int max)
        {
            // Arrange
            var request = new DicesRollRequest
            {
                NumberOfRolls = 3,
                Type = diceType,
                Min = min,
                Max = max,
                RandomnessMethod = RandomnessMethods.CSharpCryptographyRandomNumberGenerator
            };

            var randomBytes = new byte[] { 0, 0, 0, 2 };
            _rngMock.Setup(r => r.GetBytes(It.IsAny<byte[]>()))
                .Callback<byte[]>(b => Array.Copy(randomBytes, b, randomBytes.Length));

            // Act
            var response = _diceRollService.RollDice(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.NumberOfRolls);
            Assert.Equal(RandomnessMethods.CSharpCryptographyRandomNumberGenerator, response.RandomnessMethod);
            Assert.NotEmpty(response.Results);
            Assert.Equal(3, response.DiceRolls.Count);
            Assert.All(response.DiceRolls, roll => { Assert.InRange(int.Parse(roll.Result), min, max); });
        }

        [Theory]
        [InlineData(new[] { "1", "2", "3" }, 3)]
        [InlineData(new[] { "10", "20", "30", "40" }, 4)]
        [InlineData(new[] { "North", "South", "East", "West" }, 5)]
        [InlineData(new[] { "5", "10", "15", "Critical" }, 4)]
        [InlineData(new[] { "Sword", "2", "Shield", "4" }, 6)]
        public void RollDice_ShouldReturnValidResponse_ForCustomDicePatterns_WithCryptoRandom(string[] customPattern,
            int numberOfRolls)
        {
            // Arrange
            var request = new DicesRollRequest
            {
                NumberOfRolls = numberOfRolls,
                Type = DiceTypes.CustomDice,
                CustomDicePattern = string.Join(",", customPattern),
                RandomnessMethod = RandomnessMethods.CSharpCryptographyRandomNumberGenerator
            };
            _rngMock.Setup(r => r.GetBytes(It.IsAny<byte[]>()))
                .Callback<byte[]>(b => b[0] = 0);

            // Act
            var response = _diceRollService.RollDice(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(numberOfRolls, response.NumberOfRolls);
            Assert.Equal(RandomnessMethods.CSharpCryptographyRandomNumberGenerator, response.RandomnessMethod);
            Assert.Equal(numberOfRolls, response.DiceRolls.Count);
            Assert.All(response.DiceRolls, roll => { Assert.Contains(roll.Result, customPattern); });
        }

        [Theory]
        [InlineData(100, 1, 6, 3)]
        [InlineData(50, 2, 5, 4)]
        [InlineData(200, 10, 20, 15)]
        [InlineData(75, 1, 10, 7)]
        public void RandomnessMethodsDistribution_ShouldReturnDistributionResponse_WithCryptoRandom(int numberOfRolls,
            int min, int max, int expectedRoll)
        {
            // Arrange
            var request = new RandomnessMethodsDistributionRequest
            {
                NumberOfRolls = numberOfRolls,
                Min = min,
                Max = max
            };
            _rngMock.Setup(r => r.GetBytes(It.IsAny<byte[]>()))
                .Callback<byte[]>(b => b[0] = 0);

            // Act
            var response = _diceRollService.RandomnessMethodsDistribution(request,
                RandomnessMethods.CSharpCryptographyRandomNumberGenerator);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(numberOfRolls, response.NumberOfRolls);
            Assert.Equal(min, response.Min);
            Assert.Equal(max, response.Max);
            Assert.Equal(RandomnessMethods.CSharpCryptographyRandomNumberGenerator, response.RandomnessMethod);
            Assert.All(response.DiceRolls, roll => { Assert.InRange(roll.Value, min, max); });
        }
    }
}