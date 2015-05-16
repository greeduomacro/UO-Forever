#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Engines.ZombieEvent;
using Server.Items;
using VitaNex;
using VitaNex.Notify;

#endregion

namespace Server.Mobiles
{
    public sealed class ExpansionNotifyGump : NotifyGump
    {
        private static void InitSettings(NotifySettings settings)
        {
            settings.Name = "Era Updates";
            settings.CanIgnore = true;
        }

        public ExpansionNotifyGump(PlayerMobile user, string html)
            : base(user, html)
        {}
    }

    public partial class PlayerMobile
    {
        private PlayerSnapshot _Snapshot;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerSnapshot Snapshot { get { return _Snapshot ?? (_Snapshot = new PlayerSnapshot(this)); } }

        public BankBox[] BankBoxes
        {
            get
            {
                BankBox[] banks = Snapshot.Banks;

                if (banks.Length == 0 && Snapshot.Pull(Expansion))
                {
                    banks = Snapshot.Banks;
                }

                return banks;
            }
        }

        public Container[] Backpacks
        {
            get
            {
                Container[] packs = Snapshot.Packs;

                if (packs.Length == 0 && Snapshot.Pull(Expansion))
                {
                    packs = Snapshot.Packs;
                }

                return packs;
            }
        }

        public override BankBox FindBank(Expansion e)
        {
            return base.FindBank(e) ?? BankBoxes.FirstOrDefault(b => b.Expansion == e);
        }

        protected override void OnExpansionChanged(Expansion old)
        {
            base.OnExpansionChanged(old);

            if (ExpansionInfo.GetInfo(Expansion).Name != "Third Dawn")
            {
                this.SendNotification<ExpansionNotifyGump>(
                    String.Format(
                        "You are now bound by the rules and physics of the [b]{0}[/b] era!",
                        ExpansionInfo.GetInfo(Expansion).Name),
                    true,
                    1.0,
                    10.0,
                    Color.LawnGreen);
            }
            else
            {
                this.SendNotification<ExpansionNotifyGump>(
                    String.Format(
                        "Welcome to Zombie Land!"),
                    true,
                    1.0,
                    10.0,
                    Color.LawnGreen);
            }

            if (Expansion <= Expansion.T2A && AccessLevel < AccessLevel.Counselor || Expansion == Expansion.UOTD && AccessLevel < AccessLevel.Counselor)
            {
                BaseMount.SetMountPrevention(this, BlockMountType.MapRestricted, TimeSpan.FromDays(365));
            }
            else
            {
                BaseMount.ClearMountPrevention(this);
            }

            if (!Snapshot.Pull(old))
            {
                return;
            }

            var leaveEquip = new List<Item>();

            if (Alive)
            {
                leaveEquip.AddRange(
                    Items.Where(i => i != null && !i.Deleted && i.IsPhased && i != BankBox && i != Backpack));
            }

            foreach (Item item in leaveEquip)
            {
                Backpack.DropItem(item);
            }

            Item holding = Holding;

            if (holding != null)
            {
                if (!holding.Deleted && holding.HeldBy == this && holding.Map == Map.Internal)
                {
                    Backpack.DropItem(holding);
                }

                Holding = null;
                holding.ClearBounce();
            }

            Dictionary<Item, Point3D> takePack = Backpack.FindItemsByType<Item>(true,
                i => i != null && !i.Deleted && !i.IsPhased)
                .ToDictionary(i => i, i => i.Location);

            if (!Snapshot.Push(Expansion))
            {
                foreach (Item item in leaveEquip)
                {
                    EquipItem(item);
                }

                return;
            }

            foreach (Item item in leaveEquip)
            {
                Send(item.RemovePacket);
            }

            foreach (KeyValuePair<Item, Point3D> kv in takePack)
            {
                Backpack.DropItem(kv.Key);
                kv.Key.Location = kv.Value;
            }

            UpdateTotals();
            ProcessDelta();

            SendMessage(0x22, "Character snapshots synced successfully.");
        }

        private void SerializeSnapshot(GenericWriter writer)
        {
            writer.SetVersion(0);

            Snapshot.Serialize(writer);
        }

        private void DeserializeSnapshot(GenericReader reader)
        {
            reader.GetVersion();

            _Snapshot = new PlayerSnapshot(reader);
        }
    }

    public sealed class PlayerSnapshot : PropertyObject
    {
        private static readonly Expansion[] _Expansions = ((Expansion) 0).GetValues<Expansion>();

        public static List<PlayerSnapshot> Instances { get; private set; }

