using cCoder.Data.Models.Planning;


namespace cCoder.Workflow.Brokers.Storage;

public interface ICalendarBroker
{
    IQueryable<Calendar> GetAllCalendars(bool ignoreFilters);
    ValueTask<Calendar> AddCalendarAsync(Calendar entity);
    ValueTask<Calendar> UpdateCalendarAsync(Calendar entity);
    ValueTask<int> DeleteCalendarAsync(Calendar entity);
    ValueTask DeleteAllCalendarsAsync(IEnumerable<Calendar> items);
    int? GetAppId(Calendar entity);
}







