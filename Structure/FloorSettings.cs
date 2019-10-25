/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System.ComponentModel;
using Deep.Properties;
using Newtonsoft.Json;

namespace Deep.Structure
{
    internal class FloorSetting : INotifyPropertyChanged
    {
        /// <summary>
        ///     represents the highest level we before we reach the aether value.
        /// </summary>
        [JsonProperty("_levelMax")]
        public int LevelMax { get; set; }

        [JsonIgnore] public string Display => string.Format(Resources.UI_FloorSettingRow, LevelMax - 9, LevelMax, "");

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format(Resources.UI_FloorSettingRow, LevelMax - 9, LevelMax, "");
        }

        public override bool Equals(object obj)
        {
            return (obj as FloorSetting)?.LevelMax == LevelMax;
        }

        public override int GetHashCode()
        {
            return LevelMax.GetHashCode();
        }
    }
}