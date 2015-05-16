using System;
using Server.Network;

namespace Server.Items
{
	public class MoleHide : BaseGMJewel
	{
		private class MoleInfo
		{
			public Mobile From;
			public int Count;

			public MoleInfo( Mobile from )
			{
				From = from;
			}
		}

		public override bool CastHide{ get{ return false; } }

		public override void HideEffects(Mobile from)
		{
			if (from.Hidden)
			{
				from.Z -= 10;
				from.Hidden = false;
				Timer.DelayCall<MoleInfo>( TimeSpan.FromMilliseconds( 100 ), TimeSpan.FromMilliseconds( 100 ), 10, new TimerStateCallback<MoleInfo>( DoIncZ_Callback ), new MoleInfo( from ) );
			}
			else
				Timer.DelayCall<MoleInfo>( TimeSpan.FromMilliseconds( 100 ), TimeSpan.FromMilliseconds( 100 ), 10, new TimerStateCallback<MoleInfo>( DoDecZ_Callback ), new MoleInfo( from ) );

			from.PlaySound( 0x244 );
		}

		private void DoIncZ_Callback( MoleInfo info )
		{
			info.From.Z++;
			info.Count++;
			if ( info.Count > 9 )
				info.From.EndAction( typeof( MoleHide ) );
		}

		private void DoDecZ_Callback( MoleInfo info )
		{

			info.From.Z++;
			info.Count++;
			if ( info.Count > 9 )
			{
				info.From.EndAction( typeof( MoleHide ) );
				info.From.Hidden = true;
				info.From.Z += 10;
			}
		}

		[Constructable]
		public MoleHide() : base( AccessLevel.GameMaster, 0xCB, 0x1ECD )
		{
			Name = "GM Mole Ball";
			Hue = 1717;
		}

		public MoleHide( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.BeginAction( typeof(MoleHide) ) )
				base.OnDoubleClick( from );
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