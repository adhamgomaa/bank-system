using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemDataAccessLayer
{
    public class clsUserData
    {
        public static bool FindUser(int userId, ref int personId, ref string username, ref string password, ref int permission, ref bool isActive)
        {
            bool isExist = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * FROM UserInformation WHERE UserID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", userId);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isExist = true;
                    personId = (int)reader["PersonID"];
                    username = (string)reader["UserName"];
                    password = (string)reader["Password"];
                    permission = (int)reader["Permissions"];
                    isActive = (bool)reader["isActive"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            finally
            {
                connection.Close();
            }
            return isExist;
        }

        public static bool FindUser(string username, ref int userId, ref int personId, ref string password, ref int permission, ref bool isActive)
        {
            bool isExist = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * FROM UserInformation WHERE UserName = @username";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isExist = true;
                    personId = (int)reader["PersonID"];
                    userId = (int)reader["UserID"];
                    password = (string)reader["Password"];
                    permission = (int)reader["Permissions"];
                    isActive = (bool)reader["isActive"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            finally
            {
                connection.Close();
            }
            return isExist;
        }
        public static bool FindUser(string username, string password, ref int userId, ref int personId, ref int permission, ref bool isActive)
        {
            bool isExist = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * FROM UserInformation WHERE UserName = @name and Password = @pass";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            command.Parameters.AddWithValue("@pass", password);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isExist = true;
                    userId = (int)reader["UserID"];
                    personId = (int)reader["PersonID"];
                    permission = (int)reader["Permissions"];
                    isActive = (bool)reader["isActive"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            finally
            {
                connection.Close();
            }
            return isExist;
        }
        public static int AddNewUser(int personId, string username, string password, int permission, bool isActive)
        {
            int userId = -1;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "INSERT INTO UserInformation Values (@personId, @username, @password, @permission, @isActive); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@personId", personId);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@permission", permission);
            command.Parameters.AddWithValue("@isActive", isActive);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedIn))
                {
                    userId = insertedIn;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return userId;
        }
        public static bool UpdateUser(int userId, int personId, string username, string password, int permission, bool isActive)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "UPDATE UserInformation set PersonId = @personId, UserName = @username, Password = @pass, Permissions = @permission, isActive = @isActive WHERE UserID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@personId", personId);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@pass", password);
            command.Parameters.AddWithValue("@permission", permission);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@id", userId);
            try
            {
                connection.Open();
                rowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
            return rowAffected > 0;
        }
        public static bool DeleteUser(int userId)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Delete from UserInformation WHERE UserId = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", userId);
            try
            {
                connection.Open();
                rowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
            return rowAffected > 0;
        }
        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * from User_view";
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
        public static bool UserIsExist(int userId)
        {
            bool isExist = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT found = 1 FROM UserInformation WHERE UserID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", userId);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isExist = true;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            finally
            {
                connection.Close();
            }
            return isExist;
        }

        public static DataTable GetAllRegisters()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * from Register_view order by RegisterID desc";
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
        public static int AddNewRegister(int userId)
        {
            int registerId = -1;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "INSERT INTO RegisterLog Values (@userId, @date); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedIn))
                {
                    registerId = insertedIn;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return registerId;
        }
    }
}
