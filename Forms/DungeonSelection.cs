using System;
using System.Windows.Forms;
using Deep.DungeonDefinition.Base;
using Deep.Helpers.Logging;

namespace Deep.Forms
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
            if (Constants.SelectedDungeon != null)
            {
                DungeonListCombo.SelectedItem = Constants.SelectedDungeon;
            }
            else
            {
                DungeonListCombo.SelectedItem = Constants.DeepListType[0];
            }

            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";


            startLevelBox.Checked = Settings.Instance.StartAt51;
            SilverChest.Checked = Settings.Instance.OpenSilver;
            HordeCheck.Checked = Settings.Instance.GoForTheHoard;

        }
        
        private void ChangeDungeon(object sender, EventArgs e)
        {
            Logger.Verbose("Changing the selected deep dungeon to run");
            Constants.SelectedDungeon = (IDeepDungeon) DungeonListCombo.SelectedItem;
            FloorCombo.DataSource = Constants.SelectedDungeon.Floors;
            DungeonListCombo.DisplayMember = "DisplayName";
            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";
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
            Settings.Instance.BetterSelectedLevel = (FloorSetting) FloorCombo.SelectedItem;
        }

        private void startLevelBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.StartAt51 = startLevelBox.Checked;
        }
    }
}