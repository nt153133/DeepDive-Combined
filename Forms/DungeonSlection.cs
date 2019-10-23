using System;
using System.Windows.Forms;
using Deep.DungeonDefinition.Base;
using Deep.Helpers.Logging;

namespace Deep.Forms
{
    public partial class DungeonSlection : Form
    {
        public DungeonSlection()
        {
            InitializeComponent();
        }

        private void DungeonSlection_Load(object sender, EventArgs e)
        {
            DungeonListCombo.DataSource = Constants.DeepListType;
            DungeonListCombo.DisplayMember = "DisplayName";
            DungeonListCombo.SelectionChangeCommitted += changelevel;
            if (Constants.SelectedDungeon != null)
            {
                DungeonListCombo.SelectedItem = Constants.SelectedDungeon;
            }
            else
            {
                DungeonListCombo.SelectedItem = Constants.DeepListType[0];
            }

            SilverChest.Checked = Settings.Instance.OpenSilver;
            HordeCheck.Checked = Settings.Instance.GoForTheHoard;

        }
        
        private void changelevel(object sender, EventArgs e)
        {
            Logger.Verbose("Changing the selected deep dungeon to run");
            Constants.SelectedDungeon = (IDeepDungeon) DungeonListCombo.SelectedItem;
            FloorCombo.DataSource = Constants.SelectedDungeon.Floors;
            DungeonListCombo.DisplayMember = "DisplayName";
        }
        
        private void DungeonSlection_Closed(object sender, FormClosedEventArgs e)
        {
            
            DungeonListCombo.SelectionChangeCommitted -= changelevel;
        }


        private void SilverChest_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.OpenSilver = SilverChest.Checked;
        }


        private void HordeCheck_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance.GoForTheHoard = HordeCheck.Checked;
        }
    }
}