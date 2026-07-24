// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace Workflow.Web.Services.Processings;

internal sealed partial class CoreUserProcessingService(
    ISecurityDbContextFactory securityDbContextFactory)
    : ICoreUserProcessingService
{
    public User GetUser() =>
        TryCatch(operation: () =>
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
        });
}