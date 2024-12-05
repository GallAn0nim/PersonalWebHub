using PersonalWebHub.Repositories;

namespace PersonalWebHub.Services.AdventOfCode2024;

public class AdventOfCode2024Service(IAdventOfCode2024Repository adventOfCode2024Repository) : IAdventOfCode2024Service
{
    public string GetGeneralInfo()
    {
        return adventOfCode2024Repository.GetGeneralInfo();
    }

    public string GetTaskDescription(int day, bool partTwo)
    {
        return adventOfCode2024Repository.GetTaskDescription(day, partTwo);
    }

    public string GetTaskParams(int day)
    {
        return adventOfCode2024Repository.GetTaskParams(day);
    }

    public string GetTaskSolution(int day, bool partTwo)
    {
        switch (day)
        {
            case 1:
                return GetDay1Solution(partTwo);
            default:
                return string.Empty;
        }
    }

    private string GetDay1Solution(bool partTwo)
    {
        var taskParams = GetTaskParams(1);
        var firstColumn = new List<int>();
        var secondColumn = new List<int>();
        foreach (var line in taskParams.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var value1) ||
                !int.TryParse(parts[1], out var value2)) continue;
            firstColumn.Add(value1);
            secondColumn.Add(value2);
        }

        if (partTwo)
        {
            var sum = firstColumn.Sum(number => number * secondColumn.FindAll(n => n == number).Count);
            return sum.ToString();
        }
        else
        {
            firstColumn.Sort();
            secondColumn.Sort();

            var sum = firstColumn
                .Zip(secondColumn, (a, b) => Math.Abs(a - b)) 
                .Sum();
            return sum.ToString();
        }
    }
}