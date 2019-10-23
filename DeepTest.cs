﻿using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
 using Deep;
 using Deep.DungeonDefinition.Base;


 namespace Deep
{
    public partial class DeepTest : Form
    {
        public DeepTest()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // Constants.LoadList();
            comboBox1.DataSource = Constants.DeepListType;
            comboBox1.DisplayMember = "DisplayName";
            //comboBox1.ValueMember = "DungeonType";
        }
        
        private void changelevel(object sender, EventArgs e)
        {
            //Logger.Verbose("Changing the selected floor to run");
            //Constants.SelectedDungeon = (DeepDungeon) comboBox1.SelectedItem;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selected = comboBox1.SelectedItem;

            foreach (var id in (selected as IDeepDungeon).DeepDungeonRawIds)
            {
                listBox1.Items.Add(id);
            }

            //richTextBox1.Text += selected.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var selected = comboBox1.SelectedItem; 
                //Constants.deepListType.First(i => i.Index == ((IDeepDungeon) comboBox1.SelectedItem).Index);//(IDeepDungeon) comboBox1.SelectedItem;

            richTextBox1.Text = selected.ToString();

            listBox2.Items.Clear();
            
            foreach (var floor in (selected as IDeepDungeon).Floors)
            {
                listBox2.Items.Add(floor);
            }
            
            //richTextBox1.Text += "\n" + comboBox1.SelectedIndex ;
            
        }
    }
}