using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.Core.Services.Tests;

internal static class TestUsers
{
    internal static User WithPrivilege(string privilege, int appId = 1) =>
        WithPrivileges([privilege], appId);

    internal static User WithPrivileges(IEnumerable<string> privileges, int appId = 1)
    {
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = appId,
            Name = "Test Role",
            Privs = string.Join(',', privileges.Select(p => p.ToLowerInvariant())),
        };

        User user = new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
        };

        UserRole userRole = new()
        {
            Role = role,
            RoleId = role.Id,
            User = user,
            UserId = user.Id,
        };

        user.Roles = [userRole];
        role.Users = [userRole];
        role.App = new App
        {
            Id = appId,
            Name = "App",
            Domain = "app.local",
        };

        return user;
    }

    internal static User WithoutPrivileges() =>
        new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };
}





