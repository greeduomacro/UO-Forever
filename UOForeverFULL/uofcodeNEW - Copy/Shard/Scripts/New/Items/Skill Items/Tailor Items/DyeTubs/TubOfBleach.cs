using System;

namespace Server.Items
{
	public class TubOfBleach : DyeTub
	{
		public override string DefaultName{ get{ return "a tub of bleach"; } }

		[Constructable]
		public TubOfBleach() : base( 1072, false, 5 )
		{
		}

		public override bool IsDyable( Item item )
		{
			return base.IsDyable( item ) || item.DyeType.IsAssignableFrom( typeof(LeatherDyeTub) ) || item.DyeType.IsAssignableFrom( typeof(FurnitureDyeTub) ) || item.DyeType.IsAssignableFrom( typeof(RunebookDyeTub) ) || item.DyeType.IsAssignableFrom( typeof(StatuetteDyeTub) ) || item.DyeType.IsAssignableFrom( typeof(SpellbookDyeTub) );
		}

		public TubOfBleach( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}