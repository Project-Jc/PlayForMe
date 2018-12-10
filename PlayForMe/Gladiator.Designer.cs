namespace Grievous
{
    partial class Gladiator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gladiator));
            this.UseSeismicWave = new System.Windows.Forms.CheckBox();
            this.PreferWSB = new System.Windows.Forms.CheckBox();
            this.UseImprovedStamina = new System.Windows.Forms.CheckBox();
            this.OpenSkill = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UseUnwaveringDevotion = new System.Windows.Forms.CheckBox();
            this.UseWeakeningBlow = new System.Windows.Forms.CheckBox();
            this.UseDaevicFury = new System.Windows.Forms.CheckBox();
            this.UsePiercingWave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // UseSeismicWave
            // 
            this.UseSeismicWave.AutoSize = true;
            this.UseSeismicWave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseSeismicWave.Location = new System.Drawing.Point(26, 75);
            this.UseSeismicWave.Name = "UseSeismicWave";
            this.UseSeismicWave.Size = new System.Drawing.Size(128, 19);
            this.UseSeismicWave.TabIndex = 12;
            this.UseSeismicWave.Text = "Use Seismic Wave";
            this.UseSeismicWave.UseVisualStyleBackColor = true;
            // 
            // PreferWSB
            // 
            this.PreferWSB.AutoSize = true;
            this.PreferWSB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreferWSB.Location = new System.Drawing.Point(26, 101);
            this.PreferWSB.Name = "PreferWSB";
            this.PreferWSB.Size = new System.Drawing.Size(130, 19);
            this.PreferWSB.TabIndex = 16;
            this.PreferWSB.Text = "Prefer Severe Blow";
            this.PreferWSB.UseVisualStyleBackColor = true;
            // 
            // UseImprovedStamina
            // 
            this.UseImprovedStamina.AutoSize = true;
            this.UseImprovedStamina.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseImprovedStamina.Location = new System.Drawing.Point(26, 127);
            this.UseImprovedStamina.Name = "UseImprovedStamina";
            this.UseImprovedStamina.Size = new System.Drawing.Size(151, 19);
            this.UseImprovedStamina.TabIndex = 17;
            this.UseImprovedStamina.Text = "Use Improved Stamina";
            this.UseImprovedStamina.UseVisualStyleBackColor = true;
            // 
            // OpenSkill
            // 
            this.OpenSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OpenSkill.FormattingEnabled = true;
            this.OpenSkill.Items.AddRange(new object[] {
            "Attack",
            "Cleave",
            "Taunt"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 38);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(201, 21);
            this.OpenSkill.TabIndex = 20;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.Gladiator_OpenSkill_ddBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Open with skill";
            // 
            // UseUnwaveringDevotion
            // 
            this.UseUnwaveringDevotion.AutoSize = true;
            this.UseUnwaveringDevotion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseUnwaveringDevotion.Location = new System.Drawing.Point(26, 177);
            this.UseUnwaveringDevotion.Name = "UseUnwaveringDevotion";
            this.UseUnwaveringDevotion.Size = new System.Drawing.Size(167, 19);
            this.UseUnwaveringDevotion.TabIndex = 24;
            this.UseUnwaveringDevotion.Text = "Use Unwavering Devotion";
            this.UseUnwaveringDevotion.UseVisualStyleBackColor = true;
            // 
            // UseWeakeningBlow
            // 
            this.UseWeakeningBlow.AutoSize = true;
            this.UseWeakeningBlow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseWeakeningBlow.Location = new System.Drawing.Point(26, 202);
            this.UseWeakeningBlow.Name = "UseWeakeningBlow";
            this.UseWeakeningBlow.Size = new System.Drawing.Size(143, 19);
            this.UseWeakeningBlow.TabIndex = 25;
            this.UseWeakeningBlow.Text = "Use Weakening Blow";
            this.UseWeakeningBlow.UseVisualStyleBackColor = true;
            // 
            // UseDaevicFury
            // 
            this.UseDaevicFury.AutoSize = true;
            this.UseDaevicFury.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseDaevicFury.Location = new System.Drawing.Point(26, 227);
            this.UseDaevicFury.Name = "UseDaevicFury";
            this.UseDaevicFury.Size = new System.Drawing.Size(114, 19);
            this.UseDaevicFury.TabIndex = 26;
            this.UseDaevicFury.Text = "Use Daevic Fury";
            this.UseDaevicFury.UseVisualStyleBackColor = true;
            // 
            // UsePiercingWave
            // 
            this.UsePiercingWave.AutoSize = true;
            this.UsePiercingWave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsePiercingWave.Location = new System.Drawing.Point(26, 152);
            this.UsePiercingWave.Name = "UsePiercingWave";
            this.UsePiercingWave.Size = new System.Drawing.Size(129, 19);
            this.UsePiercingWave.TabIndex = 27;
            this.UsePiercingWave.Text = "Use Piercing Wave";
            this.UsePiercingWave.UseVisualStyleBackColor = true;
            // 
            // Gladiator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.UsePiercingWave);
            this.Controls.Add(this.UseDaevicFury);
            this.Controls.Add(this.UseWeakeningBlow);
            this.Controls.Add(this.UseUnwaveringDevotion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.Controls.Add(this.UseImprovedStamina);
            this.Controls.Add(this.PreferWSB);
            this.Controls.Add(this.UseSeismicWave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Gladiator";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Gladiator configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Gladiator_FormClosing);
            this.Load += new System.EventHandler(this.Gladiator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        public System.Windows.Forms.CheckBox UseSeismicWave;
        public System.Windows.Forms.CheckBox PreferWSB;
        public System.Windows.Forms.CheckBox UseImprovedStamina;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UseUnwaveringDevotion;
        public System.Windows.Forms.CheckBox UseWeakeningBlow;
        public System.Windows.Forms.CheckBox UseDaevicFury;
        public System.Windows.Forms.CheckBox UsePiercingWave;
    }
}