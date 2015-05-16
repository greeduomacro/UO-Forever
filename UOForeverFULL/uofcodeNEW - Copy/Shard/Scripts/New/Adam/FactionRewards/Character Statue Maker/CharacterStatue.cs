#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;

#endregion

namespace Server.Mobiles
{
    public enum StatueType
    {
        None,
        Minax,
        TB,
        CoM,
        SL
    }

    public enum StatuePose
    {
        Ready,
        Casting,
        Salute,
        AllPraiseMe,
        Fighting,
        HandsOnHips
    }

    public class CharacterStatue : Mobile, IRewardItem
    {
        private StatueType m_Type;
        private StatuePose m_Pose;
        private Timer _UpdateTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                InvalidateHues();
                InvalidatePose();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public StatuePose Pose
        {
            get { return m_Pose; }
            set
            {
                m_Pose = value;
                InvalidatePose();
            }
        }

        private Mobile m_SculptedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SculptedBy
        {
            get { return m_SculptedBy; }
            set
            {
                m_SculptedBy = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SculptedOn { get; set; }

        public CharacterStatuePlinth Plinth { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        public CharacterStatue(Mobile from, StatueType type)
        {
            m_Type = type;
            m_Pose = StatuePose.Ready;

            Direction = Direction.South;
            AccessLevel = AccessLevel.Counselor;
            Hits = HitsMax;
            Blessed = true;
            Frozen = true;

            CloneBody(from);
            CloneClothes(from);

            InvalidateHues();
        }

        public CharacterStatue(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            UpdateStatue();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            UpdateStatue();
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_SculptedBy == null)
            {
                return;
            }

            if (m_SculptedBy.ShowFameTitle && (m_SculptedBy.Player || m_SculptedBy.Body.IsHuman) &&
                m_SculptedBy.Fame >= 10000)
            {
                // Sculpted by ~1_Name~
                list.Add(1076202, String.Format("{0} {1}", m_SculptedBy.Female ? "Lady" : "Lord", m_SculptedBy.Name));
            }
            else
            {
                // Sculpted by ~1_Name~
                list.Add(1076202, m_SculptedBy.Name);
            }
        }

        public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(m, list);

            if (!m.Alive || m_SculptedBy == null)
            {
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if ((house != null && house.IsCoOwner(m)) || m.AccessLevel >= AccessLevel.GameMaster)
            {
                list.Add(new DemolishEntry(this));
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Plinth == null)
            {
                return;
            }

            Plinth.Delete();
            Plinth = null;
        }

        protected override void OnMapChange(Map oldMap)
        {
            InvalidatePose();

            if (Plinth != null && !Plinth.Deleted)
            {
                Plinth.Map = Map;
            }
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            InvalidatePose();

            if (Plinth != null && !Plinth.Deleted)
            {
                Plinth.Location = new Point3D(X, Y, Z - 5);
            }
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return from != null && from.AccessLevel >= AccessLevel.GameMaster;
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public void OnRequestedAnimation(Mobile from)
        {
            from.Send(new UpdateStatueAnimationSA(this, m_Animation, m_Frames));
        }

        public override void OnAosSingleClick(Mobile from)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int) m_Type);
            writer.Write((int) m_Pose);

            writer.Write(m_SculptedBy);
            writer.Write(SculptedOn);

            writer.Write(Plinth);
            writer.Write(IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();

            m_Type = (StatueType) reader.ReadInt();
            m_Pose = (StatuePose) reader.ReadInt();

            m_SculptedBy = reader.ReadMobile();
            SculptedOn = reader.ReadDateTime();

            Plinth = reader.ReadItem<CharacterStatuePlinth>();
            IsRewardItem = reader.ReadBool();

            InvalidatePose();

            Frozen = true;

            if (m_SculptedBy == null || Map == Map.Internal) // Remove preview statues
            {
                Timer.DelayCall(TimeSpan.Zero, Delete);
            }
        }

        public void Sculpt(Mobile by)
        {
            m_SculptedBy = by;

            SculptedOn = DateTime.UtcNow;

            InvalidateProperties();
        }

        public bool Demolish(Mobile m)
        {
            var deed = new CharacterStatueDeed(null);

            if (m.PlaceInBackpack(deed))
            {
                Delete();

                deed.Statue = this;
                deed.StatueType = m_Type;

                if (Plinth != null)
                {
                    Plinth.Delete();
                    Plinth = null;
                }

                return true;
            }

            m.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
            deed.Delete();

            return false;
        }

        public void Restore(CharacterStatue m)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            m_Pose = m.Pose;

            Direction = m.Direction;

            CloneBody(m);
            CloneClothes(m);

            InvalidateHues();
            InvalidatePose();
        }

        public void CloneBody(Mobile m)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            Female = m.Female;

            RawName = m.RawName;
            NameMod = m.NameMod;
            NameHue = m.NameHue;
            Hue = m.Hue;
            HairHue = m.HairHue;
            FacialHairHue = m.FacialHairHue;

            Title = m.Title;
            Fame = m.Fame;
            Karma = m.Karma;
            Kills = m.Kills;

