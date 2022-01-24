using ff14bot.Enums;

namespace DeepCombined.Structure
{
    public class ClassLevelTarget
    {
        public ClassJobType classJobType;
        public int _level;
        private int _gearSlot;

        public ClassJobType Job
        {
            get => classJobType;
            set => classJobType = value;
        }

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public int GearSlot
        {
            get => _gearSlot;
            set => _gearSlot = value;
        }

        public ClassLevelTarget(ClassJobType job, int level, int gearSlot)
        {
            classJobType = job;
            _level = level;
            _gearSlot = gearSlot;
        }

        public string DisplayString => $"{Job} to level {Level}";

        public override string ToString()
        {
            return $"Want to get {Job} to level {Level}";
        }

    }
}