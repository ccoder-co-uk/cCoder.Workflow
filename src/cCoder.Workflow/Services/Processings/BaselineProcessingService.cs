// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Workflow.Exposures.Setup;

namespace cCoder.Workflow.Services.Processings;

internal sealed partial class BaselineProcessingService : IBaselineProcessingService
{
    public Package[] GetBaselinePackages() =>
        TryCatch(operation: () =>
        {
            ValidateInputs(inputs: []);
            return UIBaseline.Packages;
        });
}