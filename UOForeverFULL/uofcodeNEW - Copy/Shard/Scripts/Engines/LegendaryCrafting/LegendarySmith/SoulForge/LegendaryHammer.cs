#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;
using Server.Engines.Conquests;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.LegendaryCrafting
{
    public sealed class LegendaryHammer : BaseTool
    {
        public override CraftSystem CraftSystem { get { return null; } }

        private const int Partial = 6;
        private const int Completed = 10;

        private int _Quantity;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (value <= 1)
                {
                    _Quantity = 1;
                }
                else if (value >= Completed)
                {
                    _Quantity = Completed;
                }
                else
                {
                    _Quantity = value;
                }

                if (_Quantity < Partial)
                {
                    ItemID = 22328;
                }
                else if (_Quantity < Completed)
                {
                    ItemID = 0x13E3;
                }
                else
                {
                    ItemID = 0xFB4;
                }

                InvalidateProperties();
            }
        }

        public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled && EraAOS; } }

        [Constructable]
        public LegendaryHammer()
            : base(0x5738)
        {
            Layer = Layer.OneHanded;
            Hue = 2498;
            _Quantity = 1;
            Weight = 16;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Quantity < Partial)
            {
                list.Add("an ancient smithing hammer shard");
            }
            else if (_Quantity < Completed)
            {
                list.Add("A partially reconstructed legendary smithing hammer");
            }
            else
            {
                list.Add("a legendary smithing hammer");
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (_Quantity < Partial)
            {
                LabelTo(from, "an ancient smithing hammer shard");
            }
            else if (_Quantity < Completed)
            {
                LabelTo(from, "A partially reconstructed legendary hammer");
            }
            else
            {
                LabelTo(from, "a legendary hammer");
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && _Quantity >= Partial && _Quantity < Completed && IsChildOf(from.Backpack))
            {
                list.Add(new DisassembleEntry(this));
            }
        }

        private class DisassembleEntry : ContextMenuEntry
        {
            private readonly LegendaryHammer _Hammer;

            public DisassembleEntry(LegendaryHammer legendaryHammer)
                : base(6142)
            {
                _Hammer = legendaryHammer;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (_Hammer.Deleted || _Hammer.Quantity < Partial || _Hammer.Quantity >= Completed ||
                    !_Hammer.IsChildOf(from.Backpack) || !from.CheckAlive())
                {
                    return;
                }

                for (int i = 0; i < _Hammer.Quantity - 1; i++)
                {
                    from.AddToBackpack(new LegendaryHammer());
                }

                _Hammer.Quantity = 1;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (_Quantity < Completed)
            {
                if (!IsChildOf(from.Backpack))
                {
                    // Nothing Happens.
                    from.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x2C, 3, 500309, "", ""));
                }
                else
                {
                    from.Target = new InternalTarget(this);
                }
            }

            if (IsChildOf(from.Backpack) && _Quantity >= Completed || Parent == from)
            {
                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted && i.Completed);
                if (notepad != null && from.Skills[SkillName.Blacksmith].Value >= 120.0 &&
                    from.Skills[SkillName.Tinkering].Value >= 120)
                {
                    var UI = new SoulForgeFinished(from as PlayerMobile,
                        onAccept: x => { from.Target = new PlaceSFInternalTarget(this, notepad); }).
                        Send<SoulForgeFinished>();
                }
                else if (from.Skills[SkillName.Blacksmith].Value >= 120.0 &&
                         from.Skills[SkillName.Tinkering].Value >= 120)
                {
                    var UI = new InitialHammer120Skill(from as PlayerMobile).
                        Send<InitialHammer120Skill>();
                }
                else
                {
                    var UI = new InitialHammer120LessThanSkill(from as PlayerMobile).
                        Send<InitialHammer120LessThanSkill>();
                    from.SendMessage(54, "The arcane runes on the hammer mean nothing to you.");
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private class InternalTarget : Target
        {
            private readonly LegendaryHammer _Hammer;

            public InternalTarget(LegendaryHammer hShard)
                : base(-1, false, TargetFlags.None)
            {
                _Hammer = hShard;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                var targ = targeted as LegendaryHammer;

                if (_Hammer.Deleted || _Hammer.Quantity >= Completed || targ == null)
                {
                    return;
                }

                if (_Hammer.IsChildOf(from.Backpack) && targ.IsChildOf(from.Backpack) && targ != _Hammer)
                {
                    LegendaryHammer targShard = targ;

                    if (targShard.Quantity < Completed)
                    {
                        if (targShard.Quantity + _Hammer.Quantity <= Completed)
                        {
                            targShard.Quantity += _Hammer.Quantity;
                            _Hammer.Delete();
                        }
                        else
                        {
                            int delta = Completed - targShard.Quantity;
                            targShard.Quantity += delta;
                            _Hammer.Quantity -= delta;
                        }

                        from.Send(
                            new AsciiMessage(
                                targShard.Serial,
                                targShard.ItemID,
                                MessageType.Regular,
                                0x59,
                                3,
                                _Hammer.Name,
                                "the partially reconstructed hammer and the ancient smithing hammer piece meld together."));

                        return;
                    }
                }

                // Nothing Happens.
                from.Send(
                    new MessageLocalized(_Hammer.Serial, _Hammer.ItemID, MessageType.Regular, 0x2C, 3, 500309,
                        _Hammer.Name, ""));
            }
        }

        private class PlaceSFInternalTarget : Target
        {
            private readonly LegendaryHammer Hammer;

            private readonly TranscriptionBook Book;

            public PlaceSFInternalTarget(LegendaryHammer hammer, TranscriptionBook book)
                : base(-1, true, TargetFlags.None)
            {
                Hammer = hammer;

                Book = book;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                var p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || Hammer.Deleted)
                {
                    return;
                }

                if (Hammer.IsChildOf(from.Backpack) && Book.IsChildOf(from.Backpack))
                {
                    BaseAddon addon = new SoulForgeAddon();

                    SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    var res = AddonFitResult.Valid;
                    if (from.AccessLevel < AccessLevel.GameMaster)
                    {
                        res = addon.CouldFit(p, map, from, ref house);
                    }
                    else
                    {
                        house = BaseHouse.FindHouseAt(new Point3D(p), map, p.Z);
                        if (house == null)
                        {
                            res = AddonFitResult.NotInHouse;
                        }
                    }

                    if (res == AddonFitResult.Valid)
                    {
                        addon.MoveToWorld(new Point3D(p), map);
                    }
                    else if (res == AddonFitResult.Blocked)
                    {
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    }
                    else if (res == AddonFitResult.NotInHouse)
                    {
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    }
                    else if (res == AddonFitResult.DoorTooClose)
                    {
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    }
                    else if (res == AddonFitResult.NoWall)
                    {
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
                    }

                    if (res == AddonFitResult.Valid && house != null && TryConsumeSfMaterials(from))
                    {
                        Conquests.Conquests.CheckProgress<ItemConquest>(from as PlayerMobile, this);
                        from.PrivateOverheadMessage(MessageType.Label, 2049, true,
                            "*You begin crafting the Soulforge*",
                            from.NetState);
                        Timer.DelayCall(TimeSpan.FromSeconds(1), SFCreate_Callback, Tuple.Create(0, from, p));
                        from.Frozen = true;
                        Hammer.Delete();
                        Book.Delete();
                        house.Addons.Add(addon);
                    }
                    else
                    {
                        addon.Delete();
                        from.SendMessage(54, "You did not have the required materials to create the Soulforge.");
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }

        public static void SFCreate_Callback(Tuple<int, Mobile, IPoint3D> obj)
        {
            int sequence = obj.Item1;
            Mobile m = obj.Item2;
            IPoint3D p = obj.Item3;
            WeaponAnimation animation = ((BaseWeapon) m.Weapon).Animation;

            switch (sequence)
            {
                case 0:
                {
                    m.Animate((int) animation, 7, 1, true, false, 0);
                    m.PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "*You create the base of the Soulforge using mundane materials and the ground dragonbone shards*",
                        m.NetState);
                    Timer.DelayCall(TimeSpan.FromSeconds(4), SFCreate_Callback, Tuple.Create(1, m, p));
                    break;
                }
                case 1:
                {
                    m.Animate((int) animation, 7, 1, true, false, 0);
                    m.PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "*You gently place the intact dragon hearts in the middle of the forge*",
                        m.NetState);
                    Timer.DelayCall(TimeSpan.FromSeconds(4), SFCreate_Callback, Tuple.Create(2, m, p));
                    break;
                }
                case 2:
                {
                    m.Animate((int) animation, 7, 1, true, false, 0);
                    m.PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "*You place both the soul of the Devourer and the soul of the Harrower into the forge*",
                        m.NetState);
                    Timer.DelayCall(TimeSpan.FromSeconds(4), SFCreate_Callback, Tuple.Create(3, m, p));
                    break;
                }
                case 3:
                {
                    m.Animate((int) animation, 7, 1, true, false, 0);
                    m.PrivateOverheadMessage(MessageType.Label, 2049, true,
                        "*You throw the heart of Rikktor, the dragon-king, into the forge*",
                        m.NetState);
                    List<Mobile> list =
                        m.GetMobilesInRange(m.Map, 30).Where(mob => mob != null && !mob.Deleted && mob.Player).ToList();
                    foreach (Mobile player in list)
                    {
                        Effects.SendIndividualFlashEffect(player, (FlashType) 2);
                    }
                    FlameSpiral(new Point3D(p), Map.Felucca);
                    m.Frozen = false;
                    break;
                }
            }
        }

        public static void FlameSpiral(Point3D target, Map map)
        {
            var queue = new EffectQueue
            {
                Deferred = false
            };

            var points = new List<Point3D>();
            double d;
            double r = 1;
            int newx;
            int newy;
            points.Add(target);
            //calculate spiral vector
            for (d = 0; d < 4 * Math.PI; d += 0.01)
            {
                newx = (int) Math.Floor(target.X + (Math.Sin(d) * d) * r);
                newy = (int) Math.Floor(target.Y + (Math.Sin(d + (Math.PI / 2)) * (d + (Math.PI / 2))) * r);
                var to = new Point3D(newx, newy, target.Z);
                if (!points.Contains(to))
                {
                    points.Add(to);
                }
            }
            int n = 0;
            //Build the queue based on the points in the line.
            foreach (Point3D p in points)
            {
                n += 20;
                queue.Add(
                    new EffectInfo(p, map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                        () => { }));
            }
            n += 400; //used to offset when the spiral reverses so it doesn't overlap
            foreach (Point3D p in points.AsEnumerable().Reverse())
            {
                n += 20;
                queue.Add(
                    new EffectInfo(p, map, 14089, 0, 10, 30, EffectRender.Normal, TimeSpan.FromMilliseconds(n),
                        () => { }));
            }
            queue.Process();
        }

        public static bool TryConsumeSfMaterials(Mobile from)
        {
            Container pack = from.Backpack;

            var toDelete = new List<Item>();
            var dragonboneshards = new List<DragonBoneShards>();
            var dragonhearts = new List<DragonHeart>();
            DevourerSoul devourersoul = null;
            HarrowerSoul harrowersoul = null;
            HeartofRikktor rikktorheart = null;

            foreach (Item item in pack.Items)
            {
                if (rikktorheart == null && item is HeartofRikktor)
                {
                    rikktorheart = item as HeartofRikktor;
                    toDelete.Add(item);
                }
                else if (devourersoul == null && item is DevourerSoul)
                {
                    devourersoul = item as DevourerSoul;
                    toDelete.Add(item);
                }
                else if (harrowersoul == null && item is HarrowerSoul)
                {
                    harrowersoul = item as HarrowerSoul;
                    toDelete.Add(item);
                }
                else if (dragonboneshards.Count < 20 && item is DragonBoneShards)
                {
                    dragonboneshards.Add(item as DragonBoneShards);
                    toDelete.Add(item);
                }
                else if (dragonhearts.Count < 2 && item is DragonHeart)
                {
                    dragonhearts.Add(item as DragonHeart);
                    toDelete.Add(item);
                }
            }
            if (rikktorheart != null && devourersoul != null && harrowersoul != null && dragonboneshards.Count == 20 &&
                dragonhearts.Count == 2)
            {
                foreach (Item item in toDelete)
                {
                    item.Delete();
                }
                return true;
            }
            return false;
        }

        public LegendaryHammer(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(_Quantity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();

            Weight = 16;

            _Quantity = reader.ReadEncodedInt();
        }
    }
}