namespace PersonalWebHub.Repositories;

public interface IAdventOfCode2024Repository
{
    public string GetGeneralInfo();
    public string GetTaskDescription(int day, bool partTwo);
    
    public string GetTaskParams(int day);
}