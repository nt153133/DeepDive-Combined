using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DeepCombined.Helpers;
using DeepCombined.DungeonDefinition.Base;
using DeepCombined.Helpers.Logging;
using DeepCombined.Structure;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using FloorSetting = DeepCombined.DungeonDefinition.Base.FloorSetting;

namespace DeepCombined.Forms
{
    public partial class DungeonSelection : Form
    {
        public DungeonSelection()
        {
            InitializeComponent();
        }

        private void DungeonSelection_Load(object sender, EventArgs e)
        {
            DungeonListCombo.DataSource = Constants.DeepListType;
            DungeonListCombo.DisplayMember = "DisplayName";

            DungeonListCombo.SelectionChangeCommitted += ChangeDungeon;
            DungeonListCombo.SelectedIndex = Settings.Instance.SelectedDungeon;


            if (Constants.SelectedDungeon != null)
                DungeonListCombo.SelectedItem = Constants.SelectedDungeon;
            else
                DungeonListCombo.SelectedItem = Constants.DeepListType[0];

            FloorCombo.DataSource = Constants.SelectedDungeon.Floors;
            FloorCombo.DisplayMember = "DisplayName";

            if (Settings.Instance.BetterSelectedLevel != null) FloorCombo.SelectedItem = Constants.SelectedDungeon.Floors.FirstOrDefault(i => i.Equals(Settings.Instance.BetterSelectedLevel));

            if (FloorCombo.SelectedItem == null)
            {
                FloorCombo.SelectedItem = Constants.SelectedDungeon.Floors[0];
                Settings.Instance.BetterSelectedLevel = (DungeonDefinition.Base.FloorSetting) FloorCombo.SelectedItem;
            }

            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";
            var list = new List<KeyValuePair<string, int>>();
            foreach (var gs in GearsetManager.GearSets.Where(i=> i.InUse && i.Class.IsDow()))
            {
                list.Add(new KeyValuePair<string, int>(gs.Class.ToString(),gs.Index));
            }
            
            classesCB.DataSource = list;
            
            
            //classesCB.ValueMember = "Value";
            classesCB.DisplayMember = "Key";
            
            listBox1.DataSource = Constants.ClassLevelTargets;
            listBox1.DisplayMember = "DisplayString";
            

            FloorCombo.SelectionChangeCommitted += ChangeLevel;
            startLevelBox.Checked = Settings.Instance.StartAt51;
            SilverChest.Checked = Settings.Instance.OpenSilver;
            HordeCheck.Checked = Settings.Instance.GoForTheHoard;
            checkBox2.Checked = Settings.Instance.GoExit;

            checkBox1.Checked = Constants.UseJobList;
        }

        private void ChangeDungeon(object sender, EventArgs e)
        {
            Logger.Verbose("Changing the selected deep dungeon to run");
            Constants.SelectedDungeon = (IDeepDungeon) DungeonListCombo.SelectedItem;
            FloorCombo.DataSource = Constants.SelectedDungeon.Floors;
            if (Settings.Instance.BetterSelectedLevel != null) FloorCombo.SelectedItem = Constants.SelectedDungeon.Floors.FirstOrDefault(i => i.Equals(Settings.Instance.BetterSelectedLevel));
            if (FloorCombo.SelectedItem == null)
            {
                FloorCombo.SelectedItem = Constants.SelectedDungeon.Floors[0];
                Settings.Instance.BetterSelectedLevel = (DungeonDefinition.Base.FloorSetting) FloorCombo.SelectedItem;
            }

            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";
        }

        private void ChangeLevel(object sender, EventArgs e)
        {
            Logger.Verbose("Changing the selected floor to run");
            Settings.Instance.BetterSelectedLevel = (DungeonDefinition.Base.FloorSetting) FloorCombo.SelectedItem;
        }

        private void DungeonSelection_Closed(object sender, FormClosedEventArgs e)
        {
            DungeonListCombo.SelectionChangeCommitted -= ChangeDungeon;
        }

        private void SilverChest_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.OpenSilver = SilverChest.Checked;
        }

        private void HordeCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.GoForTheHoard = HordeCheck.Checked;
        }

        private void FloorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Logger.Verbose("FloorCombo_SelectedIndexChanged");
            //Settings.Instance.BetterSelectedLevel = (FloorSetting) FloorCombo.SelectedItem;
        }

        private void startLevelBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.StartAt51 = startLevelBox.Checked;
        }

        private void StopCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.Stop = StopCheck.Checked;
        }

        private void DungeonListCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Instance.SelectedDungeon = DungeonListCombo.SelectedIndex;
        }

        private void addClassBtn_Click(object sender, EventArgs e)
        {
            int level = int.Parse(levelTxt.Text.Trim());
            ClassJobType test;
            Enum.TryParse(((KeyValuePair<string, int>)classesCB.SelectedValue).Key,out test);
            var classTarget = new ClassLevelTarget(test, level,((KeyValuePair<string, int>)classesCB.SelectedValue).Value);
            Logger.Info($"{classTarget}");
            Constants.ClassLevelTargets.Add(classTarget);
            listBox1.Refresh();
            checkBox1.Checked = true;
        }

        private void classesCB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            Constants.ClassLevelTargets = null;
            Constants.ClassLevelTargets = new BindingList<ClassLevelTarget>();
            listBox1.DataSource = Constants.ClassLevelTargets;
            listBox1.ResetBindings();
            checkBox1.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Constants.UseJobList = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.GoExit = checkBox2.Checked;
        }
    }
}