using LogWorkingHour.Helper;
using LogWorkingHour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using static LogWorkingHour.Helper.DatabaseHelper;

namespace LogWorkingHour.Controllers
{
    public class ValuesController : ApiController
    {
        DatabaseHelper DBHelper = new DatabaseHelper();
        JwtTokenGenerator JwtTokenGenerator = new JwtTokenGenerator();

        #region EmployeeRegister
        [HttpPost]
        [Route("api/Values/Insert")]
        public IHttpActionResult InsertEmployee([FromBody] EmployeeModel employee)
        {
            InsertionResult result;

            DBHelper.AddEmployee(employee, out result);

            switch (result)
            {
                case InsertionResult.Success:
                    return Ok("Data Inserted Successfully !");
                case InsertionResult.EmailAlreadyExist:
                    return BadRequest("Same Mail Already Exist !!!");
                case InsertionResult.InsertionNotSuccessfull:
                    return BadRequest("Error Occured During Data Insertion !!!");
                default:
                    return InternalServerError();
            }
        }
        #endregion

        #region EmployeeLogin
        [HttpPost]
        [Route("api/Values/Login")]
        public IHttpActionResult EmployeeLogin([FromBody] LoginModel login)
        {
            if (DBHelper.IsValidUser(login.Email, login.Password))
            {

                var token = JwtTokenGenerator.GenerateToken(login.Email);

                return Ok(new { token });
            }

            return Unauthorized();
        }
        #endregion
        [HttpGet]
        [Route("api/Values/GetData")]
        [Authorize]
        public IHttpActionResult GetEmployeeData()
        {
            var principal = RequestContext.Principal as ClaimsPrincipal;

            if(principal != null)
            {
                var emailClaim = principal.Claims.FirstOrDefault( c => c.Type == ClaimTypes.Email) ?.Value;
                if(!string.IsNullOrEmpty(emailClaim))
                {
                   var employeeDetails = DBHelper.GetEmployee(emailClaim);

                    if (employeeDetails != null)
                        return Ok(employeeDetails);
                    else
                        return NotFound();
                }
                else
                    return BadRequest("Claim not found in the user's claims !!");
            }
            else
                return BadRequest("User principal not found !!");
        }








        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        //public void Post([FromBody] string value)
        //{

        //}

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
