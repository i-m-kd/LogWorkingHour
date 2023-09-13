using LogWorkingHour.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Web;

namespace LogWorkingHour.Helper
{
    public class DatabaseHelper
    {
        #region ConnectionString
        private readonly string connectionString;
        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        }
        #endregion

        public bool IsValidUser(string userName, string password)
        {
            bool isValidUser = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand getPasswordCommand = new SqlCommand("GetPasswordByEmail", connection))
                {
                    getPasswordCommand.CommandType = CommandType.StoredProcedure;
                    getPasswordCommand.Parameters.AddWithValue("@Email", userName);

                    string hashedPasswordFromDb = getPasswordCommand.ExecuteScalar() as string;

                    if (!string.IsNullOrEmpty(hashedPasswordFromDb))
                    {
                        isValidUser = BCrypt.Net.BCrypt.Verify(password, hashedPasswordFromDb);
                    }
                }

                connection.Close();
            }

            return isValidUser;
        }


        #region AddEmployee
        public void AddEmployee(EmployeeModel employee, out InsertionResult result)
        {

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("AddEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", employee.Name);
                    command.Parameters.AddWithValue("@Department", employee.Department);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    SqlParameter outputParameter = new SqlParameter
                    {
                        ParameterName = "@Result",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(outputParameter);

                    command.ExecuteNonQuery();

                    if (!Enum.TryParse<InsertionResult>(outputParameter.Value.ToString(), out result))
                    {
                        result = InsertionResult.ErrorOccured;
                    }
                }
                connection.Close();
            }

        } 
        #endregion

        public EmployeeModel GetEmployee(string email)
        {
            using(SqlConnection connection = new SqlConnection())
            {
                connection.Open();

                using(SqlCommand command = new SqlCommand("GetEmployeeData",connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader["Name"].ToString();
                            string department = reader["Department"].ToString();

                            EmployeeModel employeeData = new EmployeeModel
                            {
                                Name = name,
                                Department = department
                            };
                            return employeeData;
                        }
                    }
                }
            }
            return null;
        }

        public enum InsertionResult
        {
            Success = 0
            ,EmailAlreadyExist
            ,InsertionNotSuccessfull
            ,ErrorOccured

        }
    }

}