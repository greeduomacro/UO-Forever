using System;
using Server;
using Server.Misc;

namespace Server.Items
{
	public class IDWand : BaseWand
	{
		public override TimeSpan GetUseDelay{ get{ return TimeSpan.Zero; } }

		[Constructable]
		public IDWand() : base( WandEffect.Identification, 25, 175 )
		{
		}

		public IDWand( Serial serial ) : base( serial )
		{
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

		public override bool OnWandTarget( Mobile from, object o )
		{
			Item item = o as Item;

			if ( !AutoRestart.ServerWars && item != null )
			{
				if ( o is BaseWeapon )
					((BaseWeapon)o).Identified = true;
				else if ( o is BaseArmor )
					((BaseArmor)o).Identified = true;
	//            else if (o is BaseJewel)
	//                ((BaseJewel)o).Identified = true;

				//if ( !Core.AOS )
					((Item)o).OnSingleClick( from );

				return true;
			}

			return false;
		}
	}
}