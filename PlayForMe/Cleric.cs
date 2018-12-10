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
    public partial class Cleric : Form
    {
        public String OpeningSKill;

        public Cleric()
        {
            InitializeComponent();
        }
        private void OpenSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = OpenSkill.Text;
        }
        private void Cleric_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
    }
}
