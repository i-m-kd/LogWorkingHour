using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogWorkingHour.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; } 
    }
}