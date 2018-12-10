namespace Grievous
{
    partial class Spiritmaster
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
            this.Sorcerer_OpenSkill_ddBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SpiritMaster_StoneSkin = new System.Windows.Forms.CheckBox();
            this.SpiritMaster_TBC = new System.Windows.Forms.CheckBox();
            this.SpiritMaster_UseRoot = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Sorcerer_OpenSkill_ddBox
            // 
            this.Sorcerer_OpenSkill_ddBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Sorcerer_OpenSkill_ddBox.FormattingEnabled = true;
            this.Sorcerer_OpenSkill_ddBox.Items.AddRange(new object[] {
            "Ice Chain",
            "Pet Attack"});
            this.Sorcerer_OpenSkill_ddBox.Location = new System.Drawing.Point(12, 38);
            this.Sorcerer_OpenSkill_ddBox.Name = "Sorcerer_OpenSkill_ddBox";
            this.Sorcerer_OpenSkill_ddBox.Size = new System.Drawing.Size(225, 21);
            this.Sorcerer_OpenSkill_ddBox.TabIndex = 22;
            this.Sorcerer_OpenSkill_ddBox.SelectedIndexChanged += new System.EventHandler(this.Sorcerer_OpenSkill_ddBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Open with skill";
            // 
            // SpiritMaster_StoneSkin
            // 
            this.SpiritMaster_StoneSkin.AutoSize = true;
            this.SpiritMaster_StoneSkin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpiritMaster_StoneSkin.Location = new System.Drawing.Point(42, 75);
            this.SpiritMaster_StoneSkin.Name = "SpiritMaster_StoneSkin";
            this.SpiritMaster_StoneSkin.Size = new System.Drawing.Size(110, 19);
            this.SpiritMaster_StoneSkin.TabIndex = 26;
            this.SpiritMaster_StoneSkin.Text = "Use Stone Skin";
            this.SpiritMaster_StoneSkin.UseVisualStyleBackColor = true;
            // 
            // SpiritMaster_TBC
            // 
            this.SpiritMaster_TBC.AutoSize = true;
            this.SpiritMaster_TBC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpiritMaster_TBC.Location = new System.Drawing.Point(42, 100);
            this.SpiritMaster_TBC.Name = "SpiritMaster_TBC";
            this.SpiritMaster_TBC.Size = new System.Drawing.Size(147, 19);
            this.SpiritMaster_TBC.TabIndex = 27;
            this.SpiritMaster_TBC.Text = "Use Thunderbolt Claw";
            this.SpiritMaster_TBC.UseVisualStyleBackColor = true;
            // 
            // SpiritMaster_UseRoot
            // 
            this.SpiritMaster_UseRoot.AutoSize = true;
            this.SpiritMaster_UseRoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpiritMaster_UseRoot.Location = new System.Drawing.Point(42, 125);
            this.SpiritMaster_UseRoot.Name = "SpiritMaster_UseRoot";
            this.SpiritMaster_UseRoot.Size = new System.Drawing.Size(77, 19);
            this.SpiritMaster_UseRoot.TabIndex = 28;
            this.SpiritMaster_UseRoot.Text = "Use Root";
            this.SpiritMaster_UseRoot.UseVisualStyleBackColor = true;
            // 
            // Spiritmaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.SpiritMaster_UseRoot);
            this.Controls.Add(this.SpiritMaster_TBC);
            this.Controls.Add(this.SpiritMaster_StoneSkin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Sorcerer_OpenSkill_ddBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Spiritmaster";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Spiritmaster configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Spiritmaster_FormClosing_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox Sorcerer_OpenSkill_ddBox;
        public System.Windows.Forms.CheckBox SpiritMaster_StoneSkin;
        public System.Windows.Forms.CheckBox SpiritMaster_TBC;
        public System.Windows.Forms.CheckBox SpiritMaster_UseRoot;
    }
}