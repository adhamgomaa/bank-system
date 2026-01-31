using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankSystemBusinessLayer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BankSystem___PresentationLayer
{
    public partial class Transaction : Form
    {
        DataView dvClients;
        DataView dvTransfer;
        int accNum = -1;
        public event EventHandler<clsClient> OnTransactionChanged;
        public Transaction(int accNum)
        {
            InitializeComponent();
            this.accNum = accNum;
        }

        public Transaction()
        {
            InitializeComponent();
        }

        public int SelectedTabIndex
        {
            set {
                if (value >= 0 && value < tabControl1.TabCount)
                {
                    tabControl1.SelectedIndex = value;
                }
            }

            get 
            { 
                return tabControl1.SelectedIndex;
            }
        }

        private void _SelectClient()
        {
            if (this.SelectedTabIndex == 0)
            {
                if (cbAccNumDeposite.Items.Contains(accNum))
                {
                    cbAccNumDeposite.SelectedIndex = cbAccNumDeposite.FindString(accNum.ToString());
                }
            }

            if (this.SelectedTabIndex == 1)
            {
                if (cbAccNumWithdraw.Items.Contains(accNum))
                {
                    cbAccNumWithdraw.SelectedIndex = cbAccNumWithdraw.FindString(accNum.ToString());
                }
            }

            if (this.SelectedTabIndex == 3)
            {
                if (cbAccFrom.Items.Contains(accNum))
                {
                    cbAccFrom.SelectedIndex = cbAccFrom.FindString(accNum.ToString());
                }
            }
        }
             
        private void _fillAllAccountNumberInComboBox()
        {
            if(dvClients.Count == 0)
            {
                MessageBox.Show("Please enter some client to open this window", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cbAccNumDeposite.Items.Clear();
            cbAccNumWithdraw.Items.Clear();
            cbAccFrom.Items.Clear();
            for (int i = 0; i < dvClients.Count; i++)
            {
                cbAccNumDeposite.Items.Add(dvClients[i][1]);
                cbAccNumWithdraw.Items.Add(dvClients[i][1]);
                cbAccFrom.Items.Add(dvClients[i][1]);
            }
        }

        private void _LoadTransferLog()
        {
            dvTransfer = clsClient.GetAllTransfers();
            dgvTransfer.DataSource = dvTransfer;
            lblNumClients.Text = dgvTransfer.Rows.Count.ToString() + " Client (s) Found";
        }

        private void _ApplyFilters()
        {
            string keyword = txtSearch.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dvTransfer.RowFilter = $"SenderAcc = {value}";
            }
            else
            {
                dvTransfer.RowFilter = "";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilters();
        }

        private void rbSortAsc_CheckedChanged(object sender, EventArgs e)
        {
            dvTransfer.Sort = "SenderAcc";
        }

        private void rbSortDesc_CheckedChanged(object sender, EventArgs e)
        {
            dvTransfer.Sort = "SenderAcc Desc";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
        }

        private void Transaction_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblWelcome.Text = "Welcome: " + clsGlobal.CurrentUser.username;
            _LoadClientDataInTotalBalance();
            _fillAllAccountNumberInComboBox();
            _SelectClient();
            _LoadTransferLog();
        }

        private void cbAccNumDeposite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbAccNumDeposite.SelectedIndex == -1)
            {
                lblCurrentBalance.Visible = false;
                lblBalance.Visible = false;
                return;
            }
            lblCurrentBalance.Visible = true;
            lblBalance.Text = "$ " + clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccNumDeposite.Text)).ToString();
            lblBalance.Visible = true;
        }

        private void btnDeposite_Click(object sender, EventArgs e)
        {
            if(cbAccNumDeposite.SelectedIndex == -1)
            {
                return;
            }
            clsClient updateBalance = clsClient.FindClientByAccNum(Convert.ToInt32(cbAccNumDeposite.Text));
            if (updateBalance != null)
            {
                updateBalance.TransactionChanged += HandleDepositeTransaction;
                if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    updateBalance.Deposit(Convert.ToDouble(numDeposite.Value));
                    _LoadClientDataInTotalBalance();
                }
            }
            cbAccNumDeposite.SelectedIndex = -1;
            numDeposite.Value = 0;
        }

        private void HandleDepositeTransaction(object sender, TransactionEventArgs e)
        {
            MessageBox.Show($"Client With AccountNumber {e.accountNumber} Deposited Amount with value ${-e.transactionAmount}, then your balance become ${e.newBalance} Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbAccNumWithdraw_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAccNumWithdraw.SelectedIndex == -1)
            {
                lblCurrentBalanceWithdraw.Visible = false;
                lblBalanceWithdraw.Visible = false;
                return;
            }
            decimal balance = (decimal)clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccNumWithdraw.Text));
            lblCurrentBalanceWithdraw.Visible = true;
            lblBalanceWithdraw.Text = "$ " + balance.ToString();
            lblBalanceWithdraw.Visible = true;
            numWithdraw.Maximum = balance;
            numWithdraw.Value = numWithdraw.Maximum;
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (cbAccNumWithdraw.SelectedIndex == -1)
            {
                return;
            }
            clsClient updateBalance = clsClient.FindClientByAccNum(Convert.ToInt32(cbAccNumWithdraw.Text));
            if (updateBalance != null)
            {
                updateBalance.TransactionChanged += HandleWithdrawTransaction;
                if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    if(!updateBalance.Withdraw(Convert.ToDouble(numWithdraw.Value)))
                    {
                        MessageBox.Show("Your balance is not enough", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _LoadClientDataInTotalBalance();
                }
            }
            cbAccNumWithdraw.SelectedIndex = -1;
            numWithdraw.Value = 0;
        }

        private void HandleWithdrawTransaction(object sender, TransactionEventArgs e)
        {
            MessageBox.Show($"Client With AccountNumber {e.accountNumber} Withrawed Amount with value ${e.transactionAmount}, then your balance become ${e.newBalance} Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void _LoadClientDataInTotalBalance()
        {
            dvClients = clsClient.GetAllClients();
            dgvShowClients.DataSource = dvClients;
            txtBalance.Text = "($" + clsClient.GetTotalBalances().ToString() + ")";
            lblClients.Text = dgvShowClients.RowCount.ToString() + " Client (s) Found";
        }

        private void _ApplyFiltersForTotalBalance()
        {
            string keyword = txtAccNum.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dvClients.RowFilter = $"AccountNumber = {value}";
            }
            else
            {
                dvClients.RowFilter = "";
            }
        }

        private void rbASC_CheckedChanged(object sender, EventArgs e)
        {
            dvClients.Sort = "AccountNumber";
        }

        private void rbDESC_CheckedChanged(object sender, EventArgs e)
        {
            dvClients.Sort = "AccountNumber DESC";
        }

        private void txtAccNum_TextChanged(object sender, EventArgs e)
        {
            _ApplyFiltersForTotalBalance();
        }

        private void cbAccFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal balanceFrom = (decimal)clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccFrom.Text.Trim()));
            if (cbAccFrom.SelectedIndex == -1)
            {
                cbAccTo.SelectedIndex = -1;
                cbAccTo.Enabled = false;
                lblCurrentFrom.Visible = false;
                lblBalanceFrom.Visible = false;
                return;
            }
            if ((int)balanceFrom == 0)
            {
                MessageBox.Show($"Cannot Transfer From Account Number ({cbAccFrom.Text}) Balance is $0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbAccFrom.SelectedIndex = -1;
                return;
            }

            lblCurrentTo.Visible = false;
            lblBalanceTo.Visible = false;
            cbAccTo.Enabled = true;
            lblCurrentFrom.Visible = true;
            lblBalanceFrom.Text = "$ " + balanceFrom.ToString();
            lblBalanceFrom.Visible = true;
            numTransfer.Maximum = (int)balanceFrom;
            numTransfer.Value = numTransfer.Maximum;
            cbAccTo.Items.Clear();
            for (int i = 0; i < cbAccFrom.Items.Count; i++)
            {
                if(cbAccFrom.SelectedIndex != i)
                {
                    cbAccTo.Items.Add(cbAccFrom.Items[i]);
                }
            }

        }

        private void cbAccTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAccTo.SelectedIndex == -1)
            {
                lblCurrentTo.Visible = false;
                lblBalanceTo.Visible = false;
                return;
            }
            lblCurrentTo.Visible = true;
            lblBalanceTo.Text = "$ " + clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccTo.Text)).ToString();
            lblBalanceTo.Visible = true;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (cbAccFrom.SelectedIndex == -1 || cbAccTo.SelectedIndex == -1)
            {
                return;
            }
            clsClient FromClient = clsClient.FindClientByAccNum(Convert.ToInt32(cbAccFrom.Text));
            clsClient ToClient = clsClient.FindClientByAccNum(Convert.ToInt32(cbAccTo.Text));
            if (FromClient != null && ToClient != null)
            {
                FromClient.balance -= Convert.ToDouble(numTransfer.Value);
                ToClient.balance += Convert.ToDouble(numTransfer.Value);
                if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    if (FromClient.Save() && ToClient.Save())
                    {
                        MessageBox.Show($"Transfer Done Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _LoadClientDataInTotalBalance();
                        FromClient.AddNewTransfer(FromClient.accountNumber, ToClient.accountNumber, Convert.ToDouble(numTransfer.Value), FromClient.balance, ToClient.balance, clsGlobal.CurrentUser.userId);
                        _LoadTransferLog();
                    }
                    else
                    {
                        MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            cbAccFrom.SelectedIndex = -1;
            numTransfer.Value = 0;
        } 
    }
}
