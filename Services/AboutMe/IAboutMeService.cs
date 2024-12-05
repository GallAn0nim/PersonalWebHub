using PersonalWebHub.Models.AboutMe;

namespace PersonalWebHub.Services.AboutMe;

public interface IAboutMeService
{
    Task<List<SocialMediaResponse>> GetSocialMedias(bool whySoSerious);
}