        static PlayerSnapshot()
        {
            Instances = new List<PlayerSnapshot>();
        }

        private static void Register(PlayerSnapshot state)
        {
            if (state == null || state.Owner == null)
            {
                return;
            }

            Instances.RemoveAll(
                s => s == null || s == state || s.Owner == null || s.Owner.Deleted || s.Owner == state.Owner);

            if (!state.Owner.Deleted)
            {
                Instances.Add(state);
            }
        }

        public PlayerMobile Owner { get; private set; }

        public Dictionary<Expansion, PlayerSnapshotState> States { get; private set; }

        public PlayerSnapshotState CoreState { get { return Owner != null ? EnsureState(Core.Expansion) : null; } }
        public PlayerSnapshotState State { get { return Owner != null ? EnsureState(Owner.Expansion) : null; } }

        public BankBox[] Banks
        {
            //
            get
            {
                return States.Values.Where(s => s.BankBox != null && !s.BankBox.Deleted).Select(s => s.BankBox).ToArray();
            }
        }

        public Container[] Packs
        {
            //
            get
            {
                return
                    States.Values.Where(s => s.Backpack != null && !s.Backpack.Deleted)
                        .Select(s => s.Backpack)
                        .ToArray();
            }
        }

        public PlayerSnapshot(PlayerMobile owner)
        {
            Owner = owner;

            States = _Expansions.ToDictionary(e => e, e => new PlayerSnapshotState(Owner, e));

            Instances.Add(this);
        }

        public PlayerSnapshot(GenericReader reader)
            : base(reader)
        {
            Instances.Add(this);
        }

        public override void Clear()
        {}

        public override void Reset()
        {}

        /// <summary>
        ///     Pull values from the player, based on the given expansion.
        /// </summary>
        public bool Pull(Expansion ex)
        {
            PlayerSnapshotState state = EnsureState(ex);

            return state != null && state.Pull();
        }

        /// <summary>
        ///     Push values to the player, based on the given expansion.
        /// </summary>
        public bool Push(Expansion ex)
        {
            PlayerSnapshotState state = EnsureState(ex);

            return state != null && state.Push();
        }

