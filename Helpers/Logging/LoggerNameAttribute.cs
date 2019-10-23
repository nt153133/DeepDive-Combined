/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;

namespace Deep.Helpers.Logging
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LoggerNameAttribute : Attribute
    {
        public LoggerNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public string Name { get; private set; }
    }
}