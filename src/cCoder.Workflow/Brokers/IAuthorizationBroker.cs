// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.Workflow.Brokers;

public interface IAuthorizationBroker
{
    User GetCurrentUser();

    User GetUser(string userId);

    bool IsAdminOfApp(int? appId);

    bool IsAdminOfApp(int appId, string userName);

    bool IsAuthorized(int? appId, string privilege);

    bool IsAuthorized(string userId, int? appId, string privilege);

    bool UserBelongsToApp(string userId, int? appId);
}