using System;

namespace Server.Items
{
	public class PutridCuirass : BaseShirt
	{
		[Constructable]
		public PutridCuirass() : this( 0 )
		{
		    LootType = LootType.Blessed;
		}

		[Constructable]
        public PutridCuirass(int hue)
            : base(5199, hue)
		{
		    Name = "Putrid Cuirass [Aesthetic]";
			Weight = 2.0;
            LootType = LootType.Blessed;
            Layer = Layer.OuterTorso;
		}

		public PutridCuirass( Serial serial ) : base( serial )
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Crafted with flesh and sinew from the denizens of Zombieland", 61);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}