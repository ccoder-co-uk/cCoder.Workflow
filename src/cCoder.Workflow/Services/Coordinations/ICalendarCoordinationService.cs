using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Coordinations;

public interface ICalendarCoordinationService
{
    ValueTask HandleCalendarAddAsync(Calendar calendar);

    ValueTask HandleCalendarUpdateAsync(Calendar calendar);

    ValueTask HandleCalendarDeleteAsync(Calendar calendar);
}










