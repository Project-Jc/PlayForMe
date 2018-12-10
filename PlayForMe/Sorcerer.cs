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
    public partial class Sorcerer : Form
    {
        public string OpeningSkill;

        public Sorcerer()
        {
            InitializeComponent();
        }
        private void OpenSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSkill = OpenSkill.Text;
        }
        private void Sorcerer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
    }
}
