// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using cCoder.CodeAnalysis.Exposures;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace cCoder.Workflow.Brokers;

internal class AuthorizationBroker(
    ICoreContextFactory coreContextFactory)
    : IAuthorizationBroker, IUtilityBroker
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

        return user?.IsAdminOfApp(appId: appId) ?? false;
    }

    public bool IsAdminOfApp(int appId, string userName)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        User user = coreDataContext.Users
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userName);

        return user?.IsAdminOfApp(appId: appId) ?? false;
    }

    public void Authorize(int? appId, string privilege)
    {
        User user = GetCurrentUser();

        EnsureAuthorized(
            user: user,
            appId: appId,
            privilege: privilege);
    }

    public void Authorize(string userId, int? appId, string privilege)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User user = LoadUserWithRoles(coreDataContext: coreDataContext, userId: userId);

        EnsureAuthorized(
            user: user,
            appId: appId,
            privilege: privilege);
    }

    public bool UserBelongsToApp(string userId, int? appId)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();

        return !string.IsNullOrWhiteSpace(value: userId)
            && appId.HasValue
            && coreDataContext.UserRoles
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

    private static User LoadUserWithRoles(CoreDataContext coreDataContext, string userId) =>
        coreDataContext.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(navigationPropertyPath: foundUser => foundUser.Roles)
                .ThenInclude(navigationPropertyPath: userRole => userRole.Role)
            .FirstOrDefault(predicate: foundUser => foundUser.Id == userId);

    private static void EnsureAuthorized(
        User user,
        int? appId,
        string privilege)
    {
        if (user?.Can(
            appId: appId,
            operation: privilege) != true)
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }
}