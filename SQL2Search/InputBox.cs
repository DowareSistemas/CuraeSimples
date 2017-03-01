using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQL2Search
{
    public partial class InputBox : Form
    {
        public InputBox(string labelText)
        {
            InitializeComponent();

            lbText.Text = labelText;
            this.ShowDialog();
        }

        public string Value
        {
            get
            {
                return txValue.Text;
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Close();
        }
    }
}
