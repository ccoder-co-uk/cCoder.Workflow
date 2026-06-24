using cCoder.Data.Models.Security;

namespace cCoder.Workflow.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();
    User GetUser(string id);
    bool IsAdminOfApp(int? appId);
    bool IsAdminOfApp(int appId, string userName);
    void Authorize(int? appId, string privilege);
    void Authorize(string userId, int? appId, string privilege);
    bool UserBelongsToApp(string userId, int? appId);
}
