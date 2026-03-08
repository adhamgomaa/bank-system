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
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindUserById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isExist = true;
                                personId = (int)reader["PersonID"];
                                username = (string)reader["UserName"];
                                password = (string)reader["Password"];
                                permission = (int)reader["Permissions"];
                                isActive = (bool)reader["isActive"];
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                isExist = false;
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isExist;
        }

        public static bool FindUser(string username, ref int userId, ref int personId, ref string password, ref int permission, ref bool isActive)
        {
            bool isExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindUserByUsername", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@username", username);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isExist = true;
                                personId = (int)reader["PersonID"];
                                userId = (int)reader["UserID"];
                                password = (string)reader["Password"];
                                permission = (int)reader["Permissions"];
                                isActive = (bool)reader["isActive"];
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                isExist = false;
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isExist;
        }
        public static bool FindUser(string username, string password, ref int userId, ref int personId, ref int permission, ref bool isActive)
        {
            bool isExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindUserByUsernameAndPass", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isExist = true;
                                userId = (int)reader["UserID"];
                                personId = (int)reader["PersonID"];
                                permission = (int)reader["Permissions"];
                                isActive = (bool)reader["isActive"];
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                isExist = false;
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isExist;
        }
        public static int AddNewUser(int personId, string username, string password, int permission, bool isActive)
        {
            int userId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewUser", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@permission", permission);
                        command.Parameters.AddWithValue("@isActive", isActive);
                        SqlParameter outputId = new SqlParameter("@userId", SqlDbType.Int);
                        outputId.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputId);
                        connection.Open();
                        command.ExecuteNonQuery();
                        userId = (int)outputId.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return userId;
        }
        public static bool UpdateUser(int userId, int personId, string username, string password, int permission, bool isActive)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@permission", permission);
                        command.Parameters.AddWithValue("@isActive", isActive);
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            return rowAffected > 0;
        }
        public static bool DeleteUser(int userId)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_DeleteUser", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        rowAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            return rowAffected > 0;
        }
        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllUsers", connection))
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
            catch (Exception ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return dt;
        }
        public static bool UserIsExist(int userId)
        {
            bool isExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_IsUserExist", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isExist = true;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                isExist = false;
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return isExist;
        }

        public static DataTable GetAllRegisters()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllRegisters", connection))
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
        public static int AddNewRegister(int userId)
        {
            int registerId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_LoginLog", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userId", userId);
                        SqlParameter outputId = new SqlParameter("@registerId", SqlDbType.Int);
                        outputId.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputId);
                        connection.Open();
                        command.ExecuteNonQuery();
                        registerId = (int)outputId.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return registerId;
        }
    }
}
