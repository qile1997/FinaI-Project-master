using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team2Project.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public string Name { get; set; }
        public Programme? Programme { get; set; }
        public int CourseTimetableId { get; set; }
        public DateTime AttendanceTime { get; set; }
    }
}