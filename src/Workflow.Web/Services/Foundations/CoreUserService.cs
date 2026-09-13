// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Workflow.Web.Brokers;

namespace Workflow.Web.Services.Foundations;

internal sealed partial class CoreUserService(
    ICoreUserBroker coreUserBroker)
    : ICoreUserService
{
    public User GetUser() =>
        TryCatch(operation: () =>
        {
            return coreUserBroker.SelectUser();
        });
}