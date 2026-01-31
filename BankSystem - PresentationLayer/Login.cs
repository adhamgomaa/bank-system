using BankSystemBusinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BankSystem___PresentationLayer
{
    public partial class Login : Form
    {
        string path = "login.txt";
        short attempt = 3;
        clsUser _user;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            timer1.Start();
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Length >= 2)
                {
                    txtUserName.Text = lines[0];
                    txtPassword.Text = lines[1];
                    cbRemember.Checked = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
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
        }
        private void txtBox_Validating(object sender, CancelEventArgs e)
        {
            Box_Validating((TextBox)sender, e);
        }

        private void CheckLogin()
        {
            _user = clsUser.FindUser(txtUserName.Text, txtPassword.Text);
            if (_user != null)
            {
                if (_user.isActive)
                {
                    attempt = 3;
                    lblinvalid.Visible = false;
                    lblAttempts.Visible = false;
                    clsGlobal.CurrentUser = _user;
                    if(_user.AddNewRegister())
                    {
                        Home home = new Home();
                        home.Show();
                        this.Hide();
                        home.FormClosed += (s, args) => {
                            if (home.isLogout)
                            {
                                this.Show();
                            }
                            else
                            {
                                Application.Exit();
                            }
                        };
                    }
                }
                else
                {
                    MessageBox.Show("Your account is deactivated, please contact your admin", "Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                attempt--;
                lblinvalid.Visible = true;
                lblAttempts.Visible = true;
                if (attempt == 0)
                {
                    btnLogin.Enabled = false;
                    lblinvalid.Text = "You Are Locked After 3 Faild Trails!!";
                    lblAttempts.Text = "Contact System Adminstrator To Unlock Your Account";
                    return;
                }
                lblAttempts.Text = "You have " + attempt + " attempts before lock your account";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(!this.ValidateChildren())
            {
                MessageBox.Show("Please Enter All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string username = txtUserName.Text;
            string password = txtPassword.Text;
            if (cbRemember.Checked)
            {
                File.WriteAllText(path, $"{username}\n{password}");
            }
            else
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            CheckLogin();
        }
    }
}
