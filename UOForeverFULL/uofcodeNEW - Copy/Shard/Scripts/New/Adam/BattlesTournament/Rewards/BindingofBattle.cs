using System;
using VitaNex.FX;

namespace Server.Items
{
	[Flipable]
    public class BindingofBattle : Item
	{

	    private DateTime LastUsed;

		[Constructable]
		public BindingofBattle()
            : base(4230)
		{
		    Layer = Layer.Bracelet;
			Weight = 1.0;
			Dyable = false;
            LootType = LootType.Blessed;
		    Name = "Binding of Battle";
		}

        public BindingofBattle(Serial serial)
            : base(serial)
		{
		}

	    public override void OnDoubleClick(Mobile from)
	    {
            if (DateTime.UtcNow >= LastUsed && ParentEntity != null && ParentEntity == from)
	        {
	            int range = Utility.RandomMinMax(5, 7);
	            int zOffset = from.Mounted ? 20 : 10;

	            Point3D src = from.Location.Clone3D(0, 0, zOffset);
	            var points = src.GetAllPointsInRange(from.Map, range, range);

	            Effects.PlaySound(from.Location, from.Map, 0x19C);

	            Timer.DelayCall(
	                TimeSpan.FromMilliseconds(100),
	                () =>
	                {
	                    foreach (Point3D trg in points)
	                    {
	                        int bloodID = Utility.RandomMinMax(4650, 4655);

	                        new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), from.Map, bloodID).MovingImpact(
	                            info =>
	                            {
	                                var blood = new Blood
	                                {
	                                    ItemID = bloodID
	                                };
	                                blood.MoveToWorld(info.Target.Location, info.Map);

	                                Effects.PlaySound(info.Target, info.Map, 0x028);
	                            });
	                    }
	                });
	            LastUsed = DateTime.UtcNow + TimeSpan.FromMinutes(30);
	        }
            else if (ParentEntity == null || from != ParentEntity)
            {
                from.SendMessage(54, "You must be wearing this to use it.");
                return;
            }
	        else
	        {
	            var nextuse = LastUsed - DateTime.UtcNow;
                from.SendMessage("You cannot use this again for another " + nextuse.Minutes + " minutes.");
	        }
	        base.OnDoubleClick(from);
	    }

	    public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			if ( IsArcane )
			{
				writer.Write( true );
				writer.Write( (int) m_CurArcaneCharges );
				writer.Write( (int) m_MaxArcaneCharges );
			}
			else
			{
				writer.Write( false );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					if ( reader.ReadBool() )
					{
						m_CurArcaneCharges = reader.ReadInt();
						m_MaxArcaneCharges = reader.ReadInt();

						if ( Hue == 2118 )
							Hue = ArcaneGem.DefaultArcaneHue;
					}

					break;
				}
			}
		}

		#region Arcane Impl
		private int m_MaxArcaneCharges, m_CurArcaneCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxArcaneCharges
		{
			get{ return m_MaxArcaneCharges; }
			set{ m_MaxArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurArcaneCharges
		{
			get{ return m_CurArcaneCharges; }
			set{ m_CurArcaneCharges = value; InvalidateProperties(); Update(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsArcane
		{
			get{ return ( m_MaxArcaneCharges > 0 && m_CurArcaneCharges >= 0 ); }
		}

		public void Update()
		{
			if ( IsArcane )
				ItemID = 0x26B0;
			else if ( ItemID == 0x26B0 )
				ItemID = 0x13C6;

			if ( IsArcane && CurArcaneCharges == 0 )
				Hue = 0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( IsArcane )
				list.Add( 1061837, "{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges ); // arcane charges: ~1_val~ / ~2_val~
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( IsArcane )
				LabelTo( from, 1061837, String.Format( "{0}\t{1}", m_CurArcaneCharges, m_MaxArcaneCharges ) );
		}

		public void Flip()
		{
			if ( ItemID == 0x13C6 )
				ItemID = 0x13CE;
			else if ( ItemID == 0x13CE )
				ItemID = 0x13C6;
		}
		#endregion
	}
}