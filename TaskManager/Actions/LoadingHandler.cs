/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Directors;
using ff14bot.Managers;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;

namespace DeepCombined.TaskManager.Actions
{
    internal class LoadingHandler : ITask
    {
        public string Name => "LoadingHandler";

        public async Task<bool> Run()
        {
            if (CommonBehaviors.IsLoading)
            {
                await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                return true;
            }

            if (QuestLogManager.InCutscene)
            {
                TreeRoot.StatusText = "InCutscene";
                if (AgentCutScene.Instance != null)
                {
                    AgentCutScene.Instance.PromptSkip();
                    await Coroutine.Wait(250, () => SelectString.IsOpen);
                    if (SelectString.IsOpen)
                        SelectString.ClickSlot(0);
                    return true;
                }
            }

            if (Talk.DialogOpen)
            {
                Talk.Next();
                return true;
            }

            //wait if the barrier is still up
            if (DirectorManager.ActiveDirector is InstanceContentDirector activeAsInstance)
                if (activeAsInstance.TimeLeftInDungeon == TimeSpan.Zero)
                    return true;

            return false;
        }

        public void Tick()
        {
        }
    }
}