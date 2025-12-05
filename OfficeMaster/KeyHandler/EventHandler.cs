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

        public static int GetMaxSessionId(string todayDate, int eventId) // overrided 1
		{
			try
			{
				var timeTracker = DbHelper.timeTracker.Where(x => x.date == todayDate).Where(y => y.eventId == eventId).ToList();

				return (timeTracker.Any()) ? timeTracker.Max(x => x.sessionId) + 1 : 1;

			}
			catch (Exception e)
			{
				CustomPopup.ShowResult($"Exception Occured at GetMaxSessionId1 : {e.Message}");
				return -1;
			}

		}

		public static int GetMaxSessionId(List<TimeTracker> timeTrackers) // overrided 2
		{
			try
			{
				return timeTrackers.Max(x => x.sessionId);
			}
			catch (Exception e)
			{
                CustomPopup.ShowResult($"Exception Occured at GetMaxSessionId2 : {e.Message}");
                return -1; 
			}	
		}

		public static void PushAggregatedData(string todayDate)
		{
			try
			{
				var timeTracker = DbHelper.timeTracker.Where(x => x.date == todayDate).ToList();

				var officeHours = CalculatedWorkHours(timeTracker, Events.WorkStart, Events.WorkEnd);

				var breakHours = CalculatedWorkHours(timeTracker, Events.BreakStart, Events.BreakEnd);

				var workHours = officeHours - breakHours;

				var compensationHours = NegativeTimeSpamHandler(workHours - TimeSpan.Parse("09:00"));

				var oldAggregatorData = DbHelper.timeAggregator.FirstOrDefault(x => x.date == todayDate);

				if (oldAggregatorData == null)
				{
					DbHelper.Add(TimeAggregator.Transform(todayDate, workHours, breakHours, officeHours, compensationHours));
				}
				else
				{
					oldAggregatorData.officeHours = officeHours.ToString("hh\\:mm");
					oldAggregatorData.breakHours = breakHours.ToString("hh\\:mm");
					oldAggregatorData.workHours = workHours.ToString("hh\\:mm");
					oldAggregatorData.compensationHours = compensationHours;
				}

			}
			catch (Exception e)
            {
                CustomPopup.ShowResult($"Exception Occured at PushAggregatedData : {e.Message}");
            }

        }

		public static TimeSpan CalculatedWorkHours(List<TimeTracker> timeTracker, Events sEvent, Events eEvent)
		{
			try
			{
				int maxSessionId = GetMaxSessionId(timeTracker);

				TimeSpan totalTime = TimeSpan.Zero;

				for (var i = 1; i <= maxSessionId; i++)
				{
					var startTime = timeTracker.Where(x => x.sessionId == i).Where(y => y.eventId == (int)sEvent).FirstOrDefault();

					var endTime = timeTracker.Where(x => x.sessionId == i).Where(y => y.eventId == (int)eEvent).FirstOrDefault();

					if (startTime == null && endTime == null) { totalTime += (DateTime.ParseExact("00:00", "HH:mm", null) - DateTime.ParseExact("00:00", "HH:mm", null)); }

					else { totalTime += (DateTime.ParseExact(endTime.time, "HH:mm", null) - DateTime.ParseExact(startTime.time, "HH:mm", null)); }
				}
				return totalTime;

			}
			catch(Exception e) 
			{
                CustomPopup.ShowResult($"Exception Occured at CalculatedWorkHours : {e.Message}");
				return TimeSpan.MaxValue;

            }

        }

		public static bool PushEventData(string eve, Events type)
		{
			try
			{
				DbHelper.Add(TimeTracker.Transform(eve, (int)type));
				DbHelper.SaveChanges();
				if (type == Events.WorkEnd)
				{
					PushAggregatedData(DateTime.Now.ToString("dd/MM/yyyy"));
                    DbHelper.SaveChanges();
                }
				return true;
			}
			catch (Exception e) 
			{
                CustomPopup.ShowResult($"Exception Occured at PushEventData : {e.Message}");
				return false;

            }
        }

		public static string FetchAggregatedData()
		{
			try
			{
				var startMonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

				var endMonthDate = startMonthDate.AddMonths(1).AddDays(-1);

				//var monthlyData = DbHelper.timeAggregator.Where(x => (DateTime.ParseExact(x.date, "dd/MM/yyyy", null) >= startMonthDate && DateTime.ParseExact(x.date, "dd/MM/yyyy", null) <= endMonthDate)).ToList();

				var monthlyData = DbHelper.timeAggregator
					.AsEnumerable() // switch to LINQ-to-Objects
					.Where(x =>
					{
						var dt = DateTime.ParseExact(x.date, "dd/MM/yyyy", null);
						return dt >= startMonthDate && dt <= endMonthDate;
					})
					.ToList();

				if (monthlyData != null)
				{
					return CalculateAllAggregatedHours(monthlyData);
				}
				return "No Data in Aggregated Record to show result";

			}
			catch(Exception e)
			{
                CustomPopup.ShowResult($"Exception Occured at FetchAggregatedData : {e.Message}");
				return null;

            }
        }

		public static string CalculateAllAggregatedHours(List<TimeAggregator> monthlyData)
		{
			try
			{
				var officeHours = TimeSpan.Zero;
				var workHours = TimeSpan.Zero;
				var breakHours = TimeSpan.Zero;
				var compensationHours = TimeSpan.Zero;

				foreach (var dailyData in monthlyData) 
				{
					officeHours += TimeSpan.ParseExact(dailyData.officeHours, @"hh\:mm", null);
					workHours += TimeSpan.ParseExact(dailyData.workHours, @"hh\:mm", null); 
					breakHours += TimeSpan.ParseExact(dailyData.breakHours, @"hh\:mm", null);
					compensationHours += TimeSpan.Parse(dailyData.compensationHours); // Possiblity of having negative values
				}
				return $"Total Days Available : {monthlyData.Count}" +
					$"\nTotal Office Hours : {officeHours}" +
					$"\nTotal Work Hours : {workHours}" +
					$"\nTotal Break Hours : {breakHours}" +
					$"\nCompensate Hours : {compensationHours}";

			}
			catch (Exception e)
			{
                CustomPopup.ShowResult($"Exception Occured at CalculateAllAggregatedHours : {e.Message}");
				return null;
            }
        }

		public static void ClearDB()
		{
			try
			{
				DbHelper.timeTracker.ExecuteDelete();
				DbHelper.timeAggregator.ExecuteDelete();
			}
			catch (Exception e) 
			{
                CustomPopup.ShowResult($"Exception Occured at ClearDb : {e.Message}");
            }
        }

        public static string NegativeTimeSpamHandler(TimeSpan ts)
        {
			try
			{
				string sign = ts < TimeSpan.Zero ? "-" : "+";
				ts = ts.Duration(); // get absolute value
				return $"{sign}{ts:hh\\:mm}";

			}
			catch (Exception e)
			{
                CustomPopup.ShowResult($"Exception Occured at NegativeTimeHandler : {e.Message}");
				return null;
            }
        }

    }
}