            Race = m.Race;
            BodyValue = m.BodyValue;
            BodyMod = m.BodyMod;

            HairItemID = m.HairItemID;
            
            FacialHairItemID = m.FacialHairItemID;
        }

        public void CloneClothes(Mobile m)
        {
            Items.RemoveAll(i => i == null || i.Deleted);
            Items.For((k, v) => v.Delete());

            if (m == null || m.Deleted)
            {
                return;
            }

            m.Items.Where(i => i != null && !i.Deleted && i != m.Backpack && i != m.FindBankNoCreate() && i != m.Mount)
                .Select(CloneItem)
                .Not(i => i == null || i.Deleted)
                .ForEach(AddItem);
        }

        public Item CloneItem(Item item)
        {
            return item != null
                ? new Item(item.ItemID)
                {
                    Layer = item.Layer,
                    Name = item.Name,
                    Hue = item.Hue,
                    Weight = item.Weight,
                    Movable = false
                }
                : null;
        }

        public void InvalidateHues()
        {
            switch (m_Type)
            {
                case StatueType.Minax:
                {
                    Hue = 1645;
                    break;
                }
                case StatueType.TB:
                {
                    Hue = 2214;
                    break;
                }
                case StatueType.CoM:
                {
                    Hue = 1325;
                    break;
                }
                case StatueType.SL:
                {
                    Hue = 1109;
                    break;
                }
            }

            if (m_Type != StatueType.None)
            {
                HairHue = Hue;

                if (FacialHairItemID > 0)
                {
                    FacialHairHue = Hue;
                }

                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    Items[i].Hue = Hue;
                }
            }

