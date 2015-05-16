using System;
using Server;
using Server.Misc;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x13E3, 0x13E4 )]
	public class SmithHammer : BaseTool
	{
		public override CraftSystem CraftSystem{ get{ return DefBlacksmithy.CraftSystem; } }

		//[Constructable]
		public SmithHammer() : base( 0x13E3 )
		{
			Weight = 8.0;
			Layer = Layer.OneHanded;
		}

		//[Constructable]
		public SmithHammer( int uses ) : base( uses, 0x13E3 )
		{
			Weight = 8.0;
			Layer = Layer.OneHanded;
		}

		public SmithHammer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
			{
				SmithHammerWeapon hammer = new SmithHammerWeapon( UsesRemaining );
				Timer.DelayCall<Item>( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback<Item>( Replace_Item ), hammer );
			}
		}

		public void Replace_Item( Item hammer )
		{
			Cleanup.ReplaceItem( this, hammer );
		}
	}
}