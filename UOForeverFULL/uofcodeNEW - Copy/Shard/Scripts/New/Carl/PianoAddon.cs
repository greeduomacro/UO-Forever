using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class PianoAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new PianoAddonDeed();
            }
        }

        [Constructable]
        public PianoAddon()
        {
            AddonComponent ac = null;
            ac = new

AddonComponent(2928);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 2);

            ac = new AddonComponent(5981);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 6);

            ac = new AddonComponent(5984);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 8);

            ac = new AddonComponent(5981);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 7);

            ac = new AddonComponent(5985);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 9);

            ac = new AddonComponent(5431);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 10);

            ac = new AddonComponent(7933);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 7);

            ac = new AddonComponent(2480);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 1, 11);

            ac = new AddonComponent(7883);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, 0, 1);

            ac = new AddonComponent(2480);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, -1, -1, 2);

            ac = new AddonComponent(2924);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, -1, 0);

            ac = new AddonComponent(2925);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 0);

            ac = new AddonComponent(4006);
            ac.Name = "Piano Keys";
            AddComponent(ac, 0, 0, 7);

            ac = new AddonComponent(5981);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 10);

            ac = new AddonComponent(7933);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 9);

            ac = new AddonComponent(5991);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 9);

            ac = new AddonComponent(5988);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 10);

            ac = new AddonComponent(5987);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 8);

            ac = new AddonComponent(5988);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 9);

            ac = new AddonComponent(2252);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 0, 11);

            ac = new AddonComponent(2923);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 0);

            ac = new AddonComponent(2845);
            ac.Light = LightType.Circle225;
            ac.Name = "A Candelabra";
            AddComponent(ac, 0, 1, 17);

            ac = new AddonComponent(4006);
            ac.Name = "Piano Keys";
            AddComponent(ac, 0, 1, 7);

            ac = new AddonComponent(7031);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 12);

            ac = new AddonComponent(7933);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 14);

            ac = new AddonComponent(5986);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 14);

            ac = new AddonComponent(5986);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 12);

            ac = new AddonComponent(5991);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 8);

            ac = new AddonComponent(5987);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 9);

            ac = new AddonComponent(5985);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 0, 1, 10);

            ac = new AddonComponent(3774);
            ac.Name = "Sheet Music";
            AddComponent(ac, 1, 1, 15);

            ac = new AddonComponent(3772);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 1, 1, 12);

            ac = new AddonComponent(1114);
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent(ac, 1, 0, 0);
        }

        public PianoAddon(Serial serial)
            : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent ac, Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
                from.SendMessage("You are too far away to use that!");

            else
            {
                if (ac.ItemID == 3774)
                {
                    from.SendGump(new PianoGump());
                }
                else
                    return;
            }
        }

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

    }

    public class PianoAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new PianoAddon();
            }
        }

        [Constructable]
        public PianoAddonDeed()
        {
            Name = "Piano";
            LootType = LootType.Blessed;
        }

        public PianoAddonDeed(Serial serial)
            : base(serial)
        {
        }

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
    }
}

