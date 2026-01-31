using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemBusinessLayer
{
    public class TransactionEventArgs : EventArgs
    {
        public int accountNumber { get; }
        public double currentBalance { get; }
        public double transactionAmount { get; }
        public double newBalance { get; }

        public TransactionEventArgs(int accountNumber, double currentBalance, double transactionAmount)
        {
            this.accountNumber = accountNumber;
            this.currentBalance = currentBalance;
            this.transactionAmount = transactionAmount;
            newBalance = currentBalance + transactionAmount;
        }
    }
}
