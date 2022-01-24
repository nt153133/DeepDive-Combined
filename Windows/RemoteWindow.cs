﻿using System;
using ff14bot;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public abstract class RemoteWindow
    {
        protected readonly string _name;
        private const int Offset0 = 0x1CA;
        private const int Offset2 = 0x160;
        /*      Can Also do this: Will pull the same offsets Mastahg stores in RB
                var off = typeof(Core).GetProperty("Offsets", BindingFlags.NonPublic | BindingFlags.Static);
                var struct158 = off.PropertyType.GetFields()[72];
                var offset0 = (int)struct158.FieldType.GetFields()[0].GetValue(struct158.GetValue(off.GetValue(null)));
                var offset2 = (int)struct158.FieldType.GetFields()[2].GetValue(struct158.GetValue(off.GetValue(null)));
        */

        public virtual bool IsOpen => RaptureAtkUnitManager.GetWindowByName(_name) != null;

        protected AtkAddonControl WindowByName => RaptureAtkUnitManager.GetWindowByName(_name);

        protected bool HasAgentInterfaceId => GetAgentInterfaceId() != 0;

        protected RemoteWindow(string name)
        {
            _name = name;
        }

        public virtual void Close()
        {
            SendAction(1, 3uL, 4294967295uL);
        }

        public int GetAgentInterfaceId()
        {
            if (WindowByName == null)
            {
                return 0;
            }

            AgentInterface test = WindowByName.TryFindAgentInterface();

            return test == null ? 0 : test.Id;
        }

        protected TwoInt[] ___Elements()
        {
            if (WindowByName == null)
            {
                return null;
            }

            ushort elementCount = ElementCount();
            IntPtr addr = Core.Memory.Read<IntPtr>(WindowByName.Pointer + Offset2);
            return Core.Memory.ReadArray<TwoInt>(addr, elementCount);
        }

        protected ushort ElementCount()
        {
            return WindowByName != null ? Core.Memory.Read<ushort>(WindowByName.Pointer + Offset0) : (ushort)0;
        }

        public void SendAction(int pairCount, params ulong[] param)
        {
            if (IsOpen)
            {
                WindowByName.SendAction(pairCount, param);
            }
        }
    }
}