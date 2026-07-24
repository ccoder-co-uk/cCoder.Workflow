// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Brokers;

internal class AuthorizationBroker(ICoreContextFactory coreContextFactory) 
    : IAuthorizationBroker
{
    public User GetCurrentUser()
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.User;
    }

    public User GetUser(string id)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return LoadUserWithRoles(coreDataContext, id);
    }

    public bool IsAdminOfApp(int? appId)
    {
        User user = GetCurrentUser();
        return user != null && appId.HasValue && HasAppAdminPrivilege(user, appId.Value);
    }

    public bool IsAdminOfApp(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        User user = coreDataContext.Users
            .Include(foundUser => foundUser.Roles)
            .FirstOrDefault(foundUser => foundUser.Id == userName);

        return user != null && HasAppAdminPrivilege(user, appId);
    }

    public void Authorize(int? appId, string privilege)
    {
        User user = GetCurrentUser();

        if (user == null || !(HasAppAdminPrivilege(user, appId) || HasPrivilege(user, appId, privilege)))
            throw new SecurityException("Access Denied!");
    }

    public void Authorize(string userId, int? appId, string privilege)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new SecurityException("Access Denied!");

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User user = LoadUserWithRoles(coreDataContext, userId);

        if (!(HasAppAdminPrivilege(user, appId) || HasPrivilege(user, appId, privilege)))
            throw new SecurityException("Access Denied!");
    }

    public bool UserBelongsToApp(string userId, int? appId)
    {
        if (string.IsNullOrWhiteSpace(userId) || !appId.HasValue)
            return false;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return coreDataContext.UserRoles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId)
            .Join(
                coreDataContext.Roles.IgnoreQueryFilters().AsNoTracking(),
                userRole => userRole.RoleId,
                role => role.Id,
                (_, role) => role.AppId)
            .Any(foundAppId => foundAppId == appId.Value);
    }

    private static bool HasPrivilege(User user, int? appId, string privilege)
    {
        string normalizedPrivilege = privilege?.ToLowerInvariant() ?? string.Empty;

        return (appId != null && HasAppAdminPrivilege(user, appId.Value))
            || (user.Roles?.Any(role =>
                (appId == null || role.Role?.AppId == appId)
                && (role.Role?.Privileges?.Contains(normalizedPrivilege) ?? false))
                ?? false);
    }

    private static bool HasAppAdminPrivilege(User user, int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(role =>
            role.Role?.AppId == appId.Value
            && (role.Role?.Privileges?.Contains("app_admin") ?? false)) ?? false);

    private static User LoadUserWithRoles(CoreDataContext coreDataContext, string userId) =>
        coreDataContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(foundUser => foundUser.Roles)
                .ThenInclude(userRole => userRole.Role)
            .FirstOrDefault(foundUser => foundUser.Id == userId);
}