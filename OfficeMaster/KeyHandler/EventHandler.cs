using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.Design;
using static OfficeHelper.TimeAggregator;

namespace OfficeHelper
{
	public class EventHandler
	{

		public static DbHelper DbHelper = new DbHelper();

        public static bool CreateDB()
        {
			bool dbStatus = DbHelper.Database.EnsureCreated();
			return dbStatus;
        }

        public static int GetMaxSessionId(string todayDate, int eventId)
		{
			var timeTracker = DbHelper.timeTracker.Where(x => x.date == todayDate).Where(y => y.eventId == eventId).ToList();

			return (timeTracker.Any()) ? timeTracker.Max(x => x.sessionId) + 1 : 1;

		}

		public static int GetMaxSessionId(List<TimeTracker> timeTrackers)
		{
			return timeTrackers.Max(x => x.sessionId);
		}

		public static void PushAggregatedData(string todayDate)
		{
			var timeTracker = DbHelper.timeTracker.Where(x => x.date == todayDate).ToList();

			var officeHours = CalculatedWorkHours(timeTracker, Events.WorkStart , Events.WorkEnd);

			var breakHours = CalculatedWorkHours(timeTracker, Events.BreakStart , Events.BreakEnd);

			var workHours = officeHours - breakHours;

			var compensationHours = NegativeTimeSpamHandler(workHours - TimeSpan.Parse("09:00"));

			var oldAggregatorData = DbHelper.timeAggregator.Where(x => x.date == todayDate).First();

			if(oldAggregatorData == null)
			{
				DbHelper.Add(TimeAggregator.Transform(todayDate, workHours, breakHours, officeHours, compensationHours));
			}
			else
			{
				oldAggregatorData.officeHours = officeHours.ToString("hh\\:mm");
				oldAggregatorData.breakHours = breakHours.ToString("hh\\:mm");
				oldAggregatorData.workHours = workHours.ToString("hh\\:mm");
				oldAggregatorData.compensationHours = compensationHours;

				DbHelper.SaveChanges();
			}

		}

		public static TimeSpan CalculatedWorkHours(List<TimeTracker> timeTracker, Events sEvent, Events eEvent)
		{
			int maxSessionId = GetMaxSessionId(timeTracker);

			TimeSpan totalTime = TimeSpan.Zero;

			for (var i = 1; i <= maxSessionId; i++)
			{
				var startTime = timeTracker.Where(x => x.sessionId == i).Where(y => y.eventId == (int)sEvent).FirstOrDefault();

				var endTime = timeTracker.Where(x => x.sessionId == i).Where(y => y.eventId == (int)eEvent).FirstOrDefault();

				totalTime += (DateTime.ParseExact(endTime.time, "HH:mm", null) - DateTime.ParseExact(startTime.time, "HH:mm", null));
			}
			return totalTime;

		}

		public static bool PushEventData(string eve, Events type)
		{
			DbHelper.Add(TimeTracker.Transform(eve, (int)type));
            DbHelper.SaveChanges();
            if (type == Events.WorkEnd)
			{
				PushAggregatedData(DateTime.Now.ToString("dd/MM/yyyy"));
			}
			return true;	
		}

		public static void ClearDB()
		{
			DbHelper.timeTracker.ExecuteDelete();
		}

        public static string NegativeTimeSpamHandler(TimeSpan ts)
        {
            string sign = ts < TimeSpan.Zero ? "-" : "+";
            ts = ts.Duration(); // get absolute value
            return $"{sign}{ts:hh\\:mm}";
        }

    }
}


