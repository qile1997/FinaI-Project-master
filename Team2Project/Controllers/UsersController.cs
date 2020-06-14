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
    public class UsersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            var users = CheckUser(user);
            if (users != null)
            {
                if (users.Roles == Roles.Student)
                {
                    Session["Student"] = users.UsersID;
                    return RedirectToAction("CheckIn");
                }
                else if (users.Roles == Roles.Trainer)
                {
                    Session["Admin"] = users.UsersID;
                    return RedirectToAction("AttendanceList", "Admin");
                }
            }
            else
            {
                ViewBag.Error = "Wrong Username and Passowrd ";
            }
            return View();
        }

        [NonAction]
        public Users CheckUser(Users user)
        {
            var GetUser = db.Users.SingleOrDefault(a => a.Username == user.Username && a.Password == user.Password);
            return GetUser;
        }

        public ActionResult GetAttendance()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAttendance(Users users)
        {
            return View();
        }

        public ActionResult Profile()
        {
            var id = Convert.ToInt32(Session["Student"]);

            var user = db.Users.SingleOrDefault(u => u.UsersID == id);
            if (user != null)
            {
                return View(user);
            }
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult CheckIn()
        {
            var id = Convert.ToInt32(Session["Student"]);

            var user = db.Users.SingleOrDefault(u => u.UsersID == id);
            if (user != null)
            {
                Session["StudentProgramme"] = user.Programme;
                Session["SelectedProgramme"] = user.Programme.ToString();
                var courses = db.Courses.Where(c => c.Programme == user.Programme).ToList();
                ViewBag.Courses = new SelectList(courses, "CoursesID", "CourseName");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(string CourseName)
        {
            List<TimetableVM> getTimetable = new List<TimetableVM>();
            var id = Convert.ToInt32(Session["Student"]);
            var Users = db.Users.SingleOrDefault(c => c.UsersID == id);
            var Attendance = db.Attendance.Where(c => c.Name == Users.Username).ToList();

            var getCourse = db.Courses.SingleOrDefault(c => c.CourseName == CourseName && c.Programme == Users.Programme);
            var getTime = db.CourseTimetables.Where(c => c.CoursesID == getCourse.CoursesID);
            string Check = "";
            string StatusMsg = "";
            int TotalCourseTime = 0;

            if (CourseName == "--Select Course--")
            {
                return Json(new { course = false }, JsonRequestBehavior.AllowGet);
            }
            if (getCourse == null)
            {
                return RedirectToAction("CheckIn");
            }
            if (getTime.Count() == 0)
            {
                string Error = "This course haven't create a timetable";
                return Json(new { Error, time = false }, JsonRequestBehavior.AllowGet);
            }


            foreach (var item in getTime)
            {
                var checkAttendance = Attendance.SingleOrDefault(c => c.CourseTimetableId == item.CourseTimetableId);

                if (DateTime.Now < item.StartTime)
                {
                    StatusMsg = "Course Not Started";
                }
                else if (DateTime.Now > item.EndTime)
                {
                    StatusMsg = "Failed Check In";
                }
                else if (DateTime.Now > item.StartTime || DateTime.Now < item.EndTime)
                {
                    StatusMsg = "Check In";
                }

                if (checkAttendance != null)
                {
                    Check = "Checked In";
                    StatusMsg = "Checked In";
                }
                else
                {
                    Check = "Check In";
                }
                TimetableVM AddTimetable = new TimetableVM
                {
                    CoursesID = item.CoursesID,
                    CourseTimetableId = item.CourseTimetableId,
                    CourseName = getCourse.CourseName,
                    Programme = getCourse.Programme,
                    StartTime = item.StartTime.ToString(),
                    EndTime = item.EndTime.ToString(),
                    CheckInButton = Check,
                    Msg = StatusMsg
                };
                getTimetable.Add(AddTimetable);

            }
            TotalCourseTime = getTimetable.Count();
            return Json(new { TotalCourseTime, getTimetable, time = true }, JsonRequestBehavior.AllowGet);
            //Session["SelectedCourse"] = selected.CourseName;
            //return RedirectToAction("CourseDateIndex");
        }

        // Ajax
        [HttpPost]
        public ActionResult TakeAttendance(int CourseTimeID)
        {
            var getTime = db.CourseTimetables.SingleOrDefault(c => c.CourseTimetableId == CourseTimeID);
            var studentID = Convert.ToInt32(Session["Student"]);
            var student = db.Users.SingleOrDefault(c => c.UsersID == studentID);

            if (DateTime.Now < getTime.StartTime || DateTime.Now > getTime.EndTime)
            {
                return Json(new { TdyDate = false }, JsonRequestBehavior.AllowGet);
            }

            Attendance AddAttendance = new Attendance
            {
                Name = student.Username,
                Programme = student.Programme,
                CourseTimetableId = getTime.CourseTimetableId,
                AttendanceTime = DateTime.Now
            };
            db.Attendance.Add(AddAttendance);
            db.SaveChanges();
            return Json(new { TdyDate = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckAttendance()
        {
            var id = Convert.ToInt32(Session["Student"]);

            var user = db.Users.SingleOrDefault(u => u.UsersID == id);
            if (user != null)
            {
                Session["StudentProgramme"] = user.Programme;
                Session["SelectedProgramme"] = user.Programme.ToString();
                var courses = db.Courses.Where(c => c.Programme == user.Programme).ToList();
                ViewBag.Courses = new SelectList(courses, "CoursesID", "CourseName");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckAttendance(string CourseName)
        {
            List<TimetableVM> getTimetable = new List<TimetableVM>();
            var id = Convert.ToInt32(Session["Student"]);
            var Users = db.Users.SingleOrDefault(c => c.UsersID == id);
            var Attendance = db.Attendance.Where(c => c.Name == Users.Username).ToList();

            var getCourse = db.Courses.SingleOrDefault(c => c.CourseName == CourseName && c.Programme == Users.Programme);
            var getTime = db.CourseTimetables.Where(c => c.CoursesID == getCourse.CoursesID);
            string Check = "";
            string CheckAttendanceTime = "";
            int TotalCourseTime = 0;
            int TotalCheckedIn = 0;
            if (CourseName == "--Select Course--")
            {
                return Json(new { course = false }, JsonRequestBehavior.AllowGet);
            }
            if (getCourse == null)
            {
                return RedirectToAction("CheckIn");
            }
            if (getTime.Count() == 0)
            {
                string Error = "This course haven't create a timetable";
                return Json(new { Error, time = false }, JsonRequestBehavior.AllowGet);
            }

            foreach (var item in getTime)
            {
                var checkAttendance = Attendance.SingleOrDefault(c => c.CourseTimetableId == item.CourseTimetableId);

                if (checkAttendance != null)
                {
                    Check = "Checked In";
                    CheckAttendanceTime = checkAttendance.AttendanceTime.ToString();
                    TotalCheckedIn++;
                }
                else
                {
                    Check = "No Check In";
                    CheckAttendanceTime = "No Check In";
                }
                TimetableVM AddTimetable = new TimetableVM
                {
                    CoursesID = item.CoursesID,
                    CourseTimetableId = item.CourseTimetableId,
                    CourseName = getCourse.CourseName,
                    Programme = getCourse.Programme,
                    StartTime = item.StartTime.ToString(),
                    EndTime = item.EndTime.ToString(),
                    AttendanceTime = CheckAttendanceTime,
                    CheckInButton = Check
                };
                getTimetable.Add(AddTimetable);

            }
            TotalCourseTime = getTimetable.Count();
            return Json(new { TotalCheckedIn, TotalCourseTime, getTimetable, time = true }, JsonRequestBehavior.AllowGet);
            //Session["SelectedCourse"] = selected.CourseName;
            //return RedirectToAction("CourseDateIndex");
        }






















        //// GET: Users
        //public ActionResult Index()
        //{
        //    return View(db.Users.ToList());
        //}

        //// GET: Users/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Users users = db.Users.Find(id);
        //    if (users == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(users);
        //}

        //// GET: Users/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Users/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UsersID,Username,Password,Email,Programme,Roles")] Users users)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(users);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(users);
        //}

        //// GET: Users/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Users users = db.Users.Find(id);
        //    if (users == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(users);
        //}

        //// POST: Users/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UsersID,Username,Password,Email,Programme,Roles")] Users users)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(users).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(users);
        //}

        //// GET: Users/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Users users = db.Users.Find(id);
        //    if (users == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(users);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Users users = db.Users.Find(id);
        //    db.Users.Remove(users);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
