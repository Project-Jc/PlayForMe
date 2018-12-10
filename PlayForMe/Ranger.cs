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
    public partial class Ranger : Form
    {
        public String OpeningSKill;

        public Ranger()
        {
            InitializeComponent();
        }
        private void Ranger_Load(object sender, EventArgs e)
        {

        }
        private void OpenSkill_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpeningSKill = OpenSkill.Text;
        }
        private void Ranger_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
    }
}
