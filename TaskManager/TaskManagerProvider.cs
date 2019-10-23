/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deep.Helpers.Logging;

namespace Deep.TaskManager
{

    interface ITask
    {
        string Name { get; }
        void Tick();
        Task<bool> Run();
    }
        


    class TaskManagerProvider : List<ITask>
    {
        public TaskManagerProvider() { }

        public void Tick()
        {
            foreach(var x in this)
            {
                try
                {
                    x.Tick();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"[TaskManager][Tick] {x.Name} threw an Exception {ex}");
                }
            }
        }

        public async Task<bool> Run()
        {
            foreach(var x in this)
            {
                try
                {
                    if (await x.Run())
                        return true;
                }
                catch (Exception ex)
                {
                    Logger.Warn($"[TaskManager][Run] {x.Name} threw an Exception {ex}");
                    return false;
                }
            }
            return false;
        }
    }
}
