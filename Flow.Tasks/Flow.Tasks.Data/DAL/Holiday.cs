using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Tasks.Contract.Interface;
using Flow.Tasks.Contract.Message;
using Flow.Tasks.Data.Infrastructure;

namespace Flow.Tasks.Data.DAL
{
    public sealed class Holiday : IHoliday
    {
        /// <summary>
        /// Apply Holiday
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="type"></param>
        /// <param name="holiday">Holiday</param>
        /// <returns>holiday id</returns>
        public int ApplyHoliday(string user, int type, IEnumerable<string> holiday)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var holidayList = holiday as IList<string> ?? holiday.ToList();
                var h = new Core.Holiday {Status = HolidayStatus.S.ToString(), HolidayTypeId  = type, User = user, Year = GetYearFromDate(holidayList.FirstOrDefault()), Dates = String.Join(",", holidayList)};
                uofw.Holidays.Insert(h);

                uofw.Commit();

                return h.HolidayId;
            }
        }

        /// <summary>
        /// Update Holiday
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="holiday">Holiday</param>
        public void UpdateHoliday(string user, int id, HolidayStatus status, IEnumerable<string> holiday)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var hday = uofw.Holidays.Find(h => h.HolidayId == id).FirstOrDefault();
                if (hday == null) return;

                hday.Status = status.ToString();

                if (holiday != null)
                {
                    var holidayList = holiday as IList<string> ?? holiday.ToList();

                    hday.Year = GetYearFromDate(holidayList.FirstOrDefault());

                    if (holidayList.Any())
                    {
                        hday.Dates = String.Join(",", holidayList);
                    }
                }

                uofw.Commit();
            }
        }

        /// <summary>
        /// Remove Holiday
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="id">Id</param>
        public bool RemoveHoliday(string user, int id)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                var holiday = uofw.Holidays.Find(h => h.HolidayId == id).FirstOrDefault();
                if (holiday != null)
                {
                    uofw.Holidays.Delete(holiday);
                    uofw.Commit();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get Holiday For User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="holidayId">HolidayId</param>
        /// <param name="year">Year</param>
        /// <returns>List of holidays</returns>
        public IEnumerable<HolidayInfo> GetHolidayForUser(string user, int holidayId, int year)
        {
            using (var uofw = new FlowTasksUnitOfWork())
            {
                const string publicHoliday = "Public";
                var rejectHoliday = HolidayStatus.R.ToString();
                var holidays = uofw.Holidays.Find(
                    h =>
                        (h.HolidayType.Type == publicHoliday && (holidayId == 0 || h.HolidayId == holidayId) && (year == 0 || h.Year == year)) ||
                        (h.User.Equals(user, StringComparison.OrdinalIgnoreCase) && h.Status != rejectHoliday && 
                        (holidayId == 0 || h.HolidayId == holidayId) && (year == 0 || h.Year == year)), h => h.HolidayType).ToList();

                return
                    holidays.Select(
                        h =>
                            new HolidayInfo
                            {
                                Holiday = h.Dates.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries),
                                Status = (HolidayStatus)Enum.Parse(typeof(HolidayStatus), h.Status),
                                Type = h.HolidayType.Type
                            });
            }

        }

        /// <summary>
        /// Get Year From Date
        /// </summary>
        /// <param name="date">Date: 20/01/2014</param>
        /// <returns>Year: 2014</returns>
        private int GetYearFromDate(string date)
        {
            DateTime holiday;
            if (DateTime.TryParse(date, out holiday))
            {
                return holiday.Year;
            }

            return DateTime.Now.Year;

            //var splitDate = date.Split(new [] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            //if (splitDate.Length < 3) return DateTime.Now.Year;

            //int year;
            //return !int.TryParse(splitDate[2], out year) ? DateTime.Now.Year : year;
        }
    }
}
