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
        public static bool FindClient(int clientId, ref int accountNumber, ref int personId, ref int pinCode, ref decimal balance)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindClientById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@clientId", clientId);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                accountNumber = (int)reader["AccountNumber"];
                                personId = (int)reader["PersonID"];
                                pinCode = (int)reader["PinCode"];
                                balance = (decimal)reader["Balance"];
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

        public static bool FindClient(ref int clientId, int accountNumber, ref int personId, ref int pinCode, ref decimal balance)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_FindClientByAccNum", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", accountNumber);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                clientId = (int)reader["ClientID"];
                                personId = (int)reader["PersonID"];
                                pinCode = (int)reader["PinCode"];
                                balance = (decimal)reader["Balance"];
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
        public static int AddNewClient(int accNum, int personId, int code, decimal balance)
        {
            int clientId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", accNum);
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@pinCode", code);
                        command.Parameters.AddWithValue("@balance", balance);
                        SqlParameter outputId = new SqlParameter("@clientId", SqlDbType.Int);
                        outputId.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputId);
                        connection.Open();
                        command.ExecuteNonQuery();
                        clientId = (int)outputId.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return clientId;
        }
        public static bool UpdateClient(int clientId, int accNum, int personId, int code, decimal balance)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdateClient", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", accNum);
                        command.Parameters.AddWithValue("@personId", personId);
                        command.Parameters.AddWithValue("@pinCode", code);
                        command.Parameters.AddWithValue("@balance", balance);
                        command.Parameters.AddWithValue("@clientId", clientId);
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
        public static bool DeleteClient(int clientId)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_DeleteClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@clientId", clientId);
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
        public static DataTable GetAllClients()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllClients", connection))
                    {
                        command.CommandType=CommandType.StoredProcedure;
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
        public static bool ClientIsExist(int ClientId)
        {
            bool isExist = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_ClientIsExists", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@clientId", ClientId);
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
        public static decimal GetBalanceByAccountNumber(int accountNumber)
        {
            decimal balance = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetClientBalance", connection))
                    {
                        command.CommandType=CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", accountNumber);
                        SqlParameter output = new SqlParameter("@balance", SqlDbType.Decimal);
                        output.Direction = ParameterDirection.Output;
                        command.Parameters.Add(output);
                        connection.Open();
                        command.ExecuteNonQuery();
                        balance = (decimal)output.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return balance;
        }
        public static decimal GetTotalBalances()
        {
            decimal balance = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetTotalBalance", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter output = new SqlParameter("@totalBalance", SqlDbType.Decimal);
                        output.Direction = ParameterDirection.Output;
                        command.Parameters.Add(output);
                        connection.Open();
                        command.ExecuteNonQuery();
                        balance = (decimal)output.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return balance;
        }

        public static bool Deposit(int AccNum, decimal amount)
        {
            int rowAffected = 0;
            try
            {
                using(SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using( SqlCommand command = new SqlCommand("SP_Deposit", connection))
                    {
                        command.CommandType=CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", AccNum);
                        command.Parameters.AddWithValue("@amount", amount);
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

        public static bool Withdrawal(int AccNum, decimal amount)
        {
            int rowAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Withdrawal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@accNum", AccNum);
                        command.Parameters.AddWithValue("@amount", amount);
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

        public static DataTable GetAllTransfer()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_TransferLog", connection))
                    {
                        command.CommandType= CommandType.StoredProcedure;
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

        public static bool AddNewTransfer(int sAcc, int rAcc, decimal amount, int userId)
        {
            int transferId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsBankSystemDataSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Transfer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FromAccNum", sAcc);
                        command.Parameters.AddWithValue("@ToAccNum", rAcc);
                        command.Parameters.AddWithValue("@amount", amount);
                        command.Parameters.AddWithValue("@userId", userId);
                        SqlParameter output = new SqlParameter("@transferId", SqlDbType.Int);
                        output.Direction = ParameterDirection.Output;
                        command.Parameters.Add(output);
                        connection.Open();
                        command.ExecuteNonQuery();
                        transferId = (int)output.Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLogger.LoggingAllExepctions(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return transferId > -1;
        }
    }
}
