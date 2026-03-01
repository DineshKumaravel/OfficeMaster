using Microsoft.EntityFrameworkCore;
using OfficeHealper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static OfficeHelper.TimeAggregator;

namespace OfficeHelper
{
	public class EventHandler
	{

		private static DbHelper DbHelper = new DbHelper();

        public static bool CreateDB()
        {
			try
			{
				bool dbStatus = DbHelper.Database.EnsureCreated();
				return dbStatus;
			}
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at createdb"); return false; }
        }

		public static bool CreateCloudRecords()
		{
			try
			{
				if(!GoogleSpreedSheetAPI.CheckSheetExistance())
				{
					return GoogleSpreedSheetAPI.CreateNewSheet();
				}
				else
				{
					return true;
				}
			}
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at createcloudrecord"); return false; }
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
			ErrorHandler.RecordError(e.Message + "error at max session id 1"); 
            return -1;
			}

		}

		private static int GetMaxSessionId(List<TimeTracker> timeTrackers) // overrided 2
		{
			try
			{
				return timeTrackers.Max(x => x.sessionId);
			}
			catch (Exception e)
			{
                ErrorHandler.RecordError(e.Message + "error at max session id 2");
                return -1; 
			}	
		}

		private static void PushAggregatedData(string todayDate, Events eventType)
		{
			try
			{
				var timeTracker = DbHelper.timeTracker.Where(x => x.date == todayDate).ToList();

				var officeHours = (eventType == Events.HalfDayEnd) ? CalculatedWorkHours(timeTracker, Events.WorkStart, Events.HalfDayEnd)  : CalculatedWorkHours(timeTracker, Events.WorkStart, Events.WorkEnd);

				var breakHours = CalculatedWorkHours(timeTracker, Events.BreakStart, Events.BreakEnd);

				var workHours = officeHours - breakHours;

				var compensationHours = (eventType == Events.HalfDayEnd) ? NegativeTimeSpamHandler(workHours - TimeSpan.Parse("04:30")) : NegativeTimeSpamHandler(workHours - TimeSpan.Parse("09:00"));

				var oldAggregatorData = DbHelper.timeAggregator.FirstOrDefault(x => x.date == todayDate);

				var timeAggregate = TimeAggregator.Transform(todayDate, workHours, breakHours, officeHours, compensationHours);
				
				if (oldAggregatorData == null)
				{
                    DbHelper.Add(timeAggregate);
				}
				else
				{
					oldAggregatorData.officeHours = officeHours.ToString("hh\\:mm");
					oldAggregatorData.breakHours = breakHours.ToString("hh\\:mm");
					oldAggregatorData.workHours = workHours.ToString("hh\\:mm");
					oldAggregatorData.compensationHours = compensationHours;

				}
				GoogleSpreedSheetAPI.WriteToExcel<TimeAggregator>(new List<TimeAggregator>() { timeAggregate });
				DbHelper.SaveChanges();

			}
			catch (Exception e)
            {
                ErrorHandler.RecordError(e.Message + "error at push aggregated data");
            }

        }

