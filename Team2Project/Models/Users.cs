using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Team2Project.Models
{
    public class Users
    {
        public int UsersID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }   


        public Programme? Programme { get; set; }

        public Roles Roles { get; set; }
    }
    public enum Roles
    {
        Student,Trainer
    }

    public enum Programme
    {
        [Display(Name ="Diploma In IT")]
        DiplomaInIT,
        [Display(Name = "Diploma In SE")]
        DiplomaInSE,
        [Display(Name = "Degree In IT")]
        DegreeInIT,
        [Display(Name = "Degree In SE")]
        DegreeInSE
    }
}