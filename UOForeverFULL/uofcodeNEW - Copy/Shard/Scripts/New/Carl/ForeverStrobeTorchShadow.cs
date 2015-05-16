#region References

using System;
using System.Linq;
using Server;
using Server.Items;

#endregion

namespace VitaNex.Items
{
    public class ForeverStrobeTorchShadow : BaseEquipableLight
    {
        public override int LitItemID { get { return 0xA12; } }
        public override int UnlitItemID { get { return 0xF6B; } }

        public override int LitSound { get { return 0x54; } }
        public override int UnlitSound { get { return 0x4BB; } }

        private Timer _Timer;
        private int _HueCycleIndex;
        private TimeSpan _HueCycleDelay;

        [CommandProperty(AccessLevel.GameMaster)]
        private bool Active { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                if (HueCycle != null && HueCycle.Length > 0)
                {
                    if (_HueCycleIndex >= HueCycle.Length)
                    {
                        _HueCycleIndex = 0;
                    }
                    else if (_HueCycleIndex < 0)
                    {
                        _HueCycleIndex = HueCycle.Length - 1;
                    }

                    return HueCycle[_HueCycleIndex];
                }

                return base.Hue;
            }
            set { base.Hue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan HueCycleDelay
        {
            get { return _HueCycleDelay; }
            set
            {
                _HueCycleDelay = TimeSpan.FromSeconds(Math.Max(0.100, value.TotalSeconds));

                if (_Timer != null)
                {
                    _Timer.Interval = _HueCycleDelay;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool HueCycleReverse { get; set; }

        public virtual short[] HueCycle { get; set; }

        [Constructable]
        public ForeverStrobeTorchShadow()
            : base(0xF6B)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(30);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle300;

            Name = "Forever Strobe Torch";
            Weight = 3;
            Hue = 0;

            Active = false;

            HueCycleDelay = TimeSpan.FromSeconds(0.5);
            HueCycle = new short[] {2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026};

            LootType = LootType.Blessed;
        }

        public ForeverStrobeTorchShadow(Serial serial)
            : base(serial)
        {}

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
        public override void OnAdded(object parent)
#endif
        {
            base.OnAdded(parent);

            if (!(Parent is Mobile))
            {
                EndHueCycle();
                return;
            }

            if (!Active)
            {
                Active = true;
                EndHueCycle();
                BeginHueCycle();               
            }
            else
            {
                Active = false;
                EndHueCycle();
                HueCycleReverse = false;
            }
        }

#if NEWPARENT
		public override void OnRemoved(IEntity parent)
#else
        public override void OnRemoved(object parent)
#endif
        {
            base.OnRemoved(parent);

            Active = false;
            EndHueCycle();
            HueCycleReverse = false;
        }

        private void BeginHueCycle()
        {
            if (_Timer == null)
            {
                _Timer = Timer.DelayCall(HueCycleDelay, HueCycleDelay, CycleHue);
            }
            else
            {
                _Timer.Start();
            }

            OnHueCycleBegin();
        }

        private void EndHueCycle()
        {
            if (_Timer != null)
            {
                _Timer.Stop();
                _Timer = null;
            }

            OnHueCycleEnd();
        }

        private void CycleHue()
        {
            if (Map == null || Map == Map.Internal || (!(Parent is Mobile) && Parent != null))
            {
                EndHueCycle();
                return;
            }

            if (Hue == HueCycle.Last())
            {
                HueCycleReverse = true;
            }
            else if (Hue == HueCycle.First())
            {
                HueCycleReverse = false;
            }

            if (HueCycleReverse)
            {
                --_HueCycleIndex;
            }
            else
            {
                ++_HueCycleIndex;
            }

            ReleaseWorldPackets();
            Delta(ItemDelta.Update);

            OnHueCycled();
        }

        protected virtual void OnHueCycled()
        {}

        protected virtual void OnHueCycleBegin()
        {
            _HueCycleIndex = HueCycle[0];
            ReleaseWorldPackets();
            Delta(ItemDelta.Update);
        }

        protected virtual void OnHueCycleEnd()
        {
            _HueCycleIndex = HueCycle[0];
            ReleaseWorldPackets();
            Delta(ItemDelta.Update);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(_HueCycleDelay);
            writer.Write(_HueCycleIndex);
            writer.Write(HueCycleReverse);
            writer.WriteArray(HueCycle, writer.Write);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            _HueCycleDelay = reader.ReadTimeSpan();
            _HueCycleIndex = reader.ReadInt();
            HueCycleReverse = reader.ReadBool();
            HueCycle = reader.ReadArray(reader.ReadShort);
        }
    }
}