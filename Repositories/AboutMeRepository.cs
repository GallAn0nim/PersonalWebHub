using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using PersonalWebHub.Models.AboutMe;
using PersonalWebHub.Models.AppSettings;
using PersonalWebHub.Models.Tools;
using PersonalWebHub.Services.Tools;

namespace PersonalWebHub.Repositories;

public class AboutMeRepository(IObfuscateService obfuscateService, IOptions<ObfuscateSettings> obfuscateSettings) : IAboutMeRepository
{
    private readonly string _secretKey = obfuscateSettings.Value.Key;
    private readonly string _csvFilePath = Path.Combine(AppContext.BaseDirectory, "resources", "SocialMedia.csv");

    public async Task<List<SocialMedia>> GetSocialMedias()
    {
        var obfuscatedHexText = await File.ReadAllTextAsync(_csvFilePath);
       
        var deobfuscatedText = obfuscateService.Deobfuscate(new DeobfuscateTextRequest
        {
            Text = obfuscatedHexText,
            Key = _secretKey 
        });
        using var reader = new StringReader(deobfuscatedText);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true 
        });
        var socialMediaList = new List<SocialMedia>();
        await foreach (var record in csv.GetRecordsAsync<SocialMedia>())
        {
            socialMediaList.Add(record);
        }

        return socialMediaList;
    }
}