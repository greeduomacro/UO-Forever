using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public class TransgenderCostume : HalloweenMask
	{
		public override int BodyMod{ get{ return 0; } }
		public override int HueMod{ get{ return 0; } }
		public override int Rarity{ get{ return 4; } }
		public override string DefaultName{ get{ return "a gender changing costume"; } }

		[Constructable]
		public TransgenderCostume() : base ()
		{
		}

		public TransgenderCostume( Serial serial ) : base ( serial )
		{
		}

		public override void AddEffect()
		{
			if ( Parent is Mobile )
			{
				Mobile from = (Mobile)Parent;
				if ( from.Mounted )
				{
					IMount mount = (IMount)from.Mount;
					mount.Rider = null;
				}

				from.BodyMod = from.Female ? 400 : 401;
				TransformationSpellHelper.AddContext( from, new TransformContext( null, typeof(HalloweenMask), null ) );
			}
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