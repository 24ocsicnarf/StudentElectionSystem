using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentElection.Dialogs
{
    public partial class PasswordInputForm: Form
    {
        public string Message
        {
            private get
            {
                return lblMessage.Text;
            }
            set
            {
                lblMessage.Text = value;
            }
        }

        public string Submessage
        {
            private get
            {
                return lblSubmessage.Text;
            }
            set
            {
                lblSubmessage.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return txtPassword.Text;
            }
            set
            {
                txtPassword.Text = value;
            }
        }

        public PasswordInputForm(string message, string submessage = null)
        {
            InitializeComponent();

            this.Message = message;
            this.Submessage = submessage;
        }

        private void PasswordInputForm_Load(object sender, EventArgs e)
        {
            Focus();

            txtPassword.Focus();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
