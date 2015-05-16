using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Network;

namespace Server.Items
{
	public class IronMaidenComponent : AddonComponent
	{
		public override int LabelNumber { get { return 1076288; } } // Iron Maiden
        //private Mobile LastUsed = null;

		public IronMaidenComponent() : base( 0x1249 )
		{
		}

		public IronMaidenComponent( Serial serial ) : base( serial )
		{
		}

		public void Activate( Mobile from )
		{
			ItemID += 1;
            if (from != null)
                from.Direction = Direction.Down;
            if (ItemID >= 0x124D)
            {
                // blood
                int amount = Utility.RandomMinMax(3, 7);

                for (int i = 0; i < amount; i++)
                {
                    int x = X + Utility.RandomMinMax(-1, 1);
                    int y = Y + Utility.RandomMinMax(-1, 1);
                    int z = Z;

                    if (!Map.CanFit(x, y, z, 1, false, false, true))
                    {
                        z = Map.GetAverageZ(x, y);

                        if (!Map.CanFit(x, y, z, 1, false, false, true))
                            continue;
                    }

                    Blood blood = new Blood(Utility.RandomMinMax(0x122C, 0x122F));
                    blood.MoveToWorld(new Point3D(x, y, z), Map);
                }

                if (from.Female)
                    from.PlaySound(Utility.RandomMinMax(0x150, 0x153));
                else
                    from.PlaySound(Utility.RandomMinMax(0x15A, 0x15D));

                from.LocalOverheadMessage(MessageType.Regular, 0, 501777); // Hmm... you suspect that if you used this again, it might hurt.
                SpellHelper.Damage(TimeSpan.Zero, from, Utility.RandomMinMax(7, 20));

                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(1), new TimerStateCallback<Mobile>(Deactivate), from);
            }
		}

		private void Deactivate(Mobile from)
		{
			ItemID = 0x1249;
            if (from != null)
                from.Frozen = false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class IronMaidenAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new IronMaidenDeed(); } }

		public IronMaidenAddon() : base()
		{
			AddComponent( new IronMaidenComponent(), 0, 0, 0 );
		}

		public IronMaidenAddon( Serial serial ) : base( serial )
		{
		}

		public override void OnComponentUsed( AddonComponent c, Mobile from )
		{
			if ( from.InRange( Location, 2 ) )
			{
				if ( Utility.RandomBool() )
				{
					c.ItemID = 0x124A;
                    from.Location = Location;

                    if (c is IronMaidenComponent)
                    {
                        from.Frozen = true;
                        Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 3, new TimerStateCallback<Mobile>(((IronMaidenComponent)c).Activate), from);
                    }
				}
				else
					from.LocalOverheadMessage( MessageType.Regular, 0, 501777 ); // Hmm... you suspect that if you used this again, it might hurt.
			}
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class IronMaidenDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new IronMaidenAddon(); } }
		public override int LabelNumber { get { return 1076288; } } // Iron Maiden

		[Constructable]
		public IronMaidenDeed() : base()
		{
			LootType = LootType.Blessed;
		}

		public IronMaidenDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}