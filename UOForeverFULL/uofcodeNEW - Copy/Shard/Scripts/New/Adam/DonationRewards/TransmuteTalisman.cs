using Server.Network;

namespace Server.Items
{
    public class TransmuteTalisman : Item
    {
        public override string DefaultName { get { return "Transmute Talisman"; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TransmuteID { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TransmuteHue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string TransmuteName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation TransmuteAnimation { get; set; }

        [Constructable]
        public TransmuteTalisman()
            : base(12123)
        {
            Hue = 0;
            Weight = 1.0;
            Layer = Layer.Talisman;
            LootType = LootType.Blessed;
        }

        public TransmuteTalisman(Serial serial)
            : base(serial)
        {}

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                var m = (Mobile) parent;

                var onehandedweapon = m.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
                var twohandedweapon = m.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;
                var spellbook = m.FindItemOnLayer(Layer.OneHanded) as Spellbook;

                if (onehandedweapon != null)
                {
                    if (onehandedweapon.OriginalItemID == 0)
                    {
                        onehandedweapon.OriginalItemID = onehandedweapon.ItemID;
                        onehandedweapon.OriginalHue = onehandedweapon.Hue;
                        onehandedweapon.OriginalAnimation = onehandedweapon.Animation;
                    }
                    onehandedweapon.ItemID = TransmuteID;

                    onehandedweapon.Hue = TransmuteHue;

                    onehandedweapon.Animation = TransmuteAnimation;
                }
                else if (spellbook != null)
                {
                    if (spellbook.OriginalItemID == 0)
                    {
                        spellbook.OriginalItemID = spellbook.ItemID;
                        spellbook.OriginalHue = spellbook.Hue;
                    }
                    spellbook.ItemID = TransmuteID;

                    spellbook.Hue = TransmuteHue;
                }
                else if (twohandedweapon != null)
                {
                    if (twohandedweapon.OriginalItemID == 0)
                    {
                        twohandedweapon.OriginalItemID = twohandedweapon.ItemID;
                        twohandedweapon.OriginalHue = twohandedweapon.Hue;
                        twohandedweapon.OriginalAnimation = twohandedweapon.Animation;
                    }
                    twohandedweapon.ItemID = TransmuteID;

                    twohandedweapon.Hue = TransmuteHue;

                    twohandedweapon.Animation = TransmuteAnimation;
                }
            }
            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                var m = (Mobile) parent;

                var onehandedweapon = m.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
                var twohandedweapon = m.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;
                var spellbook = m.FindItemOnLayer(Layer.OneHanded) as Spellbook;

                if (onehandedweapon != null)
                {
                    if (onehandedweapon.ItemID == TransmuteID)
                    {
                        onehandedweapon.ItemID = onehandedweapon.OriginalItemID;

                        onehandedweapon.Hue = onehandedweapon.OriginalHue;
                    
                        onehandedweapon.Animation = onehandedweapon.OriginalAnimation;
                    }
                }
                else if (spellbook != null)
                {
                    if (spellbook.ItemID == TransmuteID)
                    {
                        spellbook.ItemID = spellbook.OriginalItemID;

                        spellbook.Hue = spellbook.OriginalHue;
                    }
                }
                else if (twohandedweapon != null)
                {
                    if (twohandedweapon.ItemID == TransmuteID)
                    {
                        twohandedweapon.ItemID = twohandedweapon.OriginalItemID;

                        twohandedweapon.Hue = twohandedweapon.OriginalHue;

                        twohandedweapon.Animation = twohandedweapon.OriginalAnimation;
                    }
                }
            }
            base.OnRemoved(parent);
        }

        public override void OnSingleClick(Mobile @from)
        {
            base.OnSingleClick(@from);

            PrivateOverheadMessage(MessageType.Label, 54, true, "[" + TransmuteName + "]", from.NetState);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.SetVersion(0);

            writer.Write(TransmuteID);
            writer.Write(TransmuteHue);
            writer.Write((int) TransmuteAnimation);
            writer.Write(TransmuteName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.GetVersion();

            TransmuteID = reader.ReadInt();
            TransmuteHue = reader.ReadInt();
            TransmuteAnimation = (WeaponAnimation) reader.ReadInt();
            TransmuteName = reader.ReadString();
        }
    }
}