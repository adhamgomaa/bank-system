using BankSystemBusinessLayer;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankSystem___PresentationLayer
{
    public partial class Home : Form
    {
        public bool isLogout = false;
        public Home()
        {
            InitializeComponent();
        }

        private void btnClients_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            client.ShowDialog();
        }

        private void btnTransactions_Click(object sender, EventArgs e)
        {
            Transaction transaction = new Transaction();
            transaction.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            users.ShowDialog();
        }

        private void checkPermissions()
        {
            Guna2Button[] buttonList = splitContainer1.Panel2.Controls.OfType<Guna2Button>().ToArray();
            if(clsGlobal.CurrentUser.permissions == -1)
            {
                btnClients.Visible = true;
                btnTransactions.Visible = true;
                btnUsers.Visible = true;
                btnPeople.Visible = true;
            } else
            {
                foreach (Guna2Button button in buttonList)
                {
                    if(Convert.ToBoolean(clsGlobal.CurrentUser.permissions & Convert.ToInt16(button.Tag)))
                    {
                        button.Visible = true;
                    } else
                    {
                        button.Visible = false;
                    }
                }
            }
        }

        private void Home_Load(object sender, EventArgs e)
        {
            timer1.Start();
            lblWelcome.Text = "Welcome: " +  clsGlobal.CurrentUser.username;
            checkPermissions();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("F");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clsGlobal.CurrentUser = null;
            isLogout = true;
            this.Close();
        }

        private void btnPeople_Click(object sender, EventArgs e)
        {
            People people = new People();
            people.ShowDialog();
        }
    }
}
