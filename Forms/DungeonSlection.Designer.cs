using System;
using System.ComponentModel;

namespace Deep.Forms
{
    partial class DungeonSelection
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
            this.startLevelBox = new System.Windows.Forms.CheckBox();
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
            this.FloorCombo.SelectedIndexChanged += new System.EventHandler(this.FloorCombo_SelectedIndexChanged);
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
            // startLevelBox
            // 
            this.startLevelBox.Location = new System.Drawing.Point(192, 13);
            this.startLevelBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startLevelBox.Name = "startLevelBox";
            this.startLevelBox.Size = new System.Drawing.Size(133, 44);
            this.startLevelBox.TabIndex = 4;
            this.startLevelBox.Text = "Start at checkpoint floor";
            this.startLevelBox.UseVisualStyleBackColor = true;
            this.startLevelBox.CheckedChanged += new System.EventHandler(this.startLevelBox_CheckedChanged);
            // 
            // SilverChest
            // 
            this.SilverChest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SilverChest.Location = new System.Drawing.Point(13, 113);
            this.SilverChest.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SilverChest.Name = "SilverChest";
            this.SilverChest.Size = new System.Drawing.Size(115, 33);
            this.SilverChest.TabIndex = 5;
            this.SilverChest.Text = "Silver Chests";
            this.SilverChest.UseVisualStyleBackColor = true;
            this.SilverChest.CheckedChanged += new System.EventHandler(this.SilverChest_CheckedChanged);
            // 
            // HordeCheck
            // 
            this.HordeCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HordeCheck.Location = new System.Drawing.Point(13, 138);
            this.HordeCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HordeCheck.Name = "HordeCheck";
            this.HordeCheck.Size = new System.Drawing.Size(115, 22);
            this.HordeCheck.TabIndex = 6;
            this.HordeCheck.Text = "Accursed Horde";
            this.HordeCheck.UseVisualStyleBackColor = true;
            this.HordeCheck.CheckedChanged += new System.EventHandler(this.HordeCheck_CheckedChanged);
            // 
            // DungeonSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 313);
            this.Controls.Add(this.HordeCheck);
            this.Controls.Add(this.SilverChest);
            this.Controls.Add(this.startLevelBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FloorCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DungeonListCombo);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DungeonSelection";
            this.Text = "DungeonSlection";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DungeonSelection_Closed);
            this.Load += new System.EventHandler(this.DungeonSelection_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DungeonListCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox FloorCombo;
        private System.Windows.Forms.CheckBox HordeCheck;
        private System.Windows.Forms.CheckBox SilverChest;
        private System.Windows.Forms.CheckBox startLevelBox;
    }
}