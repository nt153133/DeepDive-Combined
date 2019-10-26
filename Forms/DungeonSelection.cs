using System;
using System.Linq;
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
                Settings.Instance.BetterSelectedLevel = (FloorSetting) FloorCombo.SelectedItem;
            }

            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";


            FloorCombo.SelectionChangeCommitted += ChangeLevel;
            startLevelBox.Checked = Settings.Instance.StartAt51;
            SilverChest.Checked = Settings.Instance.OpenSilver;
            HordeCheck.Checked = Settings.Instance.GoForTheHoard;
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
                Settings.Instance.BetterSelectedLevel = (FloorSetting) FloorCombo.SelectedItem;
            }

            startLevelBox.Text = $"Start at floor {Constants.SelectedDungeon.CheckPointLevel}";
        }

        private void ChangeLevel(object sender, EventArgs e)
        {
            Logger.Verbose("Changing the selected floor to run");
            Settings.Instance.BetterSelectedLevel = (FloorSetting) FloorCombo.SelectedItem;
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
    }
}