using PersonalWebHub.Models.AboutMe;
using PersonalWebHub.Repositories;

namespace PersonalWebHub.Services.AboutMe;

public class AboutMeService(IAboutMeRepository aboutMeRepository) : IAboutMeService
{
    public async Task<List<SocialMediaResponse>> GetSocialMedias(bool whySoSerious)
    {
        var socialMedia = await aboutMeRepository.GetSocialMedias();
        
        var result = socialMedia.Where(sm=>!sm.IsSerious==whySoSerious).Select(sm=>new SocialMediaResponse{Name = sm.Name, Url = sm.Url, Description = sm.Description}).ToList();
        
        return result;
    }
}