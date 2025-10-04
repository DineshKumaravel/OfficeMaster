using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeHelper
{
	public class TimeAggregator
	{

		public enum Events
		{
			WorkStart = 0,
			WorkEnd,
			BreakStart,
			BreakEnd,
			CalculatedHours
		}

		//public enum MontlyAggregate
		//{
		//	OfficeHours = 4,
		//	WorkHours,
		//	BreakHours,
		//	CompensateHours
		//}

        [Key]             
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int id { get; set; }

		public virtual string date { get; set; }

		public virtual string workHours { get; set; }

		public virtual string breakHours { get; set; }

		public virtual string officeHours { get; set; }

		public virtual string compensationHours { get; set; }

		public static TimeAggregator Transform(string todayDate, TimeSpan workHours, TimeSpan breakHours, TimeSpan officeHours, string compensationHours )
		{
			return new TimeAggregator()
			{
				date = todayDate,
				workHours = workHours.ToString("hh\\:mm"),
				breakHours = breakHours.ToString("hh\\:mm"),
				officeHours = officeHours.ToString("hh\\:mm"),
				compensationHours = compensationHours
			};
		}
    }
}
