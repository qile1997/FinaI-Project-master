using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team2Project.Models
{
    public class CourseTimetable
    {
        public int CourseTimetableId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int CoursesID { get; set; }

        public Courses Courses { get; set; }
    }
}