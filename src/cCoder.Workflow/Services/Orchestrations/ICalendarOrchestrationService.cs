using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Orchestrations;

public interface ICalendarOrchestrationService
{
    Calendar Get(int id);

    IQueryable<Calendar> GetAll(bool ignoreFilters = false);

    ValueTask<Calendar> AddAsync(Calendar entity);

    ValueTask<Calendar> UpdateAsync(Calendar entity);

    ValueTask DeleteAsync(int id);
    ValueTask DeleteByAppIdAsync(int appId);

    ValueTask<IEnumerable<Result<Calendar>>> AddOrUpdate(IEnumerable<Calendar> items);

    ValueTask DeleteAllAsync(IEnumerable<Calendar> items);
}
