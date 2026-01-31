using BankSystemDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemBusinessLayer
{
    public class clsClient
    {
        enum enMode { Add, Update}
        enMode mode = enMode.Add;

        public int clientId { get; private set; }
        public int accountNumber { get; set; }
        public int personId { get; set; }
        public clsPeople personInfo { get; private set; }
        public int pinCode { get; set; }
        public double balance { get; set; }

        public event EventHandler<TransactionEventArgs> TransactionChanged;
        public clsClient()
        {
            clientId = -1;
            accountNumber = 0;
            personId = -1;
            pinCode = 0;
            balance = 0;
            mode = enMode.Add;
        }

        private clsClient(int clientId, int accountNumber, int personId, int pinCode, double balance)
        {
            this.clientId = clientId;
            this.accountNumber = accountNumber;
            this.personId = personId;
            this.pinCode = pinCode;
            this.balance = balance;
            personInfo = clsPeople.FindPerson(personId);
            mode = enMode.Update;
        }

        public void Deposit(double amount)
        {
            double currentBalance = balance;
            balance += amount;
            this.Save();
            OnTransactionChanged(currentBalance, amount);
        }

        public bool Withdraw(double amount)
        {
            if (amount > balance)
                return false;
            double currentBalance = balance;
            balance -= amount;
            this.Save();
            OnTransactionChanged(currentBalance, -amount);
            return true;
        }

        protected virtual void OnTransactionChanged(double currentBalance, double transactionAmount)
        {
            TransactionChanged.Invoke(this, new TransactionEventArgs(this.accountNumber, currentBalance, transactionAmount));
        }

        private bool _AddNewClient()
        {
            this.clientId = clsClientData.AddNewClient(accountNumber, personId, pinCode, balance);
            return this.clientId != -1;
        }

        private bool _UpdateClient()
        {
            return clsClientData.UpdateClient(clientId, accountNumber, personId, pinCode, balance);
        }

        public static clsClient FindClient(int ClientId)
        {
            int accNum = 0, personId = 0, pinCode = 0;
            double balance = 0;
            if (clsClientData.FindClient(ClientId, ref accNum, ref personId, ref pinCode, ref balance))
            {
                return new clsClient(ClientId, accNum, personId, pinCode, balance);
            }
            return null;
        }

        public static clsClient FindClientByAccNum(int accNum)
        {
            int clientId = 0, personId = 0, pinCode = 0;
            double balance = 0;
            if (clsClientData.FindClient(ref clientId, accNum, ref personId, ref pinCode, ref balance))
            {
                return new clsClient(clientId, accNum, personId, pinCode, balance);
            }
            return null;
        }

        public static bool DeleteClient(int ClientId)
        {
            return clsClientData.DeleteClient(ClientId);
        }

        public static DataView GetAllClients()
        {
            return clsClientData.GetAllClients().DefaultView;
        }

        public static bool IsClientExist(int ClientId)
        {
            return clsClientData.ClientIsExist(ClientId);
        }

        public static float GetBalanceByAccNum(int accNum)
        {
            return clsClientData.GetBalanceByAccountNumber(accNum);
        }

        public static float GetTotalBalances()
        {
            return clsClientData.GetTotalBalances();
        }

        public bool AddNewTransfer(int sAcc, int rAcc, double amount, double sBalance, double rBalance, int userId)
        {
            return clsClientData.AddNewTransfer(DateTime.Now, sAcc, rAcc, amount, sBalance, rBalance, userId);
        }

        public static DataView GetAllTransfers()
        {
            return clsClientData.GetAllTransfer().DefaultView;
        }

        public bool Save()
        {
            switch(mode)
            {
                case enMode.Add:
                    if(_AddNewClient())
                    {
                        mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateClient();
            }
            return false;
        }
    }
}
