using LlamaLibrary.RemoteWindows;

namespace DeepCombined.Windows
{
    public class DeepDungeonResult : RemoteWindow
    {
        private static DeepDungeonResult _instance;

        public static DeepDungeonResult Instance => _instance ?? (_instance = new DeepDungeonResult());

        public DeepDungeonResult() : base(WindowNames.DDResult)
        {
        }
    }
}