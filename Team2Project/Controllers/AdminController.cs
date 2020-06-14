using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team2Project.Models;
using Team2Project.ViewModel;

namespace Team2Project.Controllers
{
    public class AdminController : Controller
    {
        private AppDbContext db = new AppDbContext();


        //Create View For Attendance 
        public ActionResult AttendanceList()
        {
            db.Attendance.ToList();
            return View();
        }

        public ActionResult Profile()
        {
            var AID = Convert.ToInt32(Session["Admin"]);

            var user = db.Users.SingleOrDefault(c => c.UsersID == AID);

            if (user != null)
            {
                return View(user);
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "ID , Username , Password ,Programme,Email, Roles")] Users user)
        {
            if (ModelState.IsValid)
            {
                user.Roles = Roles.Student;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("AttendanceList");
            }
            return View();
        }

        public ActionResult AjaxCheckEmail(string Email)
        {
            // check user after you type the email
            var CheckDuplicate = db.Users.Where(c => c.Email == Email).SingleOrDefault();

            if (CheckDuplicate != null)
            {
                var emailDuplicate = "Email is not available. Please try another email!";
                return Json(new { Error = true, emailDuplicate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var emailNoDuplicate = "Email is available.";
                return Json(new { Error = false, emailNoDuplicate }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SelectProgramme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SelectProgramme(string selectedProgramme)
        {
            List<TimetableVM> AllCourseName = new List<TimetableVM>();
            var Getdata = db.Courses.ToList().Where(c => c.Programme == (Programme)Enum.Parse(typeof(Programme), selectedProgramme));
            foreach (var item in Getdata)
            {
                var check = AllCourseName.SingleOrDefault(a => a.CourseName == item.CourseName);
                if (check == null)
                {
                    TimetableVM AddCourseName = new TimetableVM
                    {
                        CoursesID = item.CoursesID,
                        CourseName = item.CourseName,
                    };
                    AllCourseName.Add(AddCourseName);
                }
            }
            return Json(new { AllCourseName, data = true }, JsonRequestBehavior.AllowGet);

            //Session["SelectedProgramme"] = selected.Programmes.ToString();
            //return RedirectToAction("CourseIndex");
        }

        //Will add one ajax action for select course

        //Will Change to Ajax to show all time for the course
        [HttpPost]
        public ActionResult CourseIndex(string SelectedOption, int CourseId, string selectedProgramme, string StudentName)
        {
            List<TimetableVM> getData = new List<TimetableVM>();
            if (SelectedOption == "Course" && StudentName == "")
            {
                /* string programme = Session["SelectedProgramme"].ToString()*/
                //var courseTimetable = db.Courses.ToList().Where(c => c.Programme == (Programme)Enum.Parse(typeof(Programme), selectedProgramme) && c.CourseName == selectedCourse);
                var programme = (Programme)Enum.Parse(typeof(Programme), selectedProgramme);
                var getCourse = db.Courses.SingleOrDefault(c => c.CoursesID == CourseId && c.Programme == programme);
                var getTime = db.CourseTimetables.Where(c => c.CoursesID == getCourse.CoursesID);
                int TotalCourseTime = 0;

                if (getTime.Count() == 0)
                {
                    string Error = "This course haven't create a timetable";
                    return Json(new { Error, time = false }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in getTime)
                {

                    TimetableVM AddTimetable = new TimetableVM
                    {
                        CoursesID = item.CoursesID,
                        CourseTimetableId = item.CourseTimetableId,
                        CourseName = getCourse.CourseName,
                        Programme = getCourse.Programme,
                        StartTime = item.StartTime.ToString(),
                        EndTime = item.EndTime.ToString()
                    };
                    getData.Add(AddTimetable);
                }
                TotalCourseTime = getTime.Count();
                return Json(new { TotalCourseTime, getData, time = true }, JsonRequestBehavior.AllowGet);
            }
            else if (SelectedOption == "Student" && StudentName == "")
            {
                var programme = (Programme)Enum.Parse(typeof(Programme), selectedProgramme);
                var getCourse = db.Users.Where(c => c.Programme == programme);
                int TotalStudent = getCourse.Count();

                if (getCourse.Count() == 0)
                {
                    string Error = "No Data";
                    return Json(new { Error, time = false }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in getCourse)
                {

                    TimetableVM AddNameList = new TimetableVM
                    {
                        StudentName = item.Username,
                        StudentID = item.UsersID
                    };
                    getData.Add(AddNameList);
                }

                return Json(new { TotalStudent, getData, time = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { time = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckAllStudentAttendance(int Dataid, string GetSelectedProgramme, string SelectedOption, string GetSelectedCourse)
        {
            List<TimetableVM> getStudentAttendance = new List<TimetableVM>();
            var getAllStudent = db.Users.Where(c => c.Roles == Roles.Student).ToList();
            var getAllCourseTime = db.CourseTimetables.Where(c => c.CoursesID.ToString() == GetSelectedCourse).ToList();
            var getStudent = db.Users.SingleOrDefault(c => c.UsersID == Dataid);

            if (getAllCourseTime.Count() == 0)
            {
                string Error = "This course haven't create a timetable";
                return Json(new { Error, notime = true }, JsonRequestBehavior.AllowGet);
            }

            string Status = "";
            string CheckAttendanceTime = "";
            int TotalCourseTime = 0;
            int TotalCheckedIn = 0;
            int TotalStudent = getAllStudent.Count();

            if (SelectedOption == "Course")
            {
                foreach (var item in getAllStudent)
                {
                    var getCourseTime = db.Attendance.Where(c => c.CourseTimetableId == Dataid && c.Name == item.Username).SingleOrDefault();

                    if (getCourseTime != null)
                    {
                        Status = "Checked In";
                        CheckAttendanceTime = getCourseTime.AttendanceTime.ToString();
                        TotalCheckedIn++;
                    }
                    else
                    {
                        Status = "No Check In";
                        CheckAttendanceTime = "No Check In";
                    }
                    TimetableVM AddTimetable = new TimetableVM
                    {
                        StudentName = item.Username,
                        CheckInButton = Status,
                        AttendanceTime =CheckAttendanceTime
                    };
                    getStudentAttendance.Add(AddTimetable);
                }
                return Json(new { TotalCheckedIn, TotalStudent, getStudentAttendance, time = false }, JsonRequestBehavior.AllowGet);

            }
            else if (SelectedOption == "Student")
            {
                foreach (var item in getAllCourseTime)
                {
                    var checkStudentAttendance = db.Attendance.Where(c => c.CourseTimetableId == item.CourseTimetableId && c.Name == getStudent.Username).SingleOrDefault();

                    if (checkStudentAttendance != null)
                    {
                        Status = "Checked In";
                        CheckAttendanceTime = checkStudentAttendance.AttendanceTime.ToString();
                        TotalCheckedIn++;
                    }
                    else
                    {
                        Status = "No Check In";
                        CheckAttendanceTime = "No Check In";
                    }
                    TimetableVM AddTimetable = new TimetableVM
                    {
                        StartTime = item.StartTime.ToString(),
                        EndTime = item.EndTime.ToString(),
                        CheckInButton = Status,
                        AttendanceTime = CheckAttendanceTime
                    };
                    getStudentAttendance.Add(AddTimetable);
                }
                TotalCourseTime = getAllCourseTime.Count();
                return Json(new { TotalCourseTime, TotalCheckedIn, getStudentAttendance, time = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { time = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateCourse(string selectedProgramme, string CreateOption, string SelectedCourseId, string CourseName, string StartTime, string EndTime)
        {
            var programme = (Programme)Enum.Parse(typeof(Programme), selectedProgramme);
            var CheckCourseName = db.Courses.SingleOrDefault(c => c.CourseName == CourseName && c.Programme == programme);
            if (CreateOption == "Course")
            {
                if (CheckCourseName != null)
                {
                    string Error = "This Course Name Already Exist";
                    return Json(new { Error, SaveCourse = false }, JsonRequestBehavior.AllowGet);
                }
                Courses getCourseName = new Courses
                {
                    Programme = programme,
                    CourseName = CourseName
                };
                db.Courses.Add(getCourseName);
                db.SaveChanges();
                return Json(new { SaveCourse = true }, JsonRequestBehavior.AllowGet);
            }
            else
            //if (CreateOption == "Time")
            {
                DateTime getStartTime = Convert.ToDateTime(StartTime);
                DateTime getEndTime = Convert.ToDateTime(EndTime);
                var id = Convert.ToInt32(SelectedCourseId);
                var getCourseName = db.Courses.SingleOrDefault(c => c.CoursesID == id && c.Programme == programme);
                if (getStartTime >= getEndTime)
                {
                    string Error = "Start Time must small than End Time";
                    return Json(new { Error, Savetime = false }, JsonRequestBehavior.AllowGet);
                }
                var CheckTime = db.CourseTimetables.SingleOrDefault(ct => ct.CoursesID == getCourseName.CoursesID && getCourseName.Programme == programme && ct.StartTime == getStartTime);
                if (CheckTime != null)
                {
                    string Error = "You Already Have A Time For This Time";
                    return Json(new { Error, Savetime = false }, JsonRequestBehavior.AllowGet);
                }
                CourseTimetable timetable = new CourseTimetable
                {
                    CoursesID = getCourseName.CoursesID,
                    StartTime = getStartTime,
                    EndTime = getEndTime
                };
                db.CourseTimetables.Add(timetable);
                db.SaveChanges();
                return Json(new { Savetime = true }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Admin/Edit/5
        public ActionResult EditCourseTime(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTimetable courseTimetable = db.CourseTimetables.SingleOrDefault(ct => ct.CourseTimetableId == id);
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == courseTimetable.CoursesID);

            if (courseTimetable == null)
            {
                return HttpNotFound();
            }
            TimetableVM timetable = new TimetableVM
            {
                CourseTimetableId = courseTimetable.CourseTimetableId,
                CourseName = courses.CourseName,
                //Programme = courses.Programme,
                StartTime = courseTimetable.StartTime.ToString(),
                EndTime = courseTimetable.EndTime.ToString(),

            };
            return View(timetable);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourseTime([Bind(Include = "CourseTimetableId,StartTime,EndTime")] CourseTimetable courseTimetable)
        {
            if (ModelState.IsValid)
            {
                var getdata = db.CourseTimetables.SingleOrDefault(ct => ct.CourseTimetableId == courseTimetable.CourseTimetableId);
                getdata.StartTime = courseTimetable.StartTime;
                getdata.EndTime = courseTimetable.EndTime;
                db.SaveChanges();
                return RedirectToAction("SelectProgramme");
            }
            return View(courseTimetable);
        }

        // GET: Admin/Details/5
        public ActionResult CourseTimeDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTimetable courseTimetable = db.CourseTimetables.SingleOrDefault(ct => ct.CourseTimetableId == id);
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == courseTimetable.CoursesID);
            if (courseTimetable == null)
            {
                return HttpNotFound();
            }
            TimetableVM timetable = new TimetableVM
            {
                //CoursesID = courses.CoursesID,
                CourseName = courses.CourseName,
                CourseTimetableId = courseTimetable.CourseTimetableId,
                //Programme = courses.Programme,
                StartTime = courseTimetable.StartTime.ToString(),
                EndTime = courseTimetable.EndTime.ToString(),

            };
            return View(timetable);
        }

        // GET: Admin/Delete/5
        public ActionResult DeleteCourseTime(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTimetable courseTimetable = db.CourseTimetables.SingleOrDefault(ct => ct.CourseTimetableId == id);
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == courseTimetable.CoursesID);
            if (courseTimetable == null)
            {
                return HttpNotFound();
            }
            TimetableVM timetable = new TimetableVM
            {
                //CoursesID = courses.CoursesID,
                CourseName = courses.CourseName,
                CourseTimetableId = courseTimetable.CourseTimetableId,
                //Programme = courses.Programme,
                StartTime = courseTimetable.StartTime.ToString(),
                EndTime = courseTimetable.EndTime.ToString(),

            };
            return View(timetable);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("DeleteCourseTime")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseTimetable courseTimetable = db.CourseTimetables.SingleOrDefault(ct => ct.CourseTimetableId == id);
            db.CourseTimetables.Remove(courseTimetable);
            db.SaveChanges();
            return RedirectToAction("SelectProgramme");
        }










        public ActionResult EditCourse(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse([Bind(Include = "CoursesID,CourseName,Programme")] Courses courses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SelectProgramme");
            }
            return View(courses);
        }

        public ActionResult DeleteCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourseConfirmed(int id)
        {
            var getId = db.Courses.SingleOrDefault(c => c.CoursesID == id);
            var checktime = db.CourseTimetables.Where(ct => ct.CoursesID == getId.CoursesID);
            if (checktime.Count() != 0)
            {
                db.CourseTimetables.RemoveRange(checktime);
            }
            Courses courses = db.Courses.SingleOrDefault(c => c.CoursesID == id);
            db.Courses.Remove(courses);
            db.SaveChanges();
            return RedirectToAction("SelectProgramme");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
