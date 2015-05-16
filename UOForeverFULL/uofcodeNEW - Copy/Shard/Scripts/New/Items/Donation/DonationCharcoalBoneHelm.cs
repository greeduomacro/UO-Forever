using System;
using Server;

namespace Server.Items
{
    public class DonationCharcoalBoneHelm : BoneHelm
	{
		//public override int ArtifactRarity{ get{ return 11; } }
        public override int ArmorBase { get { return 0; } }
        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public DonationCharcoalBoneHelm()
		{
			Name = "Bone Mask";
			LootType = LootType.Blessed;
			Hue = 2052;
            Identified = true;
	/*		Attributes.SpellDamage = 10;
			Attributes.LowerManaCost = 5;
			Attributes.ReflectPhysical = 5;
            Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			Resistances.Physical = 10;
			Resistances.Fire = 10;
			Resistances.Cold = 10;
			Resistances.Poison = 10;
			Resistances.Energy = 10;
*/
		}

        public DonationCharcoalBoneHelm(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Hue == 2052 )
				Hue = 2052;
		}
	}
}
