// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.Workflow.Brokers;

internal class UserBroker(ICoreContextFactory coreContextFactory) : IUserBroker
{
    public IQueryable<User> GetAllUsers(bool ignoreFilters)
    {
        CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return ignoreFilters
            ? coreDataContext.Users.IgnoreQueryFilters()
            : coreDataContext.Users;
    }

    public async ValueTask<User> AddUserAsync(User entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User result = (await coreDataContext.Users.AddAsync(entity)).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<User> UpdateUserAsync(User entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        User result = coreDataContext.Users.Update(entity).Entity;
        _ = await coreDataContext.SaveChangesAsync();
        return result;
    }

    public async ValueTask<int> DeleteUserAsync(User entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Users.Remove(entity);
        return await coreDataContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAllUsersAsync(IEnumerable<User> items)
    {
        if (items == null || !items.Any())
            return;

        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        coreDataContext.Users.RemoveRange(items);
        _ = await coreDataContext.SaveChangesAsync();
    }

    public int? GetAppId(User entity)
    {
        using CoreDataContext coreDataContext = coreContextFactory.CreateCoreContext();
        return coreDataContext.UserRoles
            .Where(userRole => userRole.UserId == entity.Id)
            .Join(
                coreDataContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => (int?)role.AppId)
            .FirstOrDefault();
    }
}