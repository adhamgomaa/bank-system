using BankSystemBusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace BankSystem___PresentationLayer
{
    public partial class Users : Form
    {
        short permission = -1;
        DataView dvUsers;
        DataView dvRegisters;
        public Users()
        {
            InitializeComponent();
        }

        private void _LoadUserData()
        {
            dvUsers = clsUser.GetAllUsers();
            dgvShowUsers.DataSource = dvUsers;
            lblUser.Text = dgvShowUsers.RowCount.ToString() + " User (s) Found";
        }

        private void _LoadUserRegister()
        {
            dvRegisters = clsUser.GetAllRegisters();
            dgvRegister.DataSource = dvRegisters;
            lblNumRegister.Text = dgvRegister.Rows.Count.ToString() + " User (s) Found";
        }

        private void Users_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblWelcome.Text = "Welcome: " + clsGlobal.CurrentUser.username;
            _LoadUserData();
            _fillAllUserIdInComboBox();
            _LoadUserRegister();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
        }

        private void rbASC_CheckedChanged(object sender, EventArgs e)
        {
            dvUsers.Sort = "UserName";
        }

        private void rbDESC_CheckedChanged(object sender, EventArgs e)
        {
            dvUsers.Sort = "UserName DESC";
        }

        private void rbSortAsc_CheckedChanged(object sender, EventArgs e)
        {
            dvRegisters.Sort = "UserName";
        }

        private void rbSortDesc_CheckedChanged(object sender, EventArgs e)
        {
            dvRegisters.Sort = "UserName Desc";
        }

        private void _ApplyFilters()
        {
            string keyword = txtSearchUser.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dvUsers.RowFilter = $"AccountNumber = {value}";
            }
            else
            {
                dvUsers.RowFilter = "";
            }
        }

        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilters();
        }
        private void _ApplyFiltersRegisters()
        {
            string keyword = txtSearch.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dvRegisters.RowFilter = $"UserID = {value}";
            }
            else
            {
                dvRegisters.RowFilter = "";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _ApplyFiltersRegisters();
        }

        private void Box_Validating(TextBox box, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                e.Cancel = true;
                box.Focus();
                errorProvider1.SetError(box, "Required");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(box, "");
            }

            if(clsUser.FindUser(txtUserName.Text.Trim()) != null)
            {
                e.Cancel = true;
                box.Focus();
                errorProvider1.SetError(box, "Required");
            }else
            {
                e.Cancel = false;
                errorProvider1.SetError(box, "");
            }

            if (box == txtEmail)
            {
                if (!IsValidEmail(box.Text))
                {
                    e.Cancel = true;
                    box.Focus();
                    errorProvider1.SetError(box, "Invalid email, include an '@' in the email address");
                }
            }
        }

        private void txtBox_Validating(object sender, CancelEventArgs e)
        {
            Box_Validating((TextBox)sender, e);
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void _ResetAll()
        {
            txtUserName.Text = "";
            txtPass.Text = "";
            permission = 0;
            rbYesAccess.Checked = false;
            rbNoAccess.Checked = false;
            boxPeople.Enabled = false;
            boxClients.Enabled = false;
            boxTransaction.Enabled = false;
            boxUser.Enabled = false;
            boxClients.Checked = false;
            boxTransaction.Checked = false;
            boxUser.Checked = false;
            boxPeople.Checked = false;
            cbUserName.SelectedIndex = -1;
            txtUpdatePass.Text = "";
            rbUpdateYesAccess.Checked = false;
            rbUpdateNoAccess.Checked = false;
            boxUpdateClients.Enabled = false;
            boxUpdatePeople.Enabled = false;
            boxUpdateTransaction.Enabled = false;
            boxUpdateUser.Enabled = false;
            boxUpdateClients.Checked = false;
            boxUpdatePeople.Checked = false;
            boxUpdateTransaction.Checked = false;
            boxUpdateUser.Checked = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please Enter All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            clsUser newUser = new clsUser();
            newUser.username = txtUserName.Text;
            newUser.password = txtPass.Text;
            newUser.permissions = permission;
            newUser.personId = Convert.ToInt32(cbPersonId.Text);
            newUser.isActive = true;
            if (MessageBox.Show($"Are you sure to add user?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if (newUser.Save())
                {
                    MessageBox.Show($"User with ID = [{newUser.userId}] Added Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadUserData();
                    _fillAllUserIdInComboBox();
                }
                else
                {
                    MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _ResetAll();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            boxPeople.Enabled = false;
            boxClients.Enabled = false;
            boxTransaction.Enabled = false;
            boxUser.Enabled = false;
            this.permission = -1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            boxPeople.Enabled = true;
            boxClients.Enabled = true;
            boxTransaction.Enabled = true;
            boxUser.Enabled = true;
            this.permission = 0;
        }

        private void rbYes_CheckedChanged(object sender, EventArgs e)
        {
            boxUpdateClients.Enabled = false;
            boxUpdatePeople.Enabled = false;
            boxUpdateTransaction.Enabled = false;
            boxUpdateUser.Enabled = false;
            this.permission = -1;
        }

        private void rbNo_CheckedChanged(object sender, EventArgs e)
        {
            boxUpdateClients.Enabled = true;
            boxUpdatePeople.Enabled = true;
            boxUpdateTransaction.Enabled = true;
            boxUpdateUser.Enabled = true;
            this.permission = 0;
        }

        private void permissionCheckBoxes(CheckBox box, EventArgs e)
        {
            if (box.Checked)
                this.permission += Convert.ToInt16(box.Tag);
            else
                this.permission -= Convert.ToInt16(box.Tag);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            permissionCheckBoxes((CheckBox)sender, e);
        }

        private void _fillAllUserIdInComboBox()
        {
            DataView dvPeople = clsPeople.GetAllPeople();
            cbPersonId.Items.Clear();
            cbUserName.Items.Clear();
            for (int i = 0; i < dvPeople.Count; i++)
                cbPersonId.Items.Add(dvPeople[i][0]);

            for (int i = 0; i < dvUsers.Count; i++)
            {
                cbUserName.Items.Add(dvUsers[i][2]);
            }
        }

        private void cbUserId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUserName.SelectedIndex == -1)
                return;
            clsUser user = clsUser.FindUser(cbUserName.Text);
            if (user != null)
            {
                btnUpdate.Enabled = true;
                txtUpdatePersonId.Text = user.personId.ToString();
                txtUpdateFirst.Text = user.personInfo.firstName;
                txtUpdateSec.Text = user.personInfo.secondName;
                txtUpdateLast.Text = user.personInfo.lastName;
                txtUpdateEmail.Text = user.personInfo.email;
                txtUpdatePhone.Text = user.personInfo.phone;
                txtUpdatePass.Text = user.password;
                dtUpdateBirth.Value = user.personInfo.birthDate;

                if(user.personInfo.gender == 0)
                    rbUpdateMale.Checked = true;
                else
                    rbUpdateFemale.Checked = true;

                if(user.isActive)
                    rbYesActive.Checked = true;
                else
                    rbNoAtive.Checked = true;

                if (user.permissions == -1)
                    rbUpdateYesAccess.Checked = true;
                else
                    rbUpdateNoAccess.Checked = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbUserName.SelectedIndex == -1)
                return;
            clsUser userData = clsUser.FindUser(cbUserName.Text);
            if (userData != null)
            {
                
                userData.password = txtUpdatePass.Text;
                userData.permissions = this.permission;
                userData.isActive = rbYesActive.Checked;
                if (MessageBox.Show($"Are you sure to update user [{userData.username}]?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    if (userData.Save())
                    {
                        MessageBox.Show($"User [{userData.username}] updated Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _LoadUserData();
                    }
                    else
                    {
                        MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            _ResetAll();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure to Delete User [{dgvShowUsers.CurrentRow.Cells[2].Value}]?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if (clsUser.DeleteUser((int)dgvShowUsers.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show($"User [{dgvShowUsers.CurrentRow.Cells[2].Value}] Deleted Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadUserData();
                    _fillAllUserIdInComboBox();
                }
                else
                {
                    MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
