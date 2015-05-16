using System;
using System.Text;
using Server;
using Server.Items;

namespace VitaNex.Modules.AutoPvP.Battles
{
	public enum TrophyType
	{
		Damage,
		Kills,
		Healing,
        Walls,
        CrystalTime,
        FlagCaps,
        FlagDefends,
        FlagAssaults,
        First,
        Second,
        Third
	}

	[Flipable( 5020, 4647 )]
	public class BattlesTrophy : Item
	{
		private string m_Title;
        private TrophyType m_Type;
		private Mobile m_Owner;
		private DateTime m_Date;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Title{ get{ return m_Title; } set{ m_Title = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
        public TrophyType TType { get { return m_Type; } set { m_Type = value; UpdateStyle(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime Date{ get{ return m_Date; } }

		[Constructable]
        public BattlesTrophy(string title, TrophyType type)
            : base(5020)
		{
			m_Title = title;
            m_Type = type;
			m_Date = DateTime.UtcNow;

			LootType = LootType.Blessed;

			UpdateStyle();
		}

        public BattlesTrophy(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_Title );
            writer.Write((int)m_Type);
			writer.Write( (Mobile) m_Owner );
			writer.Write( (DateTime) m_Date );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Title = reader.ReadString();
            m_Type = (TrophyType)reader.ReadInt();
			m_Owner = reader.ReadMobile();
			m_Date = reader.ReadDateTime();

			if ( version == 0 )
				LootType = LootType.Blessed;
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( m_Owner == null )
				m_Owner = this.RootParent as Mobile;
		}

		public override void OnSingleClick( Mobile from )
		{
            LabelTo(from, "Battles Trophy");

			if ( m_Owner != null )
				LabelTo( from, "{0} -- {1}", m_Title, m_Owner.RawName );
			else if ( m_Title != null )
				LabelTo( from, m_Title );

			if ( m_Date != DateTime.MinValue )
				LabelTo( from, m_Date.ToString( "d" ) );
		}

		public void UpdateStyle()
		{
            Name = String.Format("{0} trophy", m_Type.ToString().ToLower());

            switch (m_Type)
			{
                case TrophyType.FlagAssaults: Hue = 1158; break;
                case TrophyType.FlagCaps: Hue = 1266; break;
                case TrophyType.FlagDefends: Hue = 1154; break;
                case TrophyType.CrystalTime: Hue = 2498; break;
                case TrophyType.Damage: Hue = 1258; break;
                case TrophyType.Kills: Hue = 1157; break;
                case TrophyType.Healing: Hue = 1150; break;
                case TrophyType.Walls: Hue = 2407; break;
                case TrophyType.First: Hue = 1912; break;
                case TrophyType.Second: Hue = 1910; break;
                case TrophyType.Third: Hue = 1944; break;
			}
		}
	}
}