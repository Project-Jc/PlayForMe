namespace Grievous
{
    partial class Ranger
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
            this.UseDevotion = new System.Windows.Forms.CheckBox();
            this.UseDeadShot = new System.Windows.Forms.CheckBox();
            this.UseAiming = new System.Windows.Forms.CheckBox();
            this.UseDodging = new System.Windows.Forms.CheckBox();
            this.UseMauForm = new System.Windows.Forms.CheckBox();
            this.UseStrongShots = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 9);
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
            "Attack",
            "Entangling Shot"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 37);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(225, 21);
            this.OpenSkill.TabIndex = 27;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.OpenSkill_SelectedIndexChanged);
            // 
            // UseDevotion
            // 
            this.UseDevotion.AutoSize = true;
            this.UseDevotion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseDevotion.Location = new System.Drawing.Point(30, 73);
            this.UseDevotion.Name = "UseDevotion";
            this.UseDevotion.Size = new System.Drawing.Size(99, 19);
            this.UseDevotion.TabIndex = 29;
            this.UseDevotion.TabStop = false;
            this.UseDevotion.Text = "Use Devotion";
            this.UseDevotion.UseVisualStyleBackColor = true;
            // 
            // UseDeadShot
            // 
            this.UseDeadShot.AutoSize = true;
            this.UseDeadShot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseDeadShot.Location = new System.Drawing.Point(30, 98);
            this.UseDeadShot.Name = "UseDeadShot";
            this.UseDeadShot.Size = new System.Drawing.Size(109, 19);
            this.UseDeadShot.TabIndex = 30;
            this.UseDeadShot.TabStop = false;
            this.UseDeadShot.Text = "Use Dead Shot";
            this.UseDeadShot.UseVisualStyleBackColor = true;
            // 
            // UseAiming
            // 
            this.UseAiming.AutoSize = true;
            this.UseAiming.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseAiming.Location = new System.Drawing.Point(30, 123);
            this.UseAiming.Name = "UseAiming";
            this.UseAiming.Size = new System.Drawing.Size(89, 19);
            this.UseAiming.TabIndex = 31;
            this.UseAiming.TabStop = false;
            this.UseAiming.Text = "Use Aiming";
            this.UseAiming.UseVisualStyleBackColor = true;
            // 
            // UseDodging
            // 
            this.UseDodging.AutoSize = true;
            this.UseDodging.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseDodging.Location = new System.Drawing.Point(30, 148);
            this.UseDodging.Name = "UseDodging";
            this.UseDodging.Size = new System.Drawing.Size(98, 19);
            this.UseDodging.TabIndex = 32;
            this.UseDodging.TabStop = false;
            this.UseDodging.Text = "Use Dodging";
            this.UseDodging.UseVisualStyleBackColor = true;
            // 
            // UseMauForm
            // 
            this.UseMauForm.AutoSize = true;
            this.UseMauForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseMauForm.Location = new System.Drawing.Point(30, 173);
            this.UseMauForm.Name = "UseMauForm";
            this.UseMauForm.Size = new System.Drawing.Size(108, 19);
            this.UseMauForm.TabIndex = 33;
            this.UseMauForm.TabStop = false;
            this.UseMauForm.Text = "Use Mau Form";
            this.UseMauForm.UseVisualStyleBackColor = true;
            // 
            // UseStrongShots
            // 
            this.UseStrongShots.AutoSize = true;
            this.UseStrongShots.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseStrongShots.Location = new System.Drawing.Point(30, 198);
            this.UseStrongShots.Name = "UseStrongShots";
            this.UseStrongShots.Size = new System.Drawing.Size(121, 19);
            this.UseStrongShots.TabIndex = 34;
            this.UseStrongShots.TabStop = false;
            this.UseStrongShots.Text = "Use Strong Shots";
            this.UseStrongShots.UseVisualStyleBackColor = true;
            // 
            // Ranger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.UseStrongShots);
            this.Controls.Add(this.UseMauForm);
            this.Controls.Add(this.UseDodging);
            this.Controls.Add(this.UseAiming);
            this.Controls.Add(this.UseDeadShot);
            this.Controls.Add(this.UseDevotion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Ranger";
            this.Text = "Ranger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Ranger_FormClosing);
            this.Load += new System.EventHandler(this.Ranger_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UseDevotion;
        public System.Windows.Forms.CheckBox UseDeadShot;
        public System.Windows.Forms.CheckBox UseAiming;
        public System.Windows.Forms.CheckBox UseDodging;
        public System.Windows.Forms.CheckBox UseMauForm;
        public System.Windows.Forms.CheckBox UseStrongShots;
    }
}