using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Team2Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("Team2Project")
        {

        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Courses> Courses { get; set; }

        public DbSet<Attendance> Attendance { get; set; }

        public DbSet<CourseTimetable> CourseTimetables { get; set; }
    }
}