namespace Server.Gumps
{
    public class PianoGump : Gump
    {
        public PianoGump()
            : base(0, 0)
        {

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(6, 15, 570, 140, 5054);
            AddAlphaRegion(16, 20, 550, 130);
            AddImageTiled(16, 20, 550, 20, 9354);
            AddLabel(19, 20, 200, "Piano Keys");
            AddLabel(55, 60, 0, @"do");
            AddLabel(55, 80, 0, @"do");
            AddLabel(55, 100, 0, @"do");
            AddLabel(95, 60, 0, @"do#");
            AddLabel(95, 80, 0, @"do#");
            AddLabel(145, 60, 0, @"re");
            AddLabel(145, 80, 0, @"re");
            AddLabel(185, 60, 0, @"re#");
            AddLabel(185, 80, 0, @"re#");
            AddLabel(235, 60, 0, @"mi");
            AddLabel(235, 80, 0, @"mi");
            AddLabel(275, 60, 0, @"fa");
            AddLabel(275, 80, 0, @"fa");
            AddLabel(315, 60, 0, @"fa#");
            AddLabel(315, 80, 0, @"fa#");
            AddLabel(365, 60, 0, @"sol");
            AddLabel(365, 80, 0, @"sol");
            AddLabel(405, 60, 0, @"sol#");
            AddLabel(405, 80, 0, @"sol#");
            AddLabel(455, 60, 0, @"la");
            AddLabel(455, 80, 0, @"la");
            AddLabel(495, 60, 0, @"la#");
            AddLabel(495, 80, 0, @"la#");
            AddLabel(545, 60, 0, @"ti");
            AddLabel(545, 80, 0, @"ti");
            AddButton(35, 62, 5601, 5605, 1, GumpButtonType.Reply, 0);
            AddButton(35, 82, 5601, 5605, 2, GumpButtonType.Reply, 0);
            AddButton(35, 102, 5601, 5605, 3, GumpButtonType.Reply, 0);
            AddButton(75, 62, 5601, 5605, 4, GumpButtonType.Reply, 0);
            AddButton(75, 82, 5601, 5605, 5, GumpButtonType.Reply, 0);
            AddButton(125, 62, 5601, 5605, 6, GumpButtonType.Reply, 0);
            AddButton(125, 82, 5601, 5605, 7, GumpButtonType.Reply, 0);
            AddButton(165, 62, 5601, 5605, 8, GumpButtonType.Reply, 0);
            AddButton(165, 82, 5601, 5605, 9, GumpButtonType.Reply, 0);
            AddButton(215, 62, 5601, 5605, 10, GumpButtonType.Reply, 0);
            AddButton(215, 82, 5601, 5605, 11, GumpButtonType.Reply, 0);
            AddButton(255, 62, 5601, 5605, 12, GumpButtonType.Reply, 0);
            AddButton(255, 82, 5601, 5605, 13, GumpButtonType.Reply, 0);
            AddButton(295, 62, 5601, 5605, 14, GumpButtonType.Reply, 0);
            AddButton(295, 82, 5601, 5605, 15, GumpButtonType.Reply, 0);
            AddButton(345, 62, 5601, 5605, 16, GumpButtonType.Reply, 0);
            AddButton(345, 82, 5601, 5605, 17, GumpButtonType.Reply, 0);
            AddButton(385, 62, 5601, 5605, 18, GumpButtonType.Reply, 0);
            AddButton(385, 82, 5601, 5605, 19, GumpButtonType.Reply, 0);
            AddButton(435, 62, 5601, 5605, 20, GumpButtonType.Reply, 0);
            AddButton(435, 82, 5601, 5605, 21, GumpButtonType.Reply, 0);
            AddButton(475, 62, 5601, 5605, 22, GumpButtonType.Reply, 0);
            AddButton(475, 82, 5601, 5605, 23, GumpButtonType.Reply, 0);
            AddButton(525, 62, 5601, 5605, 24, GumpButtonType.Reply, 0);
            AddButton(525, 82, 5601, 5605, 25, GumpButtonType.Reply, 0);
            AddButton(425, 120, 241, 242, 26, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;

            if (m == null)
                return;

            switch (info.ButtonID)
            {
                case 0: { m.SendMessage(60, "You stop playing."); break; }

                case 1: { m.PlaySound(1028); m.SendGump(new PianoGump()); break; }

                case 2: { m.PlaySound(1029); m.SendGump(new PianoGump()); break; }

                case 3: { m.PlaySound(1030); m.SendGump(new PianoGump()); break; }

                case 4: { m.PlaySound(1031); m.SendGump(new PianoGump()); break; }

                case 5: { m.PlaySound(1032); m.SendGump(new PianoGump()); break; }

                case 6: { m.PlaySound(1033); m.SendGump(new PianoGump()); break; }

                case 7: { m.PlaySound(1034); m.SendGump(new PianoGump()); break; }

                case 8: { m.PlaySound(1036); m.SendGump(new PianoGump()); break; }

                case 9: { m.PlaySound(1037); m.SendGump(new PianoGump()); break; }

                case 10: { m.PlaySound(1038); m.SendGump(new PianoGump()); break; }

                case 11: { m.PlaySound(1039); m.SendGump(new PianoGump()); break; }

                case 12: { m.PlaySound(1040); m.SendGump(new PianoGump()); break; }

                case 13: { m.PlaySound(1041); m.SendGump(new PianoGump()); break; }

                case 14: { m.PlaySound(1042); m.SendGump(new PianoGump()); break; }

                case 15: { m.PlaySound(1043); m.SendGump(new PianoGump()); break; }

                case 16: { m.PlaySound(1044); m.SendGump(new PianoGump()); break; }

                case 17: { m.PlaySound(1045); m.SendGump(new PianoGump()); break; }

                case 18: { m.PlaySound(1046); m.SendGump(new PianoGump()); break; }

                case 19: { m.PlaySound(1047); m.SendGump(new PianoGump()); break; }

                case 20: { m.PlaySound(1021); m.SendGump(new PianoGump()); break; }

                case 21: { m.PlaySound(1022); m.SendGump(new PianoGump()); break; }

                case 22: { m.PlaySound(1023); m.SendGump(new PianoGump()); break; }

                case 23: { m.PlaySound(1024); m.SendGump(new PianoGump()); break; }

                case 24: { m.PlaySound(1025); m.SendGump(new PianoGump()); break; }

                case 25: { m.PlaySound(1026); m.SendGump(new PianoGump()); break; }

                case 26: { m.SendMessage(60, "You stop playing."); break; }

            }
        }
    }
}
