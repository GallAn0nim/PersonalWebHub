using PersonalWebHub.Models.Tools;

namespace PersonalWebHub.Services.AdventOfCode2024;

public interface IAdventOfCode2024Service
{
    public string GetGeneralInfo();
    public string GetTaskDescription(int day,bool partTwo = false);
    
    public string GetTaskParams(int day);
    public string GetTaskSolution(int day,bool partTwo = false);

}