using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class GCKeg : PotionKeg
	{
		[Constructable]
		public GCKeg()
		{
			Held = 100;
			Type = PotionEffect.CureGreater;
			Hue = 0x2D;
		}

		public GCKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}

	public class GHKeg : PotionKeg
	{
		[Constructable]
		public GHKeg()
		{
			Held = 100;
			Type = PotionEffect.HealGreater;
			Hue = 0x499;
		}

		public GHKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}
      	public class DPKeg : PotionKeg
	{
		[Constructable]
		public DPKeg()
		{
			Held = 100;
			Type = PotionEffect.PoisonDeadly;
			Hue = 0x46;
		}

		public DPKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}
      	public class TRKeg : PotionKeg
	{
		[Constructable]
		public TRKeg()
		{
			Held = 100;
			Type = PotionEffect.RefreshTotal;
			Hue = 0x21;
		}

		public TRKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}
      	public class GEKeg : PotionKeg
	{
		[Constructable]
		public GEKeg()
		{
			Held = 100;
			Type = PotionEffect.ExplosionGreater;
			Hue = 0x74;
		}

		public GEKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}
      	public class NSKeg : PotionKeg
	{
		[Constructable]
		public NSKeg()
		{
			Held = 100;
			Type = PotionEffect.Nightsight;
			Hue = 0x2C3;
		}

		public NSKeg( Serial serial ) : base( serial )
      		{
      		}

     		public override void Serialize( GenericWriter writer )
      		{
        		base.Serialize( writer );
         		writer.Write( (int) 0 );
      		}

      		public override void Deserialize( GenericReader reader )
      		{
         		base.Deserialize( reader );
         		int version = reader.ReadInt();
      		}
      	}
}