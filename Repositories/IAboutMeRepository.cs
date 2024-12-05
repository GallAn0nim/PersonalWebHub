using PersonalWebHub.Models.AboutMe;

namespace PersonalWebHub.Repositories;

public interface IAboutMeRepository
{
    Task<List<SocialMedia>> GetSocialMedias();
}