            if (Plinth != null)
            {
                Plinth.InvalidateHue();
            }
        }
        public void UpdateStatueDelayed()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.1), new TimerCallback(UpdateStatue));
        }

        public void UpdateStatue()
        {
            Animate(m_Animation, 0, 1, false, false, 255);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (_UpdateTimer == null)
            {
                _UpdateTimer = new UpdateTimer(this);
                _UpdateTimer.Start();
            }
        }

        private int m_Animation;
        private int m_Frames;

        public void InvalidatePose()
        {
            UpdateStatueDelayed();
            switch (m_Pose)
            {
                case StatuePose.Ready:
                    m_Animation = 4;
                    m_Frames = 0;
                    break;
                case StatuePose.Casting:
                    m_Animation = 16;
                    m_Frames = 2;
                    break;
                case StatuePose.Salute:
                    m_Animation = 33;
                    m_Frames = 1;
                    break;
                case StatuePose.AllPraiseMe:
                    m_Animation = 17;
                    m_Frames = 4;
                    break;
                case StatuePose.Fighting:
                    m_Animation = 31;
                    m_Frames = 5;
                    break;
                case StatuePose.HandsOnHips:
                    m_Animation = 6;
                    m_Frames = 1;
                    break;
            }

            if (Map == null)
            {
                return;
            }

            ProcessDelta();

            Packet p = null;

            IPooledEnumerable eable = Map.GetClientsInRange(Location);

            foreach (NetState state in eable)
            {
                state.Mobile.ProcessDelta();

                if (p == null)
                {
                    p = Packet.Acquire(new UpdateStatueAnimationSA(this, m_Animation, m_Frames));
                }

                state.Send(p);
            }

            Packet.Release(p);

            eable.Free();
        }

        private class DemolishEntry : ContextMenuEntry
        {
            private readonly CharacterStatue m_Statue;

            public DemolishEntry(CharacterStatue statue)
                : base(6275, 2)
            {
                m_Statue = statue;
            }

            public override void OnClick()
            {
                if (m_Statue.Deleted)
                {
                    return;
                }

                m_Statue.Demolish(Owner.From);
            }
        }
    }

    public class CharacterStatueDeed : Item
    {
        private CharacterStatue m_Statue;
        private StatueType m_Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoHouse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CharacterStatue Statue { get { return m_Statue; } set { m_Statue = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get
            {
                if (m_Statue != null)
                {
                    return m_Statue.StatueType;
                }

                return m_Type;
            }
            set { m_Type = value; }
        }

        public CharacterStatueDeed(CharacterStatue statue)
            : base(0x14F0)
        {
            m_Statue = statue;

            if (statue != null)
            {
                m_Type = statue.StatueType;
            }

            Name = "a character statue deed";

            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public CharacterStatueDeed(Serial serial)
            : base(serial)
        {}


        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Statue != null)
            {
                list.Add(1076231, m_Statue.Name); // Statue of ~1_Name~
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            string label = String.Empty;
            int labelhue = 0;
            switch (m_Type)
            {
                case StatueType.Minax:
                    {
                        label = "Minax";
                        labelhue = 1645;
                        break;
                    }
                case StatueType.TB:
                    {
                        label = "True Britannians";
                        labelhue = 2214;
                        break;
                    }
                case StatueType.CoM:
                    {
                        label = "Council of Mages";
                        labelhue = 1325;
                        break;
                    }
                case StatueType.SL:
                    {
                        label = "Shadowlords";
                        labelhue = 1109;
                        break;
                    }
            }
            if (label != String.Empty)
            {
                LabelTo(from, "[" + label + "]", labelhue);
            }
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (!from.IsBodyMod)
                {
                    from.SendLocalizedMessage(1076194); // Select a place where you would like to put your statue.
                    from.Target = new CharacterStatueTarget(this, StatueType);
                }
                else
                {
                    from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (m_Statue != null)
            {
                m_Statue.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version

            writer.Write((int) m_Type);

            writer.Write(m_Statue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version >= 1)
            {
                m_Type = (StatueType) reader.ReadInt();
            }

            m_Statue = reader.ReadMobile() as CharacterStatue;
        }
    }

    public class CharacterStatueTarget : Target
    {
        private readonly Item m_Maker;
        private readonly StatueType m_Type;

        public CharacterStatueTarget(Item maker, StatueType type)
            : base(-1, true, TargetFlags.None)
        {
            m_Maker = maker;
            m_Type = type;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            var p = targeted as IPoint3D;
            Map map = from.Map;

            if (p == null || map == null || m_Maker == null || m_Maker.Deleted)
            {
                return;
            }

            if (m_Maker.IsChildOf(from.Backpack))
            {
                SpellHelper.GetSurfaceTop(ref p);
                BaseHouse house = null;
                var loc = new Point3D(p);

                if (m_Maker is CharacterStatueDeed && !((CharacterStatueDeed) m_Maker).NoHouse ||
                    m_Maker is CharacterStatueMaker && !((CharacterStatueMaker) m_Maker).NoHouse)
                {
                    if (targeted is Item && !((Item) targeted).IsLockedDown && !((Item) targeted).IsSecure &&
                        !(targeted is AddonComponent))
                    {
                        from.SendLocalizedMessage(1076191); // Statues can only be placed in houses.
                        return;
                    }
                }
                if (@from.IsBodyMod)
                {
                    @from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
                    return;
                }

                AddonFitResult result = CouldFit(loc, map, from, ref house);

                if (result == AddonFitResult.Valid || m_Maker is CharacterStatueDeed && ((CharacterStatueDeed) m_Maker).NoHouse ||
                    m_Maker is CharacterStatueMaker && ((CharacterStatueMaker) m_Maker).NoHouse)
                {
                    var statue = new CharacterStatue(from, m_Type);
                    var plinth = new CharacterStatuePlinth(statue);

                    if (m_Maker is CharacterStatueDeed && !((CharacterStatueDeed) m_Maker).NoHouse ||
                        m_Maker is CharacterStatueMaker && !((CharacterStatueMaker) m_Maker).NoHouse)
                    {
                        house.Addons.Add(plinth);
                    }

                    if (m_Maker is IRewardItem)
                    {
                        statue.IsRewardItem = ((IRewardItem) m_Maker).IsRewardItem;
                    }

                    statue.Plinth = plinth;
                    plinth.MoveToWorld(loc, map);
                    statue.InvalidatePose();

                    /*
                     * TODO: Previously the maker wasn't deleted until after statue
                     * customization, leading to redeeding issues. Exact OSI behavior
                     * needs looking into.
                     */
                    m_Maker.Delete();
                    statue.Sculpt(from);

                    from.CloseGump(typeof(CharacterStatueGump));
                    from.SendGump(new CharacterStatueGump(m_Maker, statue, from));
                }
                else if (result == AddonFitResult.Blocked)
                {
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                }
                else if (result == AddonFitResult.NotInHouse)
                {
                    from.SendLocalizedMessage(1076192);
                    // Statues can only be placed in houses where you are the owner or co-owner.
                }
                else if (result == AddonFitResult.DoorTooClose)
                {
                    from.SendLocalizedMessage(500271); // You cannot build near the door.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public static AddonFitResult CouldFit(Point3D p, Map map, Mobile from, ref BaseHouse house)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, 20, true, true, true))
            {
                return AddonFitResult.Blocked;
            }
            if (!BaseAddon.CheckHouse(@from, p, map, 20, ref house))
            {
                return AddonFitResult.NotInHouse;
            }
            return CheckDoors(p, 20, house);
        }

        public static AddonFitResult CheckDoors(Point3D p, int height, BaseHouse house)
        {
            List<Item> doors = house.Doors;

            if ((from t in doors
                select t as BaseDoor
                into door
                let doorLoc = door.GetWorldLocation()
                let doorHeight = door.ItemData.CalcHeight
                where
                    Utility.InRange(doorLoc, p, 1) &&
                    (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z))
                select doorLoc).Any())
            {
                return AddonFitResult.DoorTooClose;
            }

            return AddonFitResult.Valid;
        }
    }

    public class UpdateTimer : Timer
    {
        private CharacterStatue Statue;

        public UpdateTimer(CharacterStatue statue) : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(1.0))
        {
            Priority = TimerPriority.OneSecond;
            Statue = statue;
        }

        protected override void OnTick()
        {
            Statue.UpdateStatue();
        }
    }
}