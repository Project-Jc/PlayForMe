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
    public partial class Assassin : Form
    {
        public string OpeningSKill;

        public Assassin()
        {
            InitializeComponent();
        }
        private void Assassin_OpenSkill_ddBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = OpenSkill.Text;
        }
        private void Assassin_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
    }
}
