using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Multis;
using Server.Mobiles;


namespace Server.Items
{

	public class EurystheusBox : Item
	{
		[Constructable]
		public EurystheusBox() : this( null )
		{
		}

		[Constructable]
		public EurystheusBox ( string name ) : base ( 0x9A8 )
		{
			Name = "Eurystheus's Collection Box";
			LootType = LootType.Blessed;
			Hue = 0x11E;
		}

		public EurystheusBox( Serial serial ) : base ( serial )
		{
		}

      		
		public override void OnDoubleClick( Mobile m )

		{
			Item a = m.Backpack.FindItemByType( typeof(MareTail) );
			if ( a != null )
			{	
			Item b = m.Backpack.FindItemByType( typeof(CerberusBone) );
			if ( b != null )
			{
			Item c = m.Backpack.FindItemByType( typeof(ErymanthusHam) );
			if ( c != null )
			{
			Item d = m.Backpack.FindItemByType( typeof(GeryonCowHide) );
			if ( d != null )
			{
			Item e = m.Backpack.FindItemByType( typeof(GoldenApple) );
			if ( e != null )
			{
			Item f = m.Backpack.FindItemByType( typeof(HippolytesGirdle) );
			if ( f != null )
			{
			Item g = m.Backpack.FindItemByType( typeof(LernaHydraScale) );
			if ( g != null )
			{
			Item h = m.Backpack.FindItemByType( typeof(MadBullHorn) );
			if ( h != null )
			{
			Item i = m.Backpack.FindItemByType( typeof(NemeanTooth) );
			if ( i != null )
			{
			Item j = m.Backpack.FindItemByType( typeof(StableDung) );
			if ( j != null )
			{
			Item k = m.Backpack.FindItemByType( typeof(StagAntler) );
			if ( k != null )
			{
			Item l = m.Backpack.FindItemByType( typeof(StymphalianFeather) );
			if ( l != null )
			{
			
				m.AddToBackpack( new CompleteEurystheusBox() );
				a.Delete();
				b.Delete();
				c.Delete();
				d.Delete();
				e.Delete();
				f.Delete();
				g.Delete();
				h.Delete();
				i.Delete();
				j.Delete();
				k.Delete();
				l.Delete();
				m.SendMessage( "You have completed your tasks." );
				this.Delete();
			}
			}
			}
			}
			}
			}
			}
			}
			}
			}
			}
			}			
				else
			{
				m.SendMessage( "Are you forgetting something?" );
			}
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}