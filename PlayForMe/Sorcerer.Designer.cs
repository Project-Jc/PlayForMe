namespace Grievous
{
    partial class Sorcerer
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
            this.UseRoot = new System.Windows.Forms.CheckBox();
            this.UseFlameCage = new System.Windows.Forms.CheckBox();
            this.UseWinterBinding = new System.Windows.Forms.CheckBox();
            this.UseBlindLeap = new System.Windows.Forms.CheckBox();
            this.UseGainMana = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 16);
            this.label1.TabIndex = 28;
            this.label1.Text = "Open with skill";
            // 
            // OpenSkill
            // 
            this.OpenSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OpenSkill.FormattingEnabled = true;
            this.OpenSkill.Items.AddRange(new object[] {
            "Ice Chain",
            "Delayed Blast"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 39);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(225, 21);
            this.OpenSkill.TabIndex = 27;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.OpenSkill_SelectedIndexChanged);
            // 
            // UseRoot
            // 
            this.UseRoot.AutoSize = true;
            this.UseRoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseRoot.Location = new System.Drawing.Point(26, 76);
            this.UseRoot.Name = "UseRoot";
            this.UseRoot.Size = new System.Drawing.Size(77, 19);
            this.UseRoot.TabIndex = 29;
            this.UseRoot.Text = "Use Root";
            this.UseRoot.UseVisualStyleBackColor = true;
            // 
            // UseFlameCage
            // 
            this.UseFlameCage.AutoSize = true;
            this.UseFlameCage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseFlameCage.Location = new System.Drawing.Point(26, 101);
            this.UseFlameCage.Name = "UseFlameCage";
            this.UseFlameCage.Size = new System.Drawing.Size(118, 19);
            this.UseFlameCage.TabIndex = 30;
            this.UseFlameCage.Text = "Use Flame Cage";
            this.UseFlameCage.UseVisualStyleBackColor = true;
            // 
            // UseWinterBinding
            // 
            this.UseWinterBinding.AutoSize = true;
            this.UseWinterBinding.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseWinterBinding.Location = new System.Drawing.Point(26, 126);
            this.UseWinterBinding.Name = "UseWinterBinding";
            this.UseWinterBinding.Size = new System.Drawing.Size(131, 19);
            this.UseWinterBinding.TabIndex = 31;
            this.UseWinterBinding.Text = "Use Winter Binding";
            this.UseWinterBinding.UseVisualStyleBackColor = true;
            // 
            // UseBlindLeap
            // 
            this.UseBlindLeap.AutoSize = true;
            this.UseBlindLeap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseBlindLeap.Location = new System.Drawing.Point(26, 151);
            this.UseBlindLeap.Name = "UseBlindLeap";
            this.UseBlindLeap.Size = new System.Drawing.Size(110, 19);
            this.UseBlindLeap.TabIndex = 32;
            this.UseBlindLeap.Text = "Use Blind Leap";
            this.UseBlindLeap.UseVisualStyleBackColor = true;
            // 
            // UseGainMana
            // 
            this.UseGainMana.AutoSize = true;
            this.UseGainMana.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseGainMana.Location = new System.Drawing.Point(26, 176);
            this.UseGainMana.Name = "UseGainMana";
            this.UseGainMana.Size = new System.Drawing.Size(112, 19);
            this.UseGainMana.TabIndex = 33;
            this.UseGainMana.Text = "Use Gain Mana";
            this.UseGainMana.UseVisualStyleBackColor = true;
            // 
            // Sorcerer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.UseGainMana);
            this.Controls.Add(this.UseBlindLeap);
            this.Controls.Add(this.UseWinterBinding);
            this.Controls.Add(this.UseFlameCage);
            this.Controls.Add(this.UseRoot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Sorcerer";
            this.Text = "Sorcerer Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Sorcerer_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UseRoot;
        public System.Windows.Forms.CheckBox UseFlameCage;
        public System.Windows.Forms.CheckBox UseWinterBinding;
        public System.Windows.Forms.CheckBox UseBlindLeap;
        public System.Windows.Forms.CheckBox UseGainMana;
    }
}