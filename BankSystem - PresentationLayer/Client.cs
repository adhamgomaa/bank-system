using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using BankSystemBusinessLayer;

namespace BankSystem___PresentationLayer
{
    public partial class Client : Form
    {
        DataView dv;
        public Client()
        {
            InitializeComponent();
        }
        private void _LoadClientData()
        {
            dv = clsClient.GetAllClients();
            dgvShowClients.DataSource = dv;
            lblClient.Text = dgvShowClients.RowCount.ToString() + " Client (s) Found";  
        }

        private void _ApplyFilters()
        {
            string keyword = txtAccountNumber.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dv.RowFilter = $"AccountNumber = {value}";
            }
            else
            {
                dv.RowFilter = "";
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblWelcome.Text = "Welcome: " + clsGlobal.CurrentUser.username;
            _LoadClientData();
            _fillInComboBox();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilters();
        }

        private void rbASC_CheckedChanged(object sender, EventArgs e)
        {
            dv.Sort = "AccountNumber";
        }

        private void rbDESC_CheckedChanged(object sender, EventArgs e)
        {
            dv.Sort = "AccountNumber DESC";
        }

        private void NumericBox_Validating(TextBox box, CancelEventArgs e)
        {
            if (!int.TryParse(box.Text, out int result))
            {
                e.Cancel = true;
                box.Focus();
                errorProvider1.SetError(box, "Please enter only numbers");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(box, "");
            }
        }

        private void NumericTexBox_Validating(object sender, CancelEventArgs e)
        {
            NumericBox_Validating((TextBox)sender, e);
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void Reset_TextBoxs()
        {
            txtAccNum.Text = "";
            txtCode.Text = "";
            numBalance.Value = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cbPersonId.SelectedIndex == -1)
            {
                return;
            }
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please Enter All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            clsClient newClient = new clsClient();
            newClient.accountNumber = Convert.ToInt32(txtAccNum.Text);
            newClient.pinCode = Convert.ToInt32(txtCode.Text);
            newClient.personId = Convert.ToInt32(cbPersonId.Text);
            newClient.balance = Convert.ToDouble(numBalance.Value);
            if(MessageBox.Show($"Are you sure to add a new client?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if(newClient.Save())
                {
                    MessageBox.Show($"The Client with ID = [{newClient.clientId}] Added Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientData();
                    _fillInComboBox();
                } else
                {
                    MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Reset_TextBoxs();
        }
        private void _fillInComboBox()
        {
            DataView dvPeople = clsPeople.GetAllPeople();
            cbPersonId.Items.Clear();
            cbClientId.Items.Clear();
            for (int i = 0; i < dvPeople.Count; i++)
                cbPersonId.Items.Add(dvPeople[i][0]);
            for (int i = 0; i < dv.Count; i++)
            {
                cbClientId.Items.Add(dv[i][0]);
            }
        }

        private void cbAccNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbClientId.SelectedIndex == -1)
            {
                return;
            }
            clsClient clientData = clsClient.FindClient(Convert.ToInt32(cbClientId.Text));
            if (clientData != null)
            {
                btnUpdate.Enabled = true;
                txtUpdatePersonId.Text = clientData.personInfo.personId.ToString();
                txtUpdateFirst.Text = clientData.personInfo.firstName;
                txtUpdateLast.Text = clientData.personInfo.lastName;
                txtUpdateSec.Text = clientData.personInfo.secondName;
                txtUpdateEmail.Text = clientData.personInfo.email;
                txtUpdatePhone.Text = clientData.personInfo.phone;
                if(clientData.personInfo.gender == 0)
                    rbUpdateMale.Enabled = true;
                else
                    rbUpdateFemale.Enabled = true;
                dtUpdateBirth.Value = clientData.personInfo.birthDate;
                txtUpdateAcc.Text = clientData.accountNumber.ToString();
                txtUpdateCode.Text = clientData.pinCode.ToString();
                numUpdateBalance.Value = (decimal)clientData.balance;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbClientId.SelectedIndex == -1)
            {
                return;
            }
            clsClient clientData = clsClient.FindClient(Convert.ToInt32(cbClientId.Text));
            if (clientData != null)
            {
                clientData.pinCode = Convert.ToInt32(txtUpdatePersonId.Text);
                clientData.accountNumber = Convert.ToInt32(txtUpdateAcc.Text);
                clientData.balance = Convert.ToDouble(numUpdateBalance.Value);
                if (MessageBox.Show($"Are you sure to update the client?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    if (clientData.Save())
                    {
                        MessageBox.Show($"The Client with ID = [{clientData.clientId}] updated Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _LoadClientData();
                        _fillInComboBox();
                    }
                    else
                    {
                        MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                btnUpdate.Enabled = false;
                cbClientId.SelectedIndex = -1;
                txtUpdateAcc.Text = "";
                txtUpdateCode.Text = "";
                numUpdateBalance.Value = 0;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure to Delete this client?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if (clsClient.DeleteClient((int)dgvShowClients.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show($"Client with ID = [{dgvShowClients.CurrentRow.Cells[0].Value}] Deleted Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientData();
                    _fillInComboBox();
                }
                else
                {
                    MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void depositeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction frm = new Transaction((int)dgvShowClients.CurrentRow.Cells[0].Value);
            frm.SelectedTabIndex = 0;
            frm.ShowDialog();
            _LoadClientData();
        }

        private void withdrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction frm = new Transaction((int)dgvShowClients.CurrentRow.Cells[0].Value);
            frm.SelectedTabIndex = 1;
            frm.ShowDialog();
            _LoadClientData();
        }

        private void transferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transaction frm = new Transaction((int)dgvShowClients.CurrentRow.Cells[0].Value);
            frm.SelectedTabIndex = 3;
            frm.ShowDialog();
            _LoadClientData();
        }

        private void cbPersonId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPersonId.SelectedIndex == -1)
            {
                return;
            }
            clsPeople person = clsPeople.FindPerson(Convert.ToInt32(cbPersonId.Text));
            txtFName.Text = person.firstName;
            txtLastName.Text = person.lastName;
            txtSecName.Text = person.secondName;
            txtEmail.Text = person.email;
            txtPhone.Text = person.phone;
            dtBirth.Value = person.birthDate;
            if (person.gender == 0)
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;
        }
    }
}
