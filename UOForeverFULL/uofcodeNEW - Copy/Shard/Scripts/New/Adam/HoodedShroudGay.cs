using Server.Mobiles;

namespace Server.Items
{
	public class HoodedShroudGay : BaseOuterTorso
	{

        private bool Enshrouded { get; set; }

        private Mobile Owner { get; set; }

		[Constructable]
		public HoodedShroudGay() : this( 1153 )
		{
		}

		[Constructable]
        public HoodedShroudGay(int hue)
            : base(7939, hue)
		{
		    Name = "a shroud";
			Dyable = true;
			Weight = 3.0;
		}

        public HoodedShroudGay(Serial serial)
            : base(serial)
		{
		}

	    public override void OnDoubleClick(Mobile from)
	    {
            PlayerMobile pm = from as PlayerMobile;

            Item item = from.FindItemOnLayer(Layer.OuterTorso);
	        if (item == this)
	        {
	            if (!Enshrouded && pm != null)
	            {
                    Name = "a hooded shroud";
	                Enshrouded = true;
	                pm.Criminal = true;
	                ItemID = 0x2684;
	                Owner = from;
	            }

                else if (Enshrouded && pm != null)
                {
                    Name = "a shroud";
                    Enshrouded = false;
                    ItemID = 7939;
                }
	        }
	        base.OnDoubleClick(@from);
	    }


        public override void OnLocationChange(Point3D oldLocation)
        {
            if (Enshrouded && Owner != null)
            {
                Enshrouded = false;
                ItemID = 7939;
                Owner = null;
                Name = "a shroud";
            }
            base.OnLocationChange(oldLocation);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            writer.Write(Enshrouded);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		    Enshrouded = reader.ReadBool();
		    if (Enshrouded)
		    {
		        Name = "a shroud";
		        Enshrouded = false;
		        ItemID = 7939;
		    }
		}
	}
}
