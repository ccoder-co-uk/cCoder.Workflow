// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Workflow.Services.Processings;

public interface IBaselineProcessingService
{
    Package[] GetBaselinePackages();
}