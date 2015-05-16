using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x1403, 0x1402)]
    public class BloodSpear : BaseSpear
    {
        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ShadowStrike; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.MortalStrike; } }

        public override int OldStrengthReq { get { return 15; } }
        public override int NewMinDamage { get { return WeaponDamageController._ShortSpearDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._ShortSpearDamageMax; } }
        public override int DiceDamage { get { return Utility.Dice(2, 15, 2); } } // 2d15+2 (4-32)
        public override int OldSpeed { get { return 50; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 70; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

        [Constructable]
        public BloodSpear()
            : base(0x1403)
        {
            Name = "Blood Spear";
            Hue = 2117;
            Identified = true;
            Weight = 4.0;
        }

        public BloodSpear(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}