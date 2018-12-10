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
    public partial class Spiritmaster : Form
    {
        public string OpeningSKill;

        public Spiritmaster()
        {
            InitializeComponent();
        }
        private void Spiritmaster_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
        private void Sorcerer_OpenSkill_ddBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = Sorcerer_OpenSkill_ddBox.Text;
        }
    }
}
