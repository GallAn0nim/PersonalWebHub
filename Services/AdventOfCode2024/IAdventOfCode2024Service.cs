using PersonalWebHub.Models.AdventOfCode2024;
using PersonalWebHub.Models.Tools;

namespace PersonalWebHub.Services.AdventOfCode2024;

public interface IAdventOfCode2024Service
{
    public string GetGeneralInfo();
    public string GetTaskDescription(int day,bool partTwo = false);
    
    public string GetTaskParams(int day);
    public AdventOfCode2024SolutionResponse GetTaskSolution(int day,bool partTwo = false);

}