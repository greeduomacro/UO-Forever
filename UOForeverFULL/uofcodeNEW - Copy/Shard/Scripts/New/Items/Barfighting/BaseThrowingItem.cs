using Server;
using System;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Items
{
	public class BaseThrowingItem : Item
	{
		public virtual int DamageMin { get { return 1; } }
		public virtual int DamageMax { get { return 2; } }
		public virtual bool Break { get { return false; } }
		public virtual bool DeleteOnThrow { get { return true; } }

		public BaseThrowingItem() : base( 0x1C12 )
		{
			Name = "base throwing item";
			Weight = 5;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from == null||from.Backpack == null )
				return;

			if ( !IsChildOf(from.Backpack) )
			{
				from.SendMessage("You need to pick this up to use it!");
				return;
			}

			from.RevealingAction();
			from.Target = new ThrowTarget( this, DamageMin, DamageMax, Break, DeleteOnThrow );
		}

		private class ThrowTarget : Target
		{
			private BaseThrowingItem m_BaseThrowingItem;
			private int m_DamageMin;
			private int m_DamageMax;
			private bool m_Break;
			private bool m_DeleteOnThrow;

			public ThrowTarget( BaseThrowingItem bti, int min, int max, bool b, bool del ) : base( 8, true, TargetFlags.Harmful )
			{
				m_BaseThrowingItem = bti;
				m_DamageMin = min;
				m_DamageMax = max;
				m_Break = b;
				m_DeleteOnThrow = del;
			}

			protected override void OnTarget( Mobile from, object obj )
			{
				if ( m_BaseThrowingItem.Deleted || m_BaseThrowingItem.Map == Map.Internal)
					return;

				if ( obj is Mobile )
				{
					Mobile to = (Mobile)obj;

					if ( !from.CanBeHarmful( to ) )
					{
					}
					else
					{	from.Direction = from.GetDirectionTo( to );
						from.Animate( 11, 5, 1, true, false, 0 );
						from.MovingEffect( to, m_BaseThrowingItem.ItemID, 10, 0, false, false );

						Timer.DelayCall<ThrowInfo>( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback<ThrowInfo>( FinishThrow ), new ThrowInfo( from, to, m_DamageMin, m_DamageMax, m_Break, m_BaseThrowingItem ) );

						if ( m_DeleteOnThrow || m_Break )
							m_BaseThrowingItem.Delete();
					}
				}
				else
				{
					IPoint3D p = obj as IPoint3D;

					if ( p == null )
						return;

					Map map = from.Map;

					if ( map == null )
						return;

					IEntity to;

					to = new Entity( Serial.Zero, new Point3D( p ), map );

					from.Direction = from.GetDirectionTo( to );
					Effects.SendMovingEffect( from, to, m_BaseThrowingItem.ItemID & 0x3FFF, 7, 0, false, false, m_BaseThrowingItem.Hue, 0 );
					from.Animate( 11, 5, 1, true, false, 0 );

					if ( m_DeleteOnThrow )
					{
						m_BaseThrowingItem.Delete();
						from.SendMessage( "You miss the target and the {0} is wasted", m_BaseThrowingItem.Name );
					}
					else
					{
						Timer.DelayCall<object[]>( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback<object[]>( FinishMiss ), new object[]{ to, map, m_BaseThrowingItem } );
						from.SendMessage( "You miss the target" );
					}
				}
			}
		}

		private class ThrowInfo
		{
			public Mobile From;
			public Mobile To;
			public int DamageMin;
			public int DamageMax;
			public bool Break;
			public Item Item;

			public ThrowInfo( Mobile from, Mobile to, int min, int max, bool b, Item item )
			{
				From = from;
				To = to;
				DamageMin = min;
				DamageMax = max;
				Break = b;
				Item = item;
			}
		}

		private static void FinishThrow( ThrowInfo info )
		{
			Mobile from = info.From;
			Mobile to = info.To;
			int min = info.DamageMin;
			int max = info.DamageMax;
			bool b = info.Break;
			Item item = info.Item;

			to.Damage( Utility.RandomMinMax( min, max ), from );

			if ( b )
			{
				int count = Utility.RandomMinMax( 1, 4 );

				for ( int i = 0; i < count; ++i )
				{
					Point3D p = new Point3D( to.Location );

					p.X += Utility.RandomMinMax(-1, 1);
					p.Y += Utility.RandomMinMax(-1, 1);

					if ( !from.Map.CanFit( p.X, p.Y, p.Z, 16, false, true ) )
					{
						p.Z = from.Map.GetAverageZ( p.X, p.X );

						if ( p.Z == to.Z || !from.Map.CanFit( p.X, p.Y, p.Z, 16, false, true ) )
							continue;
					}

					StoolLeg leg = new StoolLeg();
					leg.MoveToWorld( p, from.Map );
				}
			}

			if ( item != null )
				item.MoveToWorld( to.Location, to.Map );
		}

		private static void FinishMiss( object[] states )
		{
			IPoint3D p = (IPoint3D)states[0];
			Map map = (Map)states[1];
			Item obj = (Item)states[2];

			Point3D loc = new Point3D( p );
			obj.MoveToWorld( loc, map );
		}

		public BaseThrowingItem( Serial serial ) : base( serial )
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

	public class ThrowingKnife : BaseThrowingItem
	{
		public override int DamageMin { get { return 7; } }
		public override int DamageMax { get { return 9; } }

		[Constructable]
		public ThrowingKnife() : base()
		{
			ItemID = 0x9F6;
			Name = "throwing knife";
		}

		public ThrowingKnife( Serial serial ) : base( serial )
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

	public class ThrowingStool : BaseThrowingItem
	{
		public override int DamageMin { get { return 9; } }
		public override int DamageMax { get { return 12; } }
		public override bool Break { get { return true; } }
		public override bool DeleteOnThrow { get { return false; } }

		[Constructable]
		public ThrowingStool() : base()
		{
			ItemID = 0xA2A;
			Name = "stool";
		}

		public ThrowingStool( Serial serial ) : base( serial )
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

	public class ThrowingSpittoon : BaseThrowingItem
	{
		public override int DamageMin { get { return 4; } }
		public override int DamageMax { get { return 7; } }

		[Constructable]
		public ThrowingSpittoon() : base()
		{
			ItemID = 0x1003;
			Name = "spittoon";
		}

		public ThrowingSpittoon( Serial serial ) : base( serial )
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

	public class ThrowingMug : BaseThrowingItem
	{
		public override int DamageMin { get { return 3; } }
		public override int DamageMax { get { return 9; } }

		[Constructable]
		public ThrowingMug() : base()
		{
			ItemID = 0xFFF;
			Name = "ale mug";
		}

		public ThrowingMug( Serial serial ) : base( serial )
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

	public class ThrowingPitcher : BaseThrowingItem
	{
		public override int DamageMin { get { return 2; } }
		public override int DamageMax { get { return 8; } }

		[Constructable]
		public ThrowingPitcher() : base()
		{
			ItemID = 0xFF6;
			Name = "an empty pitcher";
		}

		public ThrowingPitcher( Serial serial ) : base( serial )
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

	public class ThrowingChair : BaseThrowingItem
	{
		public override int DamageMin { get { return 7; } }
		public override int DamageMax { get { return 12; } }
		public override bool DeleteOnThrow { get { return false; } }

		[Constructable]
		public ThrowingChair() : base()
		{
			ItemID = 0xB57;
			Name = "a chair";
		}

		public ThrowingChair( Serial serial ) : base( serial )
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

	public class ThrowingBottle : BaseExplosionPotion
	{
//		public override int MinDamage { get { return 6; } }
//		public override int MaxDamage { get { return 12; } }
		public override int Damage{ get{ return Utility.Dice( 3, 3, 3 ); } }
		public override double Delay{ get{ return 3.0; } }

		[Constructable]
		public ThrowingBottle() : base( PotionEffect.Explosion )
		{
			ItemID = Utility.RandomList( 2459, 2503, 2463 );
		}

		public ThrowingBottle( Serial serial ) : base( serial )
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