/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using ff14bot;

namespace DeepCombined.Helpers
{
    internal class FrameCache<T>
    {
        private readonly Func<T> _producer;
        private T _cached;
        private uint _lastFrame = uint.MaxValue;

        public FrameCache(Func<T> producer)
        {
            _producer = producer ?? throw new ArgumentNullException("producer");
        }

        public T Value
        {
            get
            {
                var frameCount = Core.Memory.Executor.FrameCount;
                if (_lastFrame != frameCount)
                {
                    _cached = _producer();
                    _lastFrame = frameCount;
                }

                return _cached;
            }
        }

        internal bool NeedsUpdating => Core.Memory.Executor.FrameCount != _lastFrame;

        public static implicit operator T(FrameCache<T> pfcv)
        {
            return pfcv.Value;
        }
    }
}