using cCoder.Data;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Web.AcceptanceTests.Infrastructure;

internal sealed class AcceptanceApplicationSeeder(IServiceProvider services)
{
    private const int AppId = 1;
    private const string AppDomain = "localhost";
    private const string AcceptanceAdminRoleName = "Acceptance Administrators";
    private const string AcceptanceAdminPrivileges =
        "app_admin,"
        + "file_create,file_read,file_update,file_delete,"
        + "filecontent_create,filecontent_read,filecontent_update,filecontent_delete,"
        + "folder_create,folder_read,folder_update,folder_delete,"
        + "folderrole_create,folderrole_read,folderrole_update,folderrole_delete";

    public async Task SeedAsync()
    {
        using IServiceScope scope = services.CreateScope();
        using DbContext core = scope.ServiceProvider
            .GetRequiredService<cCoder.Data.ICoreContextFactory>()
            .CreateCoreContext();

        await EnsureAppAsync(core);
        await EnsureGuestUserAsync(core);
        await EnsureGuestAdminAsync(core);
    }

    private static async Task EnsureAppAsync(DbContext core)
    {
        if (await core.Set<App>().AnyAsync(app => app.Id == AppId))
            return;

        core.Add(new App
        {
            Name = "Acceptance",
            Domain = AppDomain,
            DefaultTheme = "Default",
            DefaultCultureId = string.Empty,
            TenantId = "acceptance",
            ConfigJson = "{}",
        });

        await core.SaveChangesAsync();
    }

    private static async Task EnsureGuestUserAsync(DbContext core)
    {
        if (await core.Set<User>().AnyAsync(existing => existing.Id == "Guest"))
            return;

        core.Add(new User
        {
            Id = "Guest",
            DefaultCultureId = string.Empty,
            DisplayName = "Guest",
            Email = string.Empty,
            IsActive = true,
        });

        await core.SaveChangesAsync();
    }

    private static async Task EnsureGuestAdminAsync(DbContext core)
    {
        Role role = await core.Set<Role>().FirstOrDefaultAsync(existing =>
            existing.AppId == AppId && existing.Name == AcceptanceAdminRoleName);

        if (role is null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                AppId = AppId,
                Name = AcceptanceAdminRoleName,
                Description = "Acceptance bootstrap role",
                Privs = AcceptanceAdminPrivileges,
            };

            core.Add(role);
            await core.SaveChangesAsync();
        }
        else if (role.Privs != AcceptanceAdminPrivileges)
        {
            role.Privs = AcceptanceAdminPrivileges;
            await core.SaveChangesAsync();
        }

        bool hasGuestRole = await core.Set<UserRole>().AnyAsync(existing =>
            existing.RoleId == role.Id && existing.UserId == "Guest");

        if (!hasGuestRole)
        {
            core.Add(new UserRole { RoleId = role.Id, UserId = "Guest" });
            await core.SaveChangesAsync();
        }
    }
}


