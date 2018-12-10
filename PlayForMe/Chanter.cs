using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Grievous
{
    public partial class Chanter : Form
    {
        public String OpeningSKill;

        public Chanter()
        {
            InitializeComponent();
        }
        private void Chanter_OpenSkill_ddBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = OpenSkill.Text;
        }
        private void Chanter_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }

        private void Chanter_Load(object sender, EventArgs e)
        {
            UseAethericField.Enabled = false;
        }
    }
}
