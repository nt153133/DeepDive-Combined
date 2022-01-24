using Newtonsoft.Json;
/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

namespace DeepCombined.Helpers
{
    internal static class ResourceHelpers
    {

        /// <summary>
        /// Loads a JSON file from Resources.
        /// </summary>
        /// <typeparam name="T">Type to deserialize JSON as.</typeparam>
        /// <param name="json">Raw JSON to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        internal static T LoadResource<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}