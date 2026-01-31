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
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Select * from People Where PersonID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
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
                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }

        public static int AddNewPerson(string firstName, string secName, string lastName, string email, string phone, byte gender, 
            DateTime bDate)
        {
            int personId = -1;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Insert into People Values (@fname, @secname, @lname, @email, @phone, @gender, @date); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@fname", firstName);
            command.Parameters.AddWithValue("@secname", secName);
            command.Parameters.AddWithValue("@lname", lastName);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@gender", gender);
            command.Parameters.AddWithValue("@date", bDate);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedIn))
                {
                    personId = insertedIn;
                }
            }
            catch (Exception ex)
            {

            }finally
            {
                connection.Close();
            }
            return personId;
        }

        public static bool UpdatePerson(int id, string firstName, string secName, string lastName, string email, string phone, byte gender,
            DateTime bDate)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Update People set FirstName = @fname, SecondName = @secname, LastName = @lname, Email = @email, Phone = @phone, Gender = @gender, BirthDate = @date Where PersonID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@fname", firstName);
            command.Parameters.AddWithValue("@secname", secName);
            command.Parameters.AddWithValue("@lname", lastName);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@gender", gender);
            command.Parameters.AddWithValue("@date", bDate);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                connection.Open();
                rowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }finally
            {
                connection.Close();
            }
            return rowAffected > 0;
        }

        public static bool DeletePerson(int id)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Delete from People Where PersonID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                connection.Open();
                rowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return rowAffected > 0;
        }

        public static bool IsPersonExist(int id)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Select found = 1 from People Where PersonID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                    isFound = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * from People";
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
    }
}
