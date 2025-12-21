namespace SpaceReserve.Admin.AppService.Contracts;

public interface IKeyCloakAppService
{
    Task<string> GetAccessTokenAsync();
    Task UpdateUserStatusInKeycloakAsync(string subjectId, bool isActive);
    Task<bool> GetUserEnabledStatusAsync(string subjectId);
}
