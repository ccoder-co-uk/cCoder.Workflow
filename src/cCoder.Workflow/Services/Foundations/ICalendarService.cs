using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Planning;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations;

public interface ICalendarService
{
    Calendar Get(int id);
    IQueryable<Calendar> GetAll(bool ignoreFilters = false);
    ValueTask<Calendar> AddAsync(Calendar calendar);
    ValueTask<Calendar> UpdateAsync(Calendar calendar);
    ValueTask DeleteAsync(int id);
}