        private PlayerSnapshotState EnsureState(Expansion ex)
        {
            if (Owner == null || Owner.Deleted)
            {
                return null;
            }

            PlayerSnapshotState state;

            if (!States.TryGetValue(ex, out state))
            {
                States.Add(ex, state = new PlayerSnapshotState(Owner, ex));
            }
            else if (state == null)
            {
                States[ex] = state = new PlayerSnapshotState(Owner, ex);
            }

            return state;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version;

            writer.Write(version = 0);

            writer.Write(Owner);

            switch (version)
            {
                case 0:
                {
                    writer.WriteArray(States.Values.ToArray(), state => state.Serialize(writer));
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Owner = reader.ReadMobile<PlayerMobile>();

            switch (version)
            {
                case 0:
                {
                    States = reader.ReadArray(() => new PlayerSnapshotState(reader))
                        .ToDictionary(state => state.Expansion, state => state);
                }
                    break;
            }
        }
    }

    public sealed class PlayerSnapshotState : PropertyObject
    {
        [CommandProperty(AccessLevel.GameMaster, true)]
        public PlayerMobile Owner { get; private set; }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public Expansion Expansion { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BankBox BankBox { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container Backpack { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StatCap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RawStr { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RawDex { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RawInt { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillsCap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Skills Skills { get; set; }

        public PlayerSnapshotState(PlayerMobile owner, Expansion ex)
        {
            Owner = owner;
            Expansion = ex;

            StatCap = Owner.StatCap;

            RawStr = Owner.RawStr;
            RawDex = Owner.RawDex;
            RawInt = Owner.RawInt;

            SkillsCap = Owner.SkillsCap;

            Skills = new Skills(Owner);
        }

        public PlayerSnapshotState(GenericReader reader)
            : base(reader)
        {}

        public override void Clear()
        {}

        public override void Reset()
        {}

        /// <summary>
        ///     Pull values from the player, based on this state.
        /// </summary>
        public bool Pull()
        {
            if (Owner == null || Owner.Deleted)
            {
                return false;
            }

            #region BankBox

            if (Owner.BankBox == null || Owner.BankBox.Deleted)
            {
                Owner.BankBox = new BankBox(Owner, Expansion);
            }

            if (Owner.BankBox.Expansion != Expansion)
            {
                if (Owner.BankBox.Expansion != Expansion.None)
                {
                    Owner.BankBox = Owner.BankBoxes.FirstOrDefault(b => b.Expansion == Expansion) ??
                                    BankBox ?? new BankBox(Owner, Expansion);
                }
                else
                {
                    Owner.BankBox.Expansion = Expansion;
                }
            }

            BankBox = Owner.BankBox;

            BankBox.UpdateTotals();

            #endregion

            #region Backpack

            if (Owner.Backpack == null || Owner.Backpack.Deleted)
            {
                Owner.Backpack = new Backpack(Expansion);
            }

            if (Owner.Backpack.Expansion != Expansion)
            {
                if (Owner.Backpack.Expansion != Expansion.None)
                {
                    Owner.Backpack = Owner.Backpacks.FirstOrDefault(b => b.Expansion == Expansion) ??
                                     Backpack ?? new Backpack(Expansion);
                }
                else
                {
                    Owner.Backpack.Expansion = Expansion;
                }
            }

            Backpack = Owner.Backpack;

            Backpack.UpdateTotals();

            #endregion

            StatCap = Owner.StatCap;

            RawStr = Owner.RawStr;
            RawDex = Owner.RawDex;
            RawInt = Owner.RawInt;

            SkillsCap = Owner.SkillsCap;

            Skills.For(
                (i, s) =>
                {
                    Skill sk = Owner.Skills[i];

                    s.SetCap(sk.Cap);
                    s.SetBase(sk.Base, true, false);
                });

            return true;
        }

        /// <summary>
        ///     Push values to the player, based on this state.
        /// </summary>
        public bool Push()
        {
            if (Owner == null || Owner.Deleted)
            {
                return false;
            }

            #region BankBox

            if (BankBox == null || BankBox.Deleted)
            {
                BankBox = new BankBox(Owner, Expansion);
            }

            BankBox.Layer = Layer.Bank;
            BankBox.BlessedFor = null;
            BankBox.Visible = true;
            BankBox.Hue = 0;

            Owner.BankBox.Layer = Layer.Invalid;
            Owner.BankBox.BlessedFor = Owner;
            Owner.BankBox.Visible = false;
            Owner.BankBox.Hue = 34;

            Owner.BankBox.Close();
            Owner.Send(Owner.BankBox.RemovePacket);

            Owner.AddItem(BankBox);

            Owner.BankBox = BankBox;

            Owner.BankBox.UpdateTotals();

            #endregion

            #region Backpack

            if (Backpack == null || Backpack.Deleted)
            {
                Backpack = new Backpack(Expansion);
            }

            Backpack.Layer = Layer.Backpack;
            Backpack.BlessedFor = null;
            Backpack.Visible = true;
            Backpack.Movable = true;
            Backpack.Hue = 0;

            Owner.Backpack.Layer = Layer.Invalid;
            Owner.Backpack.BlessedFor = Owner;
            Owner.Backpack.Visible = false;
            Owner.Backpack.Movable = false;
            Owner.Backpack.Hue = 34;

            Owner.Send(Owner.Backpack.RemovePacket);

            Owner.AddItem(Backpack);

            Owner.Backpack = Backpack;

            Owner.Backpack.UpdateTotals();

            #endregion

            Owner.StatCap = StatCap;

            Owner.RawStr = RawStr;
            Owner.RawDex = RawDex;
            Owner.RawInt = RawInt;

            Owner.SkillsCap = SkillsCap;

            Owner.Skills.For(
                (i, s) =>
                {
                    Skill sk = Skills[i];

                    s.SetCap(sk.Cap);
                    s.SetBase(sk.Base, true, false);
                });

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version;

            writer.Write(version = 0);

            writer.WriteFlag(Expansion);
            writer.Write(Owner);

            switch (version)
            {
                case 0:
                {
                    writer.Write(BankBox);
                    writer.Write(Backpack);

                    writer.Write(StatCap);
                    writer.Write(RawStr);
                    writer.Write(RawDex);
                    writer.Write(RawInt);

                    writer.Write(SkillsCap);
                    Skills.Serialize(writer);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Expansion = reader.ReadFlag<Expansion>();
            Owner = reader.ReadMobile<PlayerMobile>();

            switch (version)
            {
                case 0:
                {
                    BankBox = reader.ReadItem<BankBox>();
                    Backpack = reader.ReadItem<Container>();

                    StatCap = reader.ReadInt();
                    RawStr = reader.ReadInt();
                    RawDex = reader.ReadInt();
                    RawInt = reader.ReadInt();

                    SkillsCap = reader.ReadInt();
                    Skills = new Skills(Owner, reader);
                }
                    break;
            }
        }
    }
}