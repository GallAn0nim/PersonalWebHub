using PersonalWebHub.Models.Tools;

namespace PersonalWebHub.Services.Tools;

public interface IObfuscateService
{
    public string Obfuscate(ObfuscateTextRequest request);
    
    public string Deobfuscate(DeobfuscateTextRequest request);
}