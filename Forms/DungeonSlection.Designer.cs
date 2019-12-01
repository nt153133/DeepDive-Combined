using System;
using System.ComponentModel;

namespace DeepCombined.Forms
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
            this.StopCheck = new System.Windows.Forms.CheckBox();
            this.classesCB = new System.Windows.Forms.ComboBox();
            this.addClassBtn = new System.Windows.Forms.Button();
            this.levelTxt = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DungeonListCombo
            // 
            this.DungeonListCombo.FormattingEnabled = true;
            this.DungeonListCombo.Location = new System.Drawing.Point(12, 24);
            this.DungeonListCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DungeonListCombo.Name = "DungeonListCombo";
            this.DungeonListCombo.Size = new System.Drawing.Size(193, 23);
            this.DungeonListCombo.TabIndex = 0;
            this.DungeonListCombo.SelectedIndexChanged += new System.EventHandler(this.DungeonListCombo_SelectedIndexChanged);
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
            this.FloorCombo.Size = new System.Drawing.Size(317, 23);
            this.FloorCombo.TabIndex = 2;
            this.FloorCombo.SelectedIndexChanged += new System.EventHandler(this.FloorCombo_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Profile";
            // 
            // startLevelBox
            // 
            this.startLevelBox.Location = new System.Drawing.Point(214, 13);
            this.startLevelBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startLevelBox.Name = "startLevelBox";
            this.startLevelBox.Size = new System.Drawing.Size(115, 44);
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
            // StopCheck
            // 
            this.StopCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StopCheck.Location = new System.Drawing.Point(174, 120);
            this.StopCheck.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StopCheck.Name = "StopCheck";
            this.StopCheck.Size = new System.Drawing.Size(111, 18);
            this.StopCheck.TabIndex = 7;
            this.StopCheck.Text = "Stop Run";
            this.StopCheck.UseVisualStyleBackColor = true;
            this.StopCheck.CheckedChanged += new System.EventHandler(this.StopCheck_CheckedChanged);
            // 
            // classesCB
            // 
            this.classesCB.FormattingEnabled = true;
            this.classesCB.Location = new System.Drawing.Point(13, 175);
            this.classesCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.classesCB.Name = "classesCB";
            this.classesCB.Size = new System.Drawing.Size(164, 23);
            this.classesCB.TabIndex = 8;
            this.classesCB.SelectedIndexChanged += new System.EventHandler(this.classesCB_SelectedIndexChanged);
            // 
            // addClassBtn
            // 
            this.addClassBtn.Location = new System.Drawing.Point(245, 175);
            this.addClassBtn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.addClassBtn.Name = "addClassBtn";
            this.addClassBtn.Size = new System.Drawing.Size(83, 22);
            this.addClassBtn.TabIndex = 9;
            this.addClassBtn.Text = "Add";
            this.addClassBtn.UseVisualStyleBackColor = true;
            this.addClassBtn.Click += new System.EventHandler(this.addClassBtn_Click);
            // 
            // levelTxt
            // 
            this.levelTxt.Location = new System.Drawing.Point(191, 174);
            this.levelTxt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.levelTxt.Name = "levelTxt";
            this.levelTxt.Size = new System.Drawing.Size(44, 23);
            this.levelTxt.TabIndex = 10;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(13, 207);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(222, 79);
            this.listBox1.TabIndex = 11;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(245, 207);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(80, 21);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.Location = new System.Drawing.Point(174, 140);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(111, 18);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Use Job List";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // DungeonSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 297);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.levelTxt);
            this.Controls.Add(this.addClassBtn);
            this.Controls.Add(this.classesCB);
            this.Controls.Add(this.StopCheck);
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
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DungeonListCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox FloorCombo;
        private System.Windows.Forms.CheckBox HordeCheck;
        private System.Windows.Forms.CheckBox SilverChest;
        private System.Windows.Forms.CheckBox startLevelBox;
        private System.Windows.Forms.CheckBox StopCheck;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox levelTxt;
        private System.Windows.Forms.Button addClassBtn;
        private System.Windows.Forms.ComboBox classesCB;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}