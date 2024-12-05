namespace PersonalWebHub.Repositories;

public class AdventOfCode2024Repository() : IAdventOfCode2024Repository
{
    public string GetGeneralInfo()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "resources/AdventOfCode2024", "GeneralInfo.txt");
        
        return File.ReadAllText(filePath);
    }
    public string GetTaskDescription(int day, bool partTwo)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "resources/AdventOfCode2024", $"Day{day}-Description.txt");
        
        if (partTwo)
        {
            filePath = Path.Combine(AppContext.BaseDirectory, "resources/AdventOfCode2024", $"Day{day}-Description-PartTwo.txt");
        }
        
        return File.ReadAllText(filePath);
    }

    public string GetTaskParams(int day)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "resources/AdventOfCode2024", $"Day{day}-Params.txt");
        
        return File.ReadAllText(filePath);
    }
}