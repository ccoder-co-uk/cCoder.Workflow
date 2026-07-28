// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.Data.Models.Security;

namespace cCoder.Workflow.Extensions;

internal static class AuthorizationExtensions
{
    internal static void Authorize(
        this User user,
        int? appId,
        string privilege)
    {
        if (user == null
            || !(user.HasAppAdminPrivilege(appId: appId)
                || user.HasPrivilege(
                    appId: appId,
                    privilege: privilege)))
        {
            throw new SecurityException(message: "Access Denied!");
        }
    }

    internal static void Authorize(
        this User user,
        string userId,
        int? appId,
        string privilege)
    {
        if (string.IsNullOrWhiteSpace(value: userId))
        {
            throw new SecurityException(message: "Access Denied!");
        }

        user.Authorize(
            appId: appId,
            privilege: privilege);
    }

    internal static bool HasAppAdminPrivilege(
        this User user,
        int? appId) =>
        appId.HasValue
        && (user.Roles?.Any(predicate: role =>
            role.Role?.AppId == appId.Value
            && (role.Role?.Privileges?.Contains(item: "app_admin") ?? false))
            ?? false);

    private static bool HasPrivilege(
        this User user,
        int? appId,
        string privilege)
    {
        string normalizedPrivilege =
            privilege?.ToLowerInvariant() ?? string.Empty;

        return user.HasAppAdminPrivilege(appId: appId)
            || (user.Roles?.Any(predicate: role =>
                (appId == null || role.Role?.AppId == appId)
                && (role.Role?.Privileges?.Contains(
                    item: normalizedPrivilege) ?? false))
                ?? false);
    }
}