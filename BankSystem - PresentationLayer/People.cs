using BankSystemBusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace BankSystem___PresentationLayer
{
    public partial class People : Form
    {
        DataView dv;
        public People()
        {
            InitializeComponent();
        }
        private void _LoadClientData()
        {
            dv = clsPeople.GetAllPeople();
            dgvShowPeople.DataSource = dv;
            lblPerson.Text = dgvShowPeople.RowCount.ToString() + " Client (s) Found";
        }

        private void _ApplyFilters()
        {
            string keyword = txtId.Text.Trim();
            if (int.TryParse(keyword, out int value))
            {
                dv.RowFilter = $"PersonID = {value}";
            }
            else
            {
                dv.RowFilter = "";
            }
        }

        private void People_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblWelcome.Text = "Welcome: " + clsGlobal.CurrentUser.username;
            dtBirth.MaxDate = DateTime.Today.AddYears(-21);
            dtBirth.MinDate = DateTime.Today.AddYears(-80);
            _LoadClientData();
            _fillAllAccountNumberInComboBox();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilters();
        }

        private void rbASC_CheckedChanged(object sender, EventArgs e)
        {
            dv.Sort = "PersonID";
        }

        private void rbDESC_CheckedChanged_1(object sender, EventArgs e)
        {
            dv.Sort = "PersonID DESC";
        }

        private void Box_Validating(TextBox box, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(box, "Required");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(box, "");
            }
            if (box == txtEmail || box == txtUpdateEmail)
            {
                if (!IsValidEmail(box.Text))
                {
                    e.Cancel = true;
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

        private void Reset_TextBoxs()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtSecName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Please Enter All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            clsPeople newPerson = new clsPeople();
            newPerson.firstName = txtFirstName.Text.Trim();
            newPerson.secondName = txtSecName.Text.Trim();
            newPerson.lastName = txtLastName.Text.Trim();
            newPerson.email = txtEmail.Text.Trim();
            newPerson.phone = txtPhone.Text.Trim();
            if (rbMale.Checked)
                newPerson.gender = 0;
            else
                newPerson.gender = 1;
            newPerson.birthDate = dtBirth.Value;
            if (MessageBox.Show($"Are you sure to add person?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                if (newPerson.Save())
                {
                    MessageBox.Show($"Person with ID = [{newPerson.personId}] Added Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _LoadClientData();
                    _fillAllAccountNumberInComboBox();
                }
                else
                {
                    MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Reset_TextBoxs();
        }

        private void _fillAllAccountNumberInComboBox()
        {
            cbPersonId.Items.Clear();
            for (int i = 0; i < dv.Count; i++)
            {
                cbPersonId.Items.Add(dv[i][0]);
            }
        }

        private void cbPersonId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPersonId.SelectedIndex == -1)
            {
                return;
            }
            clsPeople PersonData = clsPeople.FindPerson(Convert.ToInt32(cbPersonId.Text));
            if (PersonData != null)
            {
                btnUpdate.Enabled = true;
                txtUpdateFName.Text = PersonData.firstName;
                txtUpdateSec.Text = PersonData.secondName;
                txtUpdateLast.Text = PersonData.lastName;
                txtUpdateEmail.Text = PersonData.email;
                txtUpdatePhone.Text = PersonData.phone;
                if (PersonData.gender == 0)
                    rbUpdateMale.Checked = true;
                else
                    rbUpdateFemale.Checked = true;
                dtUpdateBirth.Value = PersonData.birthDate;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbPersonId.SelectedIndex == -1)
            {
                return;
            }
            clsPeople PersonData = clsPeople.FindPerson(Convert.ToInt32(cbPersonId.Text));
            if (PersonData != null)
            {
                PersonData.firstName = txtUpdateFName.Text;
                PersonData.secondName = txtUpdateSec.Text;
                PersonData.lastName = txtUpdateLast.Text;
                PersonData.email = txtUpdateEmail.Text;
                PersonData.phone = txtUpdatePhone.Text;
                if(rbUpdateMale.Checked)
                    PersonData.gender = 0;
                else
                    PersonData.gender = 1;
                PersonData.birthDate = dtUpdateBirth.Value;
                if (MessageBox.Show($"Are you sure to update person?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    if (PersonData.Save())
                    {
                        MessageBox.Show($"Person with ID = [{PersonData.personId}] updated Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _LoadClientData();
                    }
                    else
                    {
                        MessageBox.Show("Error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                btnUpdate.Enabled = false;
                cbPersonId.SelectedIndex = -1;
                txtUpdateFName.Text = "";
                txtUpdateSec.Text = "";
                txtUpdateLast.Text = "";
                txtUpdateEmail.Text = "";
                txtUpdatePhone.Text = "";
            }
        }
    }
}
