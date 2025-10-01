using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace OfficeHelper
{
	public class TimeTracker
	{
        [Key]               
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int id { get; set; }

		public virtual string date { get; set; }

		public virtual string time { get; set; }

		public virtual string event_ { get; set; }

        public virtual int eventId { get; set; }

		public virtual int sessionId { get; set; }

		public static TimeTracker Transform(string eve, int eve_id)
		{
			return new TimeTracker()
			{
				date = DateTime.Now.ToString("dd/MM/yyyy"),
				time = DateTime.Now.ToString("HH:mm"),
				eventId = eve_id,
				event_ = eve,
				sessionId = EventHandler.GetMaxSessionId(DateTime.Now.ToString("dd/MM/yyyy"), eve_id) 
			};
		}
    }

}
