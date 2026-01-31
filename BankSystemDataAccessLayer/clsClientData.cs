using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemDataAccessLayer
{
    public class clsClientData
    {
        public static bool FindClient(int clientId, ref int accountNumber, ref int personId, ref int pinCode, ref double balance)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * FROM ClientInformation WHERE ClientID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", clientId);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    accountNumber = (int)reader["AccountNumber"];
                    personId = (int)reader["PersonID"];
                    pinCode = (int)reader["PinCode"];
                    balance = (double)reader["Balance"];
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

        public static bool FindClient(ref int clientId, int accountNumber, ref int personId, ref int pinCode, ref double balance)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * FROM ClientInformation WHERE AccountNumber = @accNum";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@accNum", accountNumber);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    clientId = (int)reader["ClientID"];
                    personId = (int)reader["PersonID"];
                    pinCode = (int)reader["PinCode"];
                    balance = (double)reader["Balance"];
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
        public static int AddNewClient(int accNum, int personId, int code, double balance)
        {
            int clientId = -1;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "INSERT INTO ClientInformation Values (@accNum, @personId, @code, @balance); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@accNum", accNum);
            command.Parameters.AddWithValue("@personId", personId);
            command.Parameters.AddWithValue("@code", code);
            command.Parameters.AddWithValue("@balance", balance);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedIn))
                {
                    clientId = insertedIn;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return clientId;
        }
        public static bool UpdateClient(int clientId, int accNum, int personId, int code, double balance)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "UPDATE ClientInformation set AccountNumber = @acc, PersonID = @personId, PinCode = @code, Balance = @balance WHERE ClientID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@acc", accNum);
            command.Parameters.AddWithValue("@personId", personId);
            command.Parameters.AddWithValue("@code", code);
            command.Parameters.AddWithValue("@balance", balance);
            command.Parameters.AddWithValue("@id", clientId);
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
        public static bool DeleteClient(int clientId)
        {
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "Delete from ClientInformation WHERE ClientID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", clientId);
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
        public static DataTable GetAllClients()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * from Client_view";
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
        public static bool ClientIsExist(int ClientId)
        {
            bool isExist = false;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT found = 1 FROM ClientInformation WHERE ClientID = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", ClientId);
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
        public static float GetBalanceByAccountNumber(int accountNumber)
        {
            float balance = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT Balance FROM ClientInformation WHERE AccountNumber = @accNum";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@accNum", accountNumber);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && float.TryParse(result.ToString(), out float total))
                {
                    balance = total;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return balance;
        }
        public static float GetTotalBalances()
        {
            float balance = 0;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT SUM(Balance) FROM ClientInformation";
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && float.TryParse(result.ToString(), out float total))
                {
                    balance = total;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return balance;
        }

        public static DataTable GetAllTransfer()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "SELECT * from TransferLog";
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

        public static bool AddNewTransfer(DateTime date, int sAcc, int rAcc, double amount, double sBalance, double rBalance, int userId)
        {
            int transferId = -1;
            SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString);
            string query = "INSERT INTO TransferLog Values (@date, @sAcc, @rAcc, @amount, @sBalance, @rBalance, @userId); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@sAcc", sAcc);
            command.Parameters.AddWithValue("@rAcc", rAcc);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@sBalance", sBalance);
            command.Parameters.AddWithValue("@rBalance", rBalance);
            command.Parameters.AddWithValue("@userId", userId);
            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedIn))
                {
                    transferId = insertedIn;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }
            return transferId > -1;
        }
    }
}
