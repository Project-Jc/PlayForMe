namespace Grievous
{
    partial class Cleric
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.OpenSkill = new System.Windows.Forms.ComboBox();
            this.UseSmite = new System.Windows.Forms.CheckBox();
            this.UseHolyServant = new System.Windows.Forms.CheckBox();
            this.UsePenance = new System.Windows.Forms.CheckBox();
            this.UseLightofRenewal = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "Open with skill";
            // 
            // OpenSkill
            // 
            this.OpenSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OpenSkill.FormattingEnabled = true;
            this.OpenSkill.Items.AddRange(new object[] {
            "Smite",
            "Storm of Aion"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 36);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(225, 21);
            this.OpenSkill.TabIndex = 25;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.OpenSkill_SelectedIndexChanged);
            // 
            // UseSmite
            // 
            this.UseSmite.AutoSize = true;
            this.UseSmite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseSmite.Location = new System.Drawing.Point(30, 73);
            this.UseSmite.Name = "UseSmite";
            this.UseSmite.Size = new System.Drawing.Size(83, 19);
            this.UseSmite.TabIndex = 27;
            this.UseSmite.TabStop = false;
            this.UseSmite.Text = "Use Smite";
            this.UseSmite.UseVisualStyleBackColor = true;
            // 
            // UseHolyServant
            // 
            this.UseHolyServant.AutoSize = true;
            this.UseHolyServant.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseHolyServant.Location = new System.Drawing.Point(30, 98);
            this.UseHolyServant.Name = "UseHolyServant";
            this.UseHolyServant.Size = new System.Drawing.Size(119, 19);
            this.UseHolyServant.TabIndex = 28;
            this.UseHolyServant.TabStop = false;
            this.UseHolyServant.Text = "Use Holy Servant";
            this.UseHolyServant.UseVisualStyleBackColor = true;
            // 
            // UsePenance
            // 
            this.UsePenance.AutoSize = true;
            this.UsePenance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsePenance.Location = new System.Drawing.Point(30, 123);
            this.UsePenance.Name = "UsePenance";
            this.UsePenance.Size = new System.Drawing.Size(100, 19);
            this.UsePenance.TabIndex = 29;
            this.UsePenance.TabStop = false;
            this.UsePenance.Text = "Use Penance";
            this.UsePenance.UseVisualStyleBackColor = true;
            // 
            // UseLightofRenewal
            // 
            this.UseLightofRenewal.AutoSize = true;
            this.UseLightofRenewal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseLightofRenewal.Location = new System.Drawing.Point(30, 148);
            this.UseLightofRenewal.Name = "UseLightofRenewal";
            this.UseLightofRenewal.Size = new System.Drawing.Size(154, 19);
            this.UseLightofRenewal.TabIndex = 30;
            this.UseLightofRenewal.TabStop = false;
            this.UseLightofRenewal.Text = "Prefer Light of Renewal";
            this.UseLightofRenewal.UseVisualStyleBackColor = true;
            // 
            // Cleric
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.UseLightofRenewal);
            this.Controls.Add(this.UsePenance);
            this.Controls.Add(this.UseHolyServant);
            this.Controls.Add(this.UseSmite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Cleric";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Cleric configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Cleric_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UseSmite;
        public System.Windows.Forms.CheckBox UseHolyServant;
        public System.Windows.Forms.CheckBox UsePenance;
        public System.Windows.Forms.CheckBox UseLightofRenewal;
    }
}