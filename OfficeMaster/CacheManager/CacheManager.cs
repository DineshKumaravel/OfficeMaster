using OfficeHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeHealper
{
	public class CacheManager
	{
		private static List<TimeTracker> timeTracker;
		private static List<TimeAggregator> timeAggregator;

		static CacheManager()
		{
			timeTracker = new List<TimeTracker>();
			timeAggregator = new List<TimeAggregator>();
		}

		//public static void PutObject<T>(T obj)
		//{
		//	if (typeof(T) == typeof(TimeTracker))
		//	{
		//		timeTracker.Add((TimeTracker)(object)obj);

		//	}
		//	else
		//	{
		//		timeAggregator.Add((TimeAggregator)(object)obj);

		//	}
		//}
        public static void PutObject<T>(List<T> objs)
        {
			try
			{
				if (typeof(T) == typeof(TimeTracker))
				{
					foreach (var obj in objs)
					{
						timeTracker.Add((TimeTracker)(object)obj);
					}
				}
				else
				{
					foreach (var obj in objs)
					{
						timeAggregator.Add((TimeAggregator)(object)obj);
					}
				}
			}
			catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at cache manager"); }
        }
		public static List<T> GetObject<T>()
		{
			try
			{
				return (typeof(T) == typeof(TimeTracker)) ? timeTracker as List<T> : timeAggregator as List<T>;
			}
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at cache manager"); return new List<T>(); }
        }

		public static bool IsCacheEmpty<T>()
		{
			try
			{
				return (typeof(T) == typeof(TimeTracker)) ? !timeTracker.Any() : !timeAggregator.Any();
			}
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at cache manager"); return false; }

        }

		public static void ClearCache<T>()
		{
			try
			{
				if (typeof(T) == typeof(TimeTracker)) timeTracker.Clear();
				else timeAggregator.Clear();
			}
            catch (Exception ex) { ErrorHandler.RecordError(ex.Message + "error at cache manager"); }
        }
       
    }
}

