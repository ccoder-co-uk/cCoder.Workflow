// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace Workflow.Web.Brokers;

internal interface ICoreUserBroker
{
    User SelectUser();
}