using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team2Project.Models;

namespace Team2Project.ViewModel
{
    public class TimetableVM
    {
        public int CoursesID { get; set; }

        public int CourseTimetableId { get; set; }

        public Programme Programme { get; set; }

        public string StudentName { get; set; }

        public string CourseName { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string CheckInButton { get; set; }

        public string AttendanceTime { get; set; }

        public int StudentID { get; set; }

        public string Msg { get; set; }
    }
}