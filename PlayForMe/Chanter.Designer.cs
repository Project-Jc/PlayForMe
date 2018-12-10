namespace Grievous
{
    partial class Chanter
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
            this.UseRageSpell = new System.Windows.Forms.CheckBox();
            this.UseProtectiveWard = new System.Windows.Forms.CheckBox();
            this.UseRecoverySpell = new System.Windows.Forms.CheckBox();
            this.UseWordofWind = new System.Windows.Forms.CheckBox();
            this.UseAethericField = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 16);
            this.label1.TabIndex = 24;
            this.label1.Text = "Open with skill";
            // 
            // OpenSkill
            // 
            this.OpenSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OpenSkill.FormattingEnabled = true;
            this.OpenSkill.Items.AddRange(new object[] {
            "Smite",
            "Attack"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 37);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(225, 21);
            this.OpenSkill.TabIndex = 23;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.Chanter_OpenSkill_ddBox_SelectedIndexChanged);
            // 
            // UseRageSpell
            // 
            this.UseRageSpell.AutoSize = true;
            this.UseRageSpell.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseRageSpell.Location = new System.Drawing.Point(25, 76);
            this.UseRageSpell.Name = "UseRageSpell";
            this.UseRageSpell.Size = new System.Drawing.Size(112, 19);
            this.UseRageSpell.TabIndex = 29;
            this.UseRageSpell.Text = "Use Rage Spell";
            this.UseRageSpell.UseVisualStyleBackColor = true;
            // 
            // UseProtectiveWard
            // 
            this.UseProtectiveWard.AutoSize = true;
            this.UseProtectiveWard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseProtectiveWard.Location = new System.Drawing.Point(25, 101);
            this.UseProtectiveWard.Name = "UseProtectiveWard";
            this.UseProtectiveWard.Size = new System.Drawing.Size(136, 19);
            this.UseProtectiveWard.TabIndex = 30;
            this.UseProtectiveWard.Text = "Use Protective Ward";
            this.UseProtectiveWard.UseVisualStyleBackColor = true;
            // 
            // UseRecoverySpell
            // 
            this.UseRecoverySpell.AutoSize = true;
            this.UseRecoverySpell.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseRecoverySpell.Location = new System.Drawing.Point(25, 126);
            this.UseRecoverySpell.Name = "UseRecoverySpell";
            this.UseRecoverySpell.Size = new System.Drawing.Size(132, 19);
            this.UseRecoverySpell.TabIndex = 31;
            this.UseRecoverySpell.Text = "Use Recovery Spell";
            this.UseRecoverySpell.UseVisualStyleBackColor = true;
            // 
            // UseWordofWind
            // 
            this.UseWordofWind.AutoSize = true;
            this.UseWordofWind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseWordofWind.Location = new System.Drawing.Point(25, 151);
            this.UseWordofWind.Name = "UseWordofWind";
            this.UseWordofWind.Size = new System.Drawing.Size(124, 19);
            this.UseWordofWind.TabIndex = 32;
            this.UseWordofWind.Text = "Use Word of Wind";
            this.UseWordofWind.UseVisualStyleBackColor = true;
            // 
            // UseAethericField
            // 
            this.UseAethericField.AutoSize = true;
            this.UseAethericField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseAethericField.Location = new System.Drawing.Point(25, 176);
            this.UseAethericField.Name = "UseAethericField";
            this.UseAethericField.Size = new System.Drawing.Size(125, 19);
            this.UseAethericField.TabIndex = 33;
            this.UseAethericField.Text = "Use Aetheric Field";
            this.UseAethericField.UseVisualStyleBackColor = true;
            // 
            // Chanter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 256);
            this.Controls.Add(this.UseAethericField);
            this.Controls.Add(this.UseWordofWind);
            this.Controls.Add(this.UseRecoverySpell);
            this.Controls.Add(this.UseProtectiveWard);
            this.Controls.Add(this.UseRageSpell);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Chanter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Chanter configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chanter_FormClosing);
            this.Load += new System.EventHandler(this.Chanter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UseRageSpell;
        public System.Windows.Forms.CheckBox UseProtectiveWard;
        public System.Windows.Forms.CheckBox UseRecoverySpell;
        public System.Windows.Forms.CheckBox UseWordofWind;
        public System.Windows.Forms.CheckBox UseAethericField;
    }
}