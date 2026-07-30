// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace Workflow.Web.Exposures;

public interface ICoreUserManager
{
    User GetUser();
}