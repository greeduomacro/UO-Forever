#region References

using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using VitaNex.Targets;

#endregion

namespace Server.Items
{
    public class CorruptedCrystalPortal : Item, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public CorruptedCrystalPortal()
        {
            Hue = 1157;
            ItemID = 18059;
            Name = "Corrupted Crystal Portal";
            Movable = true;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3) && CheckAccess(from))
            {
                if (Movable)
                {
                    from.SendMessage("This must be locked down in a house to use!");
                    // from.SendGump( new CrystalPortalGump( from ) ); 
                }
                else
                {
                    from.SendGump(new CorruptedCrystalPortalGump(from, this));
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void OnSingleClick(Mobile @from)
        {
            base.OnSingleClick(@from);
            PrivateOverheadMessage(MessageType.Label, 54, true, "Charges: " + Charges, from.NetState);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(Movable ? "This must be locked down in a house to use!" : "Double-click to open help menu");
        }

        public bool CheckAccess(Mobile m)
        {
            if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
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

        public CorruptedCrystalPortal(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write((int)Level);

            writer.Write(Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Level = (SecureLevel)reader.ReadInt();
                    goto case 0;
                case 0:
                {
                    Charges = reader.ReadInt();
                }
                    break;
            }
        }

        public virtual void DoTeleport(Mobile m, Point3D loc, Map map)
        {
            if (Charges >= 1)
            {
                m.SendLocalizedMessage(501024); // You open a magical gate to another location

                Effects.PlaySound(m.Location, m.Map, 0x20E);

                var firstGate = new InternalItem(loc, map);
                firstGate.MoveToWorld(m.Location, m.Map);

                Effects.PlaySound(loc, map, 0x20E);

                Charges--;
            }
            else
            {
                m.SendMessage(54, "The crystal portal needs more charges. Add charges to it by double clicking the crystal.");
            }
        }

		[DispellableField]
		private class InternalItem : Moongate
		{
			public override bool ShowFeluccaWarning { get { return Caster != null && Caster.EraAOS; } }

			public InternalItem(Point3D target, Map map)
				: base(target, map)
			{
			    Hue = 1159;

				Map = map;

				if (ShowFeluccaWarning && map == Map.Felucca)
				{
					ItemID = 0xDDA;
				}

				Dispellable = true;

				var t = new InternalTimer(this);
				t.Start();
			}

			public InternalItem(Serial serial)
				: base(serial)
			{ }

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);

				Delete();
			}

			private class InternalTimer : Timer
			{
				private readonly Item m_Item;

				public InternalTimer(Item item)
					: base(TimeSpan.FromSeconds(30.0))
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}

				protected override void OnTick()
				{
					m_Item.Delete();
				}
			}
		}

        public void GetTarget(Mobile User)
        {
            if (User != null && !User.Deleted)
            {
                User.Target = new ItemSelectTarget<Item>((m, t) => AddCharges(m, t), m => { });
            }           
        }

        public void AddCharges(Mobile User, Item target)
        {
            GateTravelScroll scrolls = target as GateTravelScroll;

            if (scrolls != null)
            {
                if (scrolls.RootParentEntity == User)
                {
                    if (Charges < 1000)
                    {
                        User.Send(new PlaySound(0x249, GetWorldLocation()));

                        int amount = scrolls.Amount;

                        if (amount > (1000 - Charges))
                        {
                            scrolls.Consume(1000 - Charges);
                            Charges = 1000;
                            User.SendMessage(54, "You have fully charged the crystal portal.");
                        }
                        else
                        {
                            Charges += amount;
                            scrolls.Delete();
                            User.SendMessage(54, "You have added " + amount + " charge(s) to the crystal portal.");
                        }
                    }
                    else
                    {
                        User.SendMessage(54, "This crystal portal cannot hold more charges.");
                    }
                }
                else
                {
                    User.SendMessage(54, "You must have the gate travel scrolls in your backpack to charge the crystal portal.");
                }
            }
            else
            {
                User.SendMessage(54, "You must use gate travel scrolls to charge the crystal portal!");
            }
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(Location, 2))
            {
                if (Movable)
                {
                    e.Mobile.SendMessage("This must be locked down in a house to use!");
                }
                else if (Sigil.ExistsOn(e.Mobile))
                {
                    e.Mobile.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                }
                else if (WeightOverloading.IsOverloaded(e.Mobile))
                {
                    e.Mobile.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                }
                else if (e.Mobile.Criminal)
                {
                    e.Mobile.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                }
                else if (SpellHelper.CheckCombat(e.Mobile))
                {
                    e.Mobile.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                }
                else if (e.Mobile.Spell != null)
                {
                    e.Mobile.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                }

                    /*  Begin Speech Entries  */

                    // fel //
                else if (e.Speech.ToLower() == "dungeon covetous")
                {
                    var loc = new Point3D(2498, 921, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon deceit")
                {
                    var loc = new Point3D(4111, 434, 5);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon despise")
                {
                    var loc = new Point3D(1301, 1080, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon destard")
                {
                    var loc = new Point3D(1176, 2640, 2);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon fire")
                {
                    var loc = new Point3D(2923, 3409, 8);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map); ;
                }
                else if (e.Speech.ToLower() == "dungeon hythloth")
                {
                    var loc = new Point3D(4721, 3824, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon ice")
                {
                    var loc = new Point3D(1999, 81, 4);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon orc")
                {
                    var loc = new Point3D(1017, 1429, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon shame")
                {
                    var loc = new Point3D(511, 1565, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon wrong")
                {
                    var loc = new Point3D(2043, 238, 10);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
                else if (e.Speech.ToLower() == "dungeon wind")
                {
                    var loc = new Point3D(1361, 895, 0);
                    Map map = Map.Felucca;

                    DoTeleport(e.Mobile, loc, map);
                }
            }
        }
    }
}