		private static TimeSpan CalculatedWorkHours(List<TimeTracker> timeTracker, Events sEvent, Events eEvent)
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
                ErrorHandler.RecordError(e.Message + "error at calculated work hours");
                return TimeSpan.MaxValue;
            }

        }

		public static bool PushEventData(string eve, Events type)
		{
			try
			{
				var timeTracker = TimeTracker.Transform(eve, (int)type);
				DbHelper.Add(timeTracker);
				GoogleSpreedSheetAPI.WriteToExcel<TimeTracker>(new List<TimeTracker>() { timeTracker});
				DbHelper.SaveChanges();
				if (type == Events.WorkEnd || type == Events.HalfDayEnd)
				{
					PushAggregatedData(DateTime.Now.ToString("dd/MM/yyyy") , type);
                }

				if (!CacheManager.IsCacheEmpty<TimeTracker>()) { TryPushCacheData<TimeTracker>(); }

                if (!CacheManager.IsCacheEmpty<TimeAggregator>()) { TryPushCacheData<TimeAggregator>(); }

                return true;
			}
			catch (Exception e) 
			{
                ErrorHandler.RecordError(e.Message + "error at push event data");
                return false;

            }
        }
		//private static void CheckAndSendEmail()
		//{
		//	if(DbHelper.timeAggregator.ToList().Count % 30 == 0)
		//	{
		//		var last30dayrecord = DbHelper.timeAggregator
		//			.OrderByDescending(x => x.id)
		//			.Take(30)
		//			.OrderBy(x => x.id)
		//			.ToList();

		//		NotificationAPI.SendReportMail(Environment.UserName , "officemasterchartreport", "MonthlyCompensateData.png" , ChartMaker.CreateChart(last30dayrecord));

		//	}
		//}
		private static void TryPushCacheData<T>()
		{
			try
			{
				bool status;
				if (typeof(T) == typeof(TimeTracker))
					status = GoogleSpreedSheetAPI.WriteToExcel<TimeTracker>(CacheManager.GetObject<TimeTracker>(), true);
				else
					status = GoogleSpreedSheetAPI.WriteToExcel<TimeAggregator>(CacheManager.GetObject<TimeAggregator>() , true);

				if (status)
					CacheManager.ClearCache<T>();
			}
			catch(Exception e)
			{
                ErrorHandler.RecordError(e.Message + "error at trypushcachedata");
            }
        }

		public static TimeAggregatorReport FetchAggregatedData(AggregateType aggregateType)
		{
			try
			{
				DateTime startDate, endDate;
				if(aggregateType == AggregateType.Month)
				{
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    endDate = startDate.AddMonths(1).AddDays(-1);
                }
				else
				{
					startDate = DateTime.Today;

                    endDate = startDate.AddDays(1);
                }

				//var monthlyData = DbHelper.timeAggregator.Where(x => (DateTime.ParseExact(x.date, "dd/MM/yyyy", null) >= startMonthDate && DateTime.ParseExact(x.date, "dd/MM/yyyy", null) <= endMonthDate)).ToList();

				var monthlyData = DbHelper.timeAggregator
					.AsEnumerable() // switch to LINQ-to-Objects
					.Where(x =>
					{
						var dt = DateTime.ParseExact(x.date, "dd/MM/yyyy", null);
						return dt >= startDate && dt <= endDate;
					})
					.ToList();

				var todayData = DbHelper.timeTracker
					.AsEnumerable()
					.Where(x =>
					{
                        var dt = DateTime.ParseExact(x.date, "dd/MM/yyyy", null);
                        return dt >= startDate && dt <= endDate;
                    })
					.ToList();

				if (monthlyData != null || todayData != null)
				{
					if (aggregateType == AggregateType.Month)
						return CalculateAllAggregatedHours(monthlyData);
					else
					{
						var checkWorkStart = todayData.Where(x => x.eventId == (int)Events.WorkStart).ToList();

                        var checkWorkEnd = todayData.Where(x => x.eventId == (int)Events.WorkEnd).ToList();

						if(checkWorkEnd.Count != checkWorkStart.Count)	todayData.Add(TimeTracker.Transform("WorkEnd" ,(int)Events.WorkEnd));

                        var officeHours = CalculatedWorkHours(todayData, Events.WorkStart, Events.WorkEnd);

                        var breakHours = CalculatedWorkHours(todayData, Events.BreakStart, Events.BreakEnd);

                        var workHours = officeHours - breakHours;

                        var compensationHours = NegativeTimeSpamHandler(workHours - TimeSpan.Parse("09:00"));

                        return CalculateAllAggregatedHours(new List<TimeAggregator>() { TimeAggregator.Transform(DateTime.Now.ToString("dd/MM/yyyy"), workHours, breakHours, officeHours, compensationHours) });
					}
                }
				//return "No Data in Aggregated Record to show result";
				return null;

			}
			catch(Exception e)
			{
                //return $"Exception Occured at FetchAggregatedData : {e.Message}";
                ErrorHandler.RecordError(e.Message + "error at fetch aggregated data");
                return null;
            }
        }

		public static TimeAggregatorReport CalculateAllAggregatedHours(List<TimeAggregator> monthlyData)
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
					compensationHours += TimeSpan.Parse(PositiveTimeSpanHandler(dailyData.compensationHours)); // Possiblity of having negative values
				}
				return new TimeAggregatorReport()
				{
					totalDays = monthlyData.Count,
					officeHours = TimeSpanOutputFormatter(officeHours),
					breakHours = TimeSpanOutputFormatter(breakHours),
					compensateHours = TimeSpanOutputFormatter(compensationHours),
					workHours = TimeSpanOutputFormatter(workHours),
				};
				//return $"Total Days Available : {monthlyData.Count}" +
				//	$"\nTotal Office Hours : {TimeSpanOutputFormatter(officeHours)}" +
				//	$"\nTotal Work Hours : {TimeSpanOutputFormatter(workHours)}" +
				//	$"\nTotal Break Hours : {TimeSpanOutputFormatter(breakHours)}" +
				//	$"\nCompensate Hours : {TimeSpanOutputFormatter(compensationHours)}";

			}
			catch (Exception e)
			{
                //return ($"Exception Occured at CalculateAllAggregatedHours : {e.Message}");
                ErrorHandler.RecordError(e.Message + "error at calculated hours");
                return new TimeAggregatorReport();
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
                //CustomPopup.ShowResult($"Exception Occured at ClearDb : {e.Message}");
            }
        }

		public static void ClearSpecificDateDbRecord()
		{
			try
			{
				string targetDate = DateTime.Now.ToString("dd/MM/yyyy");


				var records = DbHelper.timeTracker
					.Where(o => o.date.StartsWith(targetDate))
					.ToList();

				DbHelper.timeTracker.RemoveRange(records);
				DbHelper.SaveChanges();

			}
			catch (Exception e) { ErrorHandler.RecordError(e.Message + "error at db clear"); }

        }

        private static string NegativeTimeSpamHandler(TimeSpan ts)
        {
			try
			{
				string sign = ts < TimeSpan.Zero ? "-" : "+";
				ts = ts.Duration(); // get absolute value
				return $"{sign}{ts:hh\\:mm}";

			}
			catch (Exception e)
			{
				return null;
            }
        }
		private static string TimeSpanOutputFormatter(TimeSpan ts)
		{
			return $"{ts.Days} D : {ts.Hours} H : {ts.Minutes} M";
		}
		public static string PositiveTimeSpanHandler(string timespan)
		{
			timespan = timespan.Contains("+") ? timespan.Replace("+", "") : timespan;
			return timespan;
		}

		public static void InitializeButtonProperty(Button workButton = null, Button breakButton = null)
		{
			try
			{
				throw new Exception();
				var startDate = DateTime.Today;

				var endDate = startDate.AddDays(1);

				var todayData = DbHelper.timeTracker
					.AsEnumerable()
					.Where(x =>
					{
						var dt = DateTime.ParseExact(x.date, "dd/MM/yyyy", null);
						return dt >= startDate && dt <= endDate;
					})
					.ToList();

				if (!todayData.Any())
				{
					ButtonStartProp(workButton, breakButton);
					return;
				} 
				if (workButton != null)
				{

					var WorkStart = todayData.Where(x => x.eventId == (int)Events.WorkStart);

					if (!WorkStart.Any()) { ButtonStartProp(workButton, null); return; }

					var maxWorkStartSession = WorkStart.Max(x => x.sessionId);
					//if(maxWorkStartSession == null) { ButtonStartProp(workButton, null); return; }

					var maxWorkEndSession = todayData.Where(x => x.eventId == (int)Events.WorkEnd && x.sessionId == maxWorkStartSession).FirstOrDefault();

					if (maxWorkEndSession == null) //no work end exist for the workstart
					{
						workButton.Text = "Work End";
						workButton.BackColor = Color.FromArgb(255, 102, 102);
                    }
					else
					{
						workButton.Text = "Work Start";
						workButton.BackColor = Color.LightGreen;
					}
				}
				if (breakButton != null)
				{
					var BreakStart = todayData.Where(x => x.eventId == (int)Events.BreakStart);

					if(!BreakStart.Any()) { ButtonStartProp(null , breakButton); return; }	

					var maxBreakStartSession = BreakStart.Max(x => x.sessionId);
					//if (maxBreakStartSession == null) { ButtonStartProp(null, breakButton); return; }

					var maxBreakEndSession = todayData.Where(x => x.eventId == (int)Events.BreakEnd && x.sessionId == maxBreakStartSession).FirstOrDefault();

					if (maxBreakEndSession == null)
					{
						breakButton.Text = "Break End";
						breakButton.BackColor = Color.FromArgb(255, 102, 102);
                    }
					else
					{
						breakButton.Text = "Break Start";
						breakButton.BackColor = Color.LightGreen;
					}

				}
			}
			catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at initialize button prop" + "stacktrace =>" + ex.StackTrace); }


        }
		private static void ButtonStartProp(Button workButton = null,Button breakButton= null)
		{
			if(workButton != null)
			{
				workButton.Text = "Work Start";
				workButton.BackColor = Color.LightGreen;
			}
			if (breakButton != null)
			{
				breakButton.Text = "Break Start";
				breakButton.BackColor = Color.LightGreen;

			}

        }


    }
}


