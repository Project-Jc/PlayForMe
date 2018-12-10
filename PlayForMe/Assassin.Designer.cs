namespace Grievous
{
    partial class Assassin
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
            this.UsePoison = new System.Windows.Forms.CheckBox();
            this.UseSurpriseAttack = new System.Windows.Forms.CheckBox();
            this.UseDevotion = new System.Windows.Forms.CheckBox();
            this.UseFlurry = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 9);
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
            "Attack N Dash"});
            this.OpenSkill.Location = new System.Drawing.Point(12, 38);
            this.OpenSkill.Name = "OpenSkill";
            this.OpenSkill.Size = new System.Drawing.Size(225, 21);
            this.OpenSkill.TabIndex = 25;
            this.OpenSkill.SelectedIndexChanged += new System.EventHandler(this.Assassin_OpenSkill_ddBox_SelectedIndexChanged);
            // 
            // UsePoison
            // 
            this.UsePoison.AutoSize = true;
            this.UsePoison.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsePoison.Location = new System.Drawing.Point(27, 74);
            this.UsePoison.Name = "UsePoison";
            this.UsePoison.Size = new System.Drawing.Size(95, 19);
            this.UsePoison.TabIndex = 27;
            this.UsePoison.Text = "Use Poisons";
            this.UsePoison.UseVisualStyleBackColor = true;
            // 
            // UseSurpriseAttack
            // 
            this.UseSurpriseAttack.AutoSize = true;
            this.UseSurpriseAttack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseSurpriseAttack.Location = new System.Drawing.Point(27, 149);
            this.UseSurpriseAttack.Name = "UseSurpriseAttack";
            this.UseSurpriseAttack.Size = new System.Drawing.Size(132, 19);
            this.UseSurpriseAttack.TabIndex = 28;
            this.UseSurpriseAttack.Text = "Use Surprise Attack";
            this.UseSurpriseAttack.UseVisualStyleBackColor = true;
            // 
            // UseDevotion
            // 
            this.UseDevotion.AutoSize = true;
            this.UseDevotion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseDevotion.Location = new System.Drawing.Point(27, 99);
            this.UseDevotion.Name = "UseDevotion";
            this.UseDevotion.Size = new System.Drawing.Size(99, 19);
            this.UseDevotion.TabIndex = 30;
            this.UseDevotion.Text = "Use Devotion";
            this.UseDevotion.UseVisualStyleBackColor = true;
            // 
            // UseFlurry
            // 
            this.UseFlurry.AutoSize = true;
            this.UseFlurry.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UseFlurry.Location = new System.Drawing.Point(27, 124);
            this.UseFlurry.Name = "UseFlurry";
            this.UseFlurry.Size = new System.Drawing.Size(81, 19);
            this.UseFlurry.TabIndex = 31;
            this.UseFlurry.Text = "Use Flurry";
            this.UseFlurry.UseVisualStyleBackColor = true;
            // 
            // Assassin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 276);
            this.Controls.Add(this.UseFlurry);
            this.Controls.Add(this.UseDevotion);
            this.Controls.Add(this.UseSurpriseAttack);
            this.Controls.Add(this.UsePoison);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenSkill);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Assassin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Assassin Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Assassin_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox OpenSkill;
        public System.Windows.Forms.CheckBox UsePoison;
        public System.Windows.Forms.CheckBox UseSurpriseAttack;
        public System.Windows.Forms.CheckBox UseDevotion;
        public System.Windows.Forms.CheckBox UseFlurry;
    }
}