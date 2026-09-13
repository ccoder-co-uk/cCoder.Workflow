// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models.Entities;

namespace Workflow.Web.Brokers;

internal sealed class CoreUserBroker(
    ISecurityDbContextFactory securityDbContextFactory)
    : ICoreUserBroker
{
    public User SelectUser()
    {
        using SecurityDbContext securityDbContext =
            securityDbContextFactory.CreateDbContext();

        SSOUser user = securityDbContext.GetCurrentUser();

        return new User
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            DefaultCultureId = string.Empty,
            IsActive = true
        };
    }
}