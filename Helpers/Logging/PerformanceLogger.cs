/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeepCombined.Helpers.Logging
{
    [DebuggerStepThrough]
    internal class PerformanceLogger : IDisposable
    {
        private readonly string _blockName;
        private readonly bool _forceLog;
        private readonly Stopwatch _stopwatch;
        private bool _isDisposed;

        public PerformanceLogger(string blockName, bool forceLog = false)
        {
            _forceLog = forceLog;
            _blockName = blockName;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.TotalMilliseconds > 5 || _forceLog)
            {
                if (!Constants.PerformanceStats.ContainsKey(_blockName))
                    Constants.PerformanceStats.Add(_blockName, new List<double>());
                
                Constants.PerformanceStats[_blockName].Add(_stopwatch.Elapsed.TotalMilliseconds);

                if (_stopwatch.Elapsed.TotalMilliseconds >= 10)
                    Logger.Error("[Performance] Execution of \"{0}\" took {1:00.00000}ms.", _blockName,
                        _stopwatch.Elapsed.TotalMilliseconds); 
            }
            _stopwatch.Reset();
        }

        #endregion

        ~PerformanceLogger()
        {
            Dispose();
        }
    }
}