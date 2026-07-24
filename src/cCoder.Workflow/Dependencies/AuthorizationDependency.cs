// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Dependencies;

internal sealed class AuthorizationDependency(
    ICoreContextFactory coreContextFactory)
    : IAuthorizationDependency
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public User GetUser(string userId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return LoadUserWithRoles(coreDataContext: coreDataContext, userId: userId);
    }

    public bool IsAdminOfApp(int? appId)
    {
        User user = GetCurrentUser();
        return user != null && appId.HasValue && HasAppAdminPrivilege(user: user, appId: appId.Value);
    }

    public bool IsAdminOfApp(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        User user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        return user != null && HasAppAdminPrivilege(user: user, appId: appId);
    }

    public void Authorize(int? appId, string privilege)
    {
        User user = GetCurrentUser();

        if (user == null || !(HasAppAdminPrivilege(user: user, appId: appId) || HasPrivilege(user: user, appId: appId, privilege: privilege)))
        {
            throw new SecurityException("Access Denied!");
        }
    }

    public void Authorize(string userId, int? appId, string privilege)
    {
        if (string.IsNullOrWhiteSpace(value: userId))
        {
            throw new SecurityException("Access Denied!");
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User user = LoadUserWithRoles(coreDataContext: coreDataContext, userId: userId);

        if (!(HasAppAdminPrivilege(user: user, appId: appId) || HasPrivilege(user: user, appId: appId, privilege: privilege)))
        {
            throw new SecurityException("Access Denied!");
        }
    }

    public bool UserBelongsToApp(string userId, int? appId)
    {
        if (string.IsNullOrWhiteSpace(value: userId) || !appId.HasValue)
        {
            return false;
        }

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.UserRoles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(predicate: userRole => userRole.UserId == userId)
            .Join(
inner: coreDataContext.Roles.IgnoreQueryFilters()
            .AsNoTracking(),
outerKeySelector: userRole => userRole.RoleId,
innerKeySelector: role => role.Id,
resultSelector: (_, role) => role.AppId)
            .Any(predicate: foundAppId => foundAppId == appId.Value);
    }

    private static bool HasPrivilege(User user, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege?.ToLowerInvariant() ?? string.Empty;

        return (appId != null && HasAppAdminPrivilege(user: user, appId: appId.Value))
            || (user.Roles?.Any(predicate: role =>
                (appId == null || role.Role?.AppId == appId)
                && (role.Role?.Privileges?.Contains(item: normalizedPrivilege) ?? false))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(User user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(predicate: role =>
            role.Role?.AppId == appId.Value
            && (role.Role?.Privileges?.Contains(item: "app_admin") ?? false)) ?? false);

    private static User LoadUserWithRoles(CoreDataContext coreDataContext, string userId) =>
        coreDataContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
                .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userId);
}
