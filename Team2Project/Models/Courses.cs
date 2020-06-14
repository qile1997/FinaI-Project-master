using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team2Project.Models
{
    public class Courses
    {
        // For admin
        public int CoursesID { get; set; }

        public Programme Programme { get; set; }

        public string CourseName { get; set; }
        
        public ICollection<CourseTimetable> CourseTimetables { get; set; }
    }
}