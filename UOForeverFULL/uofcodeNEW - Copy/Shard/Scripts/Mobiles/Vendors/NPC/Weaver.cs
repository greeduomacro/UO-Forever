using System;
using System.Collections.Generic;
using Server;
using Server.Engines.BulkOrders;
using Server.Factions;
using Server.Games;

namespace Server.Mobiles
{
	public class Weaver : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.TailorsGuild; } }

		[Constructable]
		public Weaver() : base( "the weaver" )
		{
			SetSkill( SkillName.Tailoring, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBWeaver() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Sandals; }
		}

		#region Bulk Orders
		public override Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm != null && pm.NextTailorBulkOrder <= TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()) )
			{
				double theirSkill = pm.Skills[SkillName.Tailoring].Base;

				if ( theirSkill >= 85.1 )
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 6.0 );
				else if ( theirSkill >= 70.1 )
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 3.0 );
				else
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 1.5 );

				if ( theirSkill >= 75.1 && ((theirSkill - 75.0) / 250.0) > Utility.RandomDouble() )
					return new LargeTailorBOD();

				return SmallTailorBOD.CreateRandomFor( from );
			}

			return null;
		}

		public override bool IsValidBulkOrder( Item item )
		{
			return ( item is SmallTailorBOD || item is LargeTailorBOD );
		}

		public override bool SupportsBulkOrders( Mobile from )
		{
			Faction fact = Faction.Find( from );
			return ( FactionAllegiance == null || fact == null || FactionAllegiance == fact ) && ( from is PlayerMobile && from.Skills[SkillName.Tailoring].Base > 55.0 );
		}

		public override TimeSpan GetNextBulkOrder( Mobile from )
		{
			if ( from is PlayerMobile )
				return ((PlayerMobile)from).NextTailorBulkOrder;

			return TimeSpan.Zero;
		}

		public override void OnSuccessfulBulkOrderReceive( Mobile from, Item dropped )
		{
			PlayerMobile pm = from as PlayerMobile;

			if( pm != null )
			{
				//if ( /*Core.AOS ||*/ dropped is LargeTailorBOD )
                // always reset
                pm.NextTailorBulkOrder = TimeSpan.FromHours(PseudoSeerStone.HoursBetweenBODs);
				/*else
				{
					pm.NextTailorBulkOrder -= TimeSpan.FromHours( 2.0 );
					if ( pm.NextTailorBulkOrder < TimeSpan.Zero )
						pm.NextTailorBulkOrder = TimeSpan.Zero;
				}*/
			}
		}
		#endregion

		public Weaver( Serial serial ) : base( serial )
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
	}
}