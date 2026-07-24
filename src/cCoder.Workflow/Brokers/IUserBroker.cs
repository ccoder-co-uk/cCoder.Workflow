// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.Workflow.Brokers;

public interface IUserBroker
{
    IQueryable<User> GetAllUsers(bool ignoreFilters);

    ValueTask<User> AddUserAsync(User newEntity);

    ValueTask<User> UpdateUserAsync(User updatedEntity);

    ValueTask<int> DeleteUserAsync(User deletedEntity);

    ValueTask DeleteAllUsersAsync(IEnumerable<User> deletedItems);

    int? SelectAppId(User entity);
}