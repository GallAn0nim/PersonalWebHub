using System.Diagnostics;
using PersonalWebHub.Models.AdventOfCode2024;
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

    public AdventOfCode2024SolutionResponse GetTaskSolution(int day, bool partTwo)
    {
        switch (day)
        {
            case 1:
                return GetDay1Solution(partTwo);
            default:
                return new AdventOfCode2024SolutionResponse();
        }
    }

    private AdventOfCode2024SolutionResponse GetDay1Solution(bool partTwo)
    {
        var result = new AdventOfCode2024SolutionResponse();
        var stopwatch = Stopwatch.StartNew();
        var globalStopWatch = Stopwatch.StartNew();
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
        stopwatch.Stop();
        result.SolutionStatistics.Add($"Read parameters from file and split string in to two list: Execution Time = {stopwatch.ElapsedMilliseconds}ms");

        stopwatch.Restart();
        if (partTwo)
        {
            var sum = firstColumn.Sum(number => number * secondColumn.FindAll(n => n == number).Count);
            stopwatch.Stop();
            result.SolutionStatistics.Add($"Linq.Sum and FindAll: Execution Time = {stopwatch.ElapsedMilliseconds}ms");
            result.Solution = sum.ToString();
        }
        else
        {
            firstColumn.Sort();
            secondColumn.Sort();
            var sum = firstColumn
                .Zip(secondColumn, (a, b) => Math.Abs(a - b)) 
                .Sum();
            stopwatch.Stop();
            result.SolutionStatistics.Add($"Linq.Zip and Math Abs: Execution Time = {stopwatch.ElapsedMilliseconds}ms");
            result.Solution = sum.ToString();
        }
        globalStopWatch.Stop();
        result.SolutionStatistics.Add($"Execution Time = {globalStopWatch.ElapsedMilliseconds}ms");
        return result;
    }
}