using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SQLite;
using System.IO;

namespace OfficeHelper
{
	public class DbHelper : DbContext
	{
        public DbSet<TimeTracker> timeTracker { get; set; }

        public DbSet<TimeAggregator> timeAggregator { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Client_Details.db");
        }

    }

}
