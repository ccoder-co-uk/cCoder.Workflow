using cCoder.Data.Models.Security;

namespace cCoder.Workflow.Brokers;

public interface IUserBroker
{
    IQueryable<User> GetAllUsers(bool ignoreFilters);
    ValueTask<User> AddUserAsync(User entity);
    ValueTask<User> UpdateUserAsync(User entity);
    ValueTask<int> DeleteUserAsync(User entity);
    ValueTask DeleteAllUsersAsync(IEnumerable<User> items);
    int? GetAppId(User entity);
}


