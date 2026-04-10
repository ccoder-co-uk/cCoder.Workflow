using System.Security;
using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Workflow.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();
    bool IsAdminOfApp(int? appId);
    bool IsAdmin(int appId, string userName);
    void Authorize(int? appId, string privilege);
}

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public bool IsAdminOfApp(int? appId)
    {
        User user = GetCurrentUser();
        return user != null && appId.HasValue && HasAppAdminPrivilege(user, appId.Value);
    }

    public bool IsAdmin(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        User user = coreDataContext.Users
            .Include(foundUser => foundUser.Roles)
            .FirstOrDefault(foundUser => foundUser.Id == userName);

        App app = coreDataContext.Apps
            .Include(foundApp => foundApp.Roles.Select(role => role.Users))
            .FirstOrDefault(foundApp => foundApp.Id == appId);

        return app?.IsAppAdmin(user) ?? false;
    }

    public void Authorize(int? appId, string privilege)
    {
        User user = GetCurrentUser();

        if (user == null || !(HasAppAdminPrivilege(user, appId) || HasPrivilege(user, appId, privilege)))
            throw new SecurityException("Access Denied!");
    }

    private static bool HasPrivilege(User user, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege.ToLower();

        return (appId != null && HasAppAdminPrivilege(user, appId.Value))
            || (user.Roles?.Any(role =>
                (appId == null || role.Role.AppId == appId)
                && role.Role.Privileges.Contains(normalizedPrivilege))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(User user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(role => role.Role.AppId == appId.Value && role.Role.Allows(user, "app_admin")) ?? false);
}



