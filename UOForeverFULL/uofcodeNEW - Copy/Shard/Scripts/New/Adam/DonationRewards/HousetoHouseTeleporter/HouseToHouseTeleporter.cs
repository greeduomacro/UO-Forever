#region References

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class HouseToHouseTeleporterDeed : Item
    {
        [Constructable]
        public HouseToHouseTeleporterDeed()
            : base(0x14F0)
        {
            Name = "a house-to-house teleporter deed";
            LootType = LootType.Blessed;
        }

        public HouseToHouseTeleporterDeed(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                Container backpack = from.Backpack;
                if (backpack != null)
                {
                    var teleporter1 = new HouseToHouseTeleporter();
                    var teleporter2 = new HouseToHouseTeleporter();

                    teleporter1.LinkedTeleporter = teleporter2;
                    teleporter1.Owner = from;
                    teleporter2.Owner = from;
                    teleporter2.LinkedTeleporter = teleporter1;
                    teleporter2.ItemID = 6183;
                    backpack.AddItem(teleporter1);
                    backpack.AddItem(teleporter2);

                    Consume();
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }

    public class HouseToHouseTeleporter : Item, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public HouseToHouseTeleporter LinkedTeleporter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        private int _Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return _Charges; }
            set
            {
                if (LinkedTeleporter != null)
                {
                    _Charges = value;
                    LinkedTeleporter._Charges = value;
                }
            } 
        }

        [Constructable]
        public HouseToHouseTeleporter()
            : base(6184)
        {
            Name = "a house-to-house teleporter deed";
            LootType = LootType.Blessed;
            Level = SecureLevel.Friends;
            Charges = 0;
        }

        public HouseToHouseTeleporter(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(Owner);
            writer.Write(LinkedTeleporter);
            writer.Write((int)Level);
            writer.Write(_Charges);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    Owner = reader.ReadMobile();
                    LinkedTeleporter = reader.ReadItem<HouseToHouseTeleporter>();
                    Level = (SecureLevel)reader.ReadInt();
                    _Charges = reader.ReadInt();
                }
                break;
            }
        }

        public bool CheckAccess(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
            {
                return false;
            }

            return house != null && house.HasSecureAccess(m, Level);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);

            PrivateOverheadMessage(MessageType.Label, 54, true, "Charges: " + _Charges, m.NetState);

            if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house == null || !house.IsOwner(m))
                {
                    return;
                }

                m.CloseGump(typeof(SetSecureLevelGump));
                m.SendGump(new SetSecureLevelGump(m, this, house));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsLockedDown && CheckAccess(from))
            {
                from.SendGump(new RechargeHousetoHouseGump(this));
            }
            else if (IsChildOf(from.Backpack) && Owner == from)
            {
                from.SendGump(new RedeedHousetoHouseGump(this));
            }
        }

        public void Redeed()
        {
            Owner.AddToBackpack(new HouseToHouseTeleporterDeed());
            if (LinkedTeleporter != null)
            {
                LinkedTeleporter.Delete();
            }
            Delete();
        }

        public void GetTarget(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new ItemSelectTarget<Item>((m, t) => AddCharges(m, t), m => { });
            }
        }

        public void AddCharges(Mobile User, Item target)
        {
            TeleportationCrystal crystal = target as TeleportationCrystal;

            if (crystal != null)
            {
                if (crystal.RootParentEntity == User)
                {
                    if (Charges < 1000)
                    {
                        User.Send(new PlaySound(0x249, GetWorldLocation()));

                        int amount = crystal.Amount;

                        if (amount > (1000 - Charges))
                        {
                            crystal.Consume(1000 - Charges);
                            Charges = 1000;
                            User.SendMessage(54, "You have fully charged the teleporter.");
                        }
                        else
                        {
                            Charges += amount;
                            crystal.Delete();
                            User.SendMessage(54, "You have added " + amount + " charge(s) to the teleporter.");
                        }
                    }
                    else
                    {
                        User.SendMessage(54, "This teleporter cannot hold more charges.");
                    }
                }
                else
                {
                    User.SendMessage(54, "You must have the teleportation crystals in your backpack to charge the teleporter.");
                }
            }
            else
            {
                User.SendMessage(54, "You must use teleportation crystals to charge the teleporter!");
            }
        }


        public override bool OnMoveOver(Mobile m)
        {
            if (LinkedTeleporter == null)
            {
                m.SendMessage(54, "The linked teleporter does not appear to exist on this plane of reality any longer..");
                return base.OnMoveOver(m);
            }

            if (LinkedTeleporter.IsLockedDown && IsLockedDown && CheckAccess(m))
            {
                if (Charges <= 0)
                {
                    m.SendMessage(54, "This teleporter requires more teleportation crystals.");
                    return true;
                }
                if (Map.CanSpawnMobile(LinkedTeleporter.Location))
                {
                    if (!m.Hidden || m.AccessLevel < AccessLevel.Counselor)
                    {
                        new EffectTimer(Location, Map, 2023, 0x1F0, TimeSpan.FromSeconds(0.4)).Start();
                    }

                    new DelayTimer(this, m).Start();
                }
                else
                {
                    m.SendMessage(54, "Invalid location.");
                }
            }
            else if (!CheckAccess(m))
            {
                m.SendMessage(54, "You do not have access to this teleporter.");
            }
            else
            {
                m.SendMessage(54, "Both teleporters must be locked down.");
            }

            return base.OnMoveOver(m);
        }

        private class EffectTimer : Timer
        {
            private readonly Point3D _Location;
            private readonly Map _Map;
            private readonly int _EffectID;
            private readonly int _SoundID;

            public EffectTimer(Point3D p, Map map, int effectID, int soundID, TimeSpan delay)
                : base(delay)
            {
                _Location = p;
                _Map = map;
                _EffectID = effectID;
                _SoundID = soundID;
            }

            protected override void OnTick()
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(_Location, _Map, EffectItem.DefaultDuration), 0x3728, 10, 10, _EffectID, 0);

                if (_SoundID != -1)
                {
                    Effects.PlaySound(_Location, _Map, _SoundID);
                }
            }
        }

        private class DelayTimer : Timer
        {
            private readonly HouseToHouseTeleporter _Teleporter;
            private readonly Mobile _Mobile;

            public DelayTimer(HouseToHouseTeleporter tp, Mobile m)
                : base(TimeSpan.FromSeconds(1.0))
            {
                _Teleporter = tp;
                _Mobile = m;
            }

            protected override void OnTick()
            {
                Item target = _Teleporter.LinkedTeleporter;

                if (_Teleporter.LinkedTeleporter == null || _Teleporter.LinkedTeleporter.Deleted || _Mobile.Location != _Teleporter.Location ||
                    _Mobile.Map != _Teleporter.Map)
                {
                    return;
                }

                Point3D p = target.GetWorldTop();
                Map map = target.Map;

                BaseCreature.TeleportPets(_Mobile, p, map);

                Effects.SendFlashEffect(_Mobile, (FlashType)2);

                _Mobile.MoveToWorld(p, map);

                _Teleporter.Charges--;

                Effects.SendFlashEffect(_Mobile, (FlashType)2);

                if (!_Mobile.Hidden || _Mobile.AccessLevel < AccessLevel.Counselor)
                {
                    Effects.PlaySound(target.Location, target.Map, 0x1FE);

                    Effects.SendLocationParticles(
                        EffectItem.Create(_Teleporter.Location, _Teleporter.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023, 0);
                    Effects.SendLocationParticles(
                        EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023, 0);

                    new EffectTimer(target.Location, target.Map, 2023, -1, TimeSpan.FromSeconds(0.4)).Start();
                }
            }
        }
    }

    public class TeleportationCrystal : Item
    {
        [Constructable]
        public TeleportationCrystal()
            : this(1)
        { }

        [Constructable]
        public TeleportationCrystal(int amount)
            : base(22328)
        {
            Name = "teleportation crystals";
            Stackable = true;
            Weight = 0.1;
            Hue = 1170;
            LootType = LootType.Blessed;
            Amount = amount;
        }

        public TeleportationCrystal(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}