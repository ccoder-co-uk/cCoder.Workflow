// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web.AcceptanceTests.Infrastructure;

internal static class AcceptanceUserSeeder
{
    public static async Task EnsureCoreUserAsync(DbContext core, string id, string displayName, string email)
    {
        if (await core.Set<User>()
            .IgnoreQueryFilters()
            .AnyAsync(predicate: user => user.Id == id))
        {
            return;
        }

        core.Add(entity: new User
        {
            Id = id,
            DefaultCultureId = string.Empty,
            DisplayName = displayName,
            Email = email,
            IsActive = true,
        });

        await core.SaveChangesAsync();
    }

    public static async Task EnsureSsoUserAsync(DbContext sso, string id, string displayName, string email)
    {
        if (await sso.Set<SSOUser>()
            .IgnoreQueryFilters()
            .AnyAsync(predicate: user => user.Id == id))
        {
            return;
        }

        sso.Add(entity: new SSOUser
        {
            Id = id,
            DisplayName = displayName,
            Email = email,
            EmailConfirmed = true,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            PhoneNumberConfirmed = false,
        });

        await sso.SaveChangesAsync();
    }
}