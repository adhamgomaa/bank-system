using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Data;

namespace BankSystemDataAccessLayer
{
    public class clsPeopleData
    {
        public static bool FindPerson(int id, ref string firstName, ref string secName, ref string lastName, ref string email,
            ref string phone, ref byte gender, ref DateTime bDate)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindPersonById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@personId", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                firstName = (string)reader["FirstName"];
                                secName = (string)reader["SecondName"];
                                lastName = (string)reader["LastName"];
                                email = (string)reader["Email"];
                                phone = (string)reader["Phone"];
                                gender = (byte)reader["Gender"];
                                bDate = (DateTime)reader["BirthDate"];
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                isFound = false;
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isFound;
        }

        public static int AddNewPerson(string firstName, string secName, string lastName, string email, string phone, byte gender, 
            DateTime bDate)
        {
            int personId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddPerson", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fname", firstName);
                        command.Parameters.AddWithValue("@secname", secName);
                        command.Parameters.AddWithValue("@lname", lastName);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@gender", gender);
                        command.Parameters.AddWithValue("@date", bDate);
                        SqlParameter outputId = new SqlParameter("@personId", SqlDbType.Int);
                        outputId.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputId);
                        connection.Open();
                        command.ExecuteNonQuery();
                        personId = (int)outputId.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return personId;
        }

        public static bool UpdatePerson(int id, string firstName, string secName, string lastName, string email, string phone, byte gender,
            DateTime bDate)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdatePerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fname", firstName);
                        command.Parameters.AddWithValue("@secname", secName);
                        command.Parameters.AddWithValue("@lname", lastName);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@gender", gender);
                        command.Parameters.AddWithValue("@date", bDate);
                        command.Parameters.AddWithValue("@personId", id);
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return rowAffected > 0;
        }

        public static bool DeletePerson(int id)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_DeletePerson", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@personId", id);
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return rowAffected > 0;
        }

        public static bool IsPersonExist(int id)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_IsPersonExist", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id", id);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                isFound = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllPeople", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return dt;
        }
    }
}
