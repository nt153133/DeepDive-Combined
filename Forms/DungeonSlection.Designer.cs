using System;
using System.ComponentModel;

namespace Deep.Forms
{
    partial class DungeonSlection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.DungeonListCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.FloorCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SilverChest = new System.Windows.Forms.CheckBox();
            this.HordeCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DungeonListCombo
            // 
            this.DungeonListCombo.FormattingEnabled = true;
            this.DungeonListCombo.Location = new System.Drawing.Point(12, 24);
            this.DungeonListCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DungeonListCombo.Name = "DungeonListCombo";
            this.DungeonListCombo.Size = new System.Drawing.Size(162, 23);
            this.DungeonListCombo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Floor";
            // 
            // FloorCombo
            // 
            this.FloorCombo.FormattingEnabled = true;
            this.FloorCombo.Location = new System.Drawing.Point(12, 75);
            this.FloorCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FloorCombo.Name = "FloorCombo";
            this.FloorCombo.Size = new System.Drawing.Size(300, 23);
            this.FloorCombo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 22);
            this.label2.TabIndex = 3;
            this.label2.Text = "Profile";
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(178, 13);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(133, 44);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Start at checkpoint (Floor 51/21)";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // SilverChest
            // 
            this.SilverChest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SilverChest.Location = new System.Drawing.Point(12, 113);
            this.SilverChest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SilverChest.Name = "SilverChest";
            this.SilverChest.Size = new System.Drawing.Size(105, 33);
            this.SilverChest.TabIndex = 5;
            this.SilverChest.Text = "Silver Chests";
            this.SilverChest.UseVisualStyleBackColor = true;
            this.SilverChest.CheckedChanged += new EventHandler(this.SilverChest_CheckedChanged);
            // 
            // HordeCheck
            // 
            this.HordeCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HordeCheck.Location = new System.Drawing.Point(12, 138);
            this.HordeCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HordeCheck.Name = "HordeCheck";
            this.HordeCheck.Size = new System.Drawing.Size(105, 33);
            this.HordeCheck.TabIndex = 6;
            this.HordeCheck.Text = "Silver Chests";
            this.HordeCheck.UseVisualStyleBackColor = true;
            this.HordeCheck.CheckedChanged += new System.EventHandler(this.HordeCheck_CheckedChanged);
            // 
            // DungeonSlection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 313);
            this.Controls.Add(this.HordeCheck);
            this.Controls.Add(this.SilverChest);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FloorCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DungeonListCombo);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DungeonSlection";
            this.Text = "DungeonSlection";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DungeonSlection_Closed);
            this.Load += new System.EventHandler(this.DungeonSlection_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DungeonListCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox FloorCombo;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox HordeCheck;
        private System.Windows.Forms.CheckBox SilverChest;
    }
}