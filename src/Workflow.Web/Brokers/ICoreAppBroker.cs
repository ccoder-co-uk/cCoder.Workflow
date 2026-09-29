// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Data.Models.CMS;

namespace Workflow.Web.Brokers;

internal interface ICoreAppBroker
{
    ValueTask<App> SelectAppAsync(int appId);
}