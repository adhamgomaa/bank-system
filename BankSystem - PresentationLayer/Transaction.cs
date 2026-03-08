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
            if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if(clsClient.Deposit(Convert.ToInt32(cbAccNumDeposite.Text), Convert.ToDecimal(numDeposite.Value)))
                {
                    MessageBox.Show($"Client With AccountNumber {cbAccNumDeposite.Text} Deposited Amount with value ${numDeposite.Value}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientDataInTotalBalance();
                }
            }
            cbAccNumDeposite.SelectedIndex = -1;
            numDeposite.Value = 0;
        }

        private void cbAccNumWithdraw_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAccNumWithdraw.SelectedIndex == -1)
            {
                lblCurrentBalanceWithdraw.Visible = false;
                lblBalanceWithdraw.Visible = false;
                return;
            }
            decimal balance = clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccNumWithdraw.Text));
            lblCurrentBalanceWithdraw.Visible = true;
            lblBalanceWithdraw.Text = "$ " + balance.ToString();
            lblBalanceWithdraw.Visible = true;
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (cbAccNumWithdraw.SelectedIndex == -1)
            {
                return;
            }
            if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if(clsClient.Withdrawal(Convert.ToInt32(cbAccNumWithdraw.Text), Convert.ToDecimal(numWithdraw.Value)))
                {
                    MessageBox.Show($"Client With AccountNumber {cbAccNumWithdraw.Text} Withrawed Amount with value ${numWithdraw.Value}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientDataInTotalBalance();
                } else
                {
                    MessageBox.Show($"Insufficient balance, your balance is lower than the amount", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            cbAccNumWithdraw.SelectedIndex = -1;
            numWithdraw.Value = 0;
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
            if (cbAccFrom.SelectedIndex == -1)
            {
                cbAccTo.SelectedIndex = -1;
                cbAccTo.Enabled = false;
                lblCurrentFrom.Visible = false;
                lblBalanceFrom.Visible = false;
                return;
            }
            decimal balanceFrom = clsClient.GetBalanceByAccNum(Convert.ToInt32(cbAccFrom.Text.Trim()));
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
            if (MessageBox.Show($"Are you sure to preform this transaction?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if(clsClient.AddNewTransfer(Convert.ToInt32(cbAccFrom.Text), Convert.ToInt32(cbAccTo.Text), Convert.ToDecimal(numTransfer.Value), clsGlobal.CurrentUser.userId))
                {
                    MessageBox.Show($"Transfer Done Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientDataInTotalBalance();
                    _LoadTransferLog();
                }
                else
                {
                    MessageBox.Show("Insufficient balance, your balance is lower than the amount", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            cbAccFrom.SelectedIndex = -1;
            numTransfer.Value = 0;
        } 
    }
}
