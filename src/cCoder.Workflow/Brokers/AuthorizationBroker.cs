// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Brokers;

internal sealed class AuthorizationBroker(
    IAuthorizationDependency authorizationDependency)
    : IAuthorizationBroker
{
    public User GetCurrentUser() =>
        authorizationDependency.GetCurrentUser();

    public User GetUser(string userId) =>
        authorizationDependency.GetUser(userId: userId);

    public bool IsAdminOfApp(int? appId) =>
        authorizationDependency.IsAdminOfApp(appId: appId);

    public bool IsAdminOfApp(int appId, string userName) =>
        authorizationDependency.IsAdminOfApp(
            appId: appId,
            userName: userName);

    public void Authorize(int? appId, string privilege) =>
        authorizationDependency.Authorize(
            appId: appId,
            privilege: privilege);

    public void Authorize(
        string userId,
        int? appId,
        string privilege) =>
        authorizationDependency.Authorize(
            userId: userId,
            appId: appId,
            privilege: privilege);

    public bool UserBelongsToApp(string userId, int? appId) =>
        authorizationDependency.UserBelongsToApp(
            userId: userId,
            appId: appId);
}