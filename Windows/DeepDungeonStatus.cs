using DeepCombined.Memory;
using ff14bot.Managers;
using LlamaLibrary.RemoteWindows;

namespace DeepCombined.Windows
{
    internal class DeepDungeonStatus : RemoteWindow
    {
        public DeepDungeonStatus() : base(WindowNames.DDStatus)
        {
        }
        private static DeepDungeonStatus _instance;

        public static DeepDungeonStatus Instance => _instance ?? (_instance = new DeepDungeonStatus());
        internal static int Agent => AgentModule.FindAgentIdByVtable(Offsets.DeepDungeonStatusVtable);

        internal void Open()
        {
            if (!IsOpen)
            {
                AgentModule.ToggleAgentInterfaceById(Agent);
            }
        }

        internal void CastMagicite()
        {

            if (!IsOpen)
            {
                //Logger.Error("Window not open");
                Open();
            }

            if (IsOpen)
            {
                //Logger.Error("Send Action");
                SendAction(2, 3, 0xC, 0x3, 0x0);
            }
        }
    }
}