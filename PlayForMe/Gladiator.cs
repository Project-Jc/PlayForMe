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
    public partial class Gladiator : Form
    {
        public string OpeningSKill;

        public Gladiator()
        {
            InitializeComponent();
        }
        private void Gladiator_OpenSkill_ddBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = OpenSkill.Text;
        }
        private void Gladiator_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
        private void Gladiator_Load(object sender, EventArgs e)
        {

        }
    }
}
