using System.Collections.Generic;
using Flow.Tasks.Contract.Message;

namespace Flow.Tasks.Contract.Interface
{
    public interface IHoliday
    {
        int ApplyHoliday(string user, int type, IEnumerable<string> holiday);
        IEnumerable<HolidayInfo> GetHolidayForUser(string user, int holidayId, int year);
        void UpdateHoliday(string user, int id, HolidayStatus status, IEnumerable<string> holiday);
        bool RemoveHoliday(string user, int holidayId);
    }
}