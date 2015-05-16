using System;
using Server.Items;

namespace Server
{
	public class BaseInstrumentLootSet : BaseLootSet
	{
		private double m_SlayerChance;

		private static Type[] m_InstrumentTypes = new Type[]
			{
				typeof( Drums ),				typeof( Harp ),					typeof( LapHarp ),
				typeof( Lute ),					typeof( Tambourine ),			typeof( TambourineTassel )
			};

		public override int BaseValue{ get{ return 100; } }

		public BaseInstrumentLootSet() : this( 0.10 )
		{
		}

		public BaseInstrumentLootSet( double slayerChance ) : base()
		{
			m_SlayerChance = Math.Max( Math.Min( slayerChance, 1.0 ), 0.0 );
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			Type type = m_InstrumentTypes[Utility.Random(m_InstrumentTypes.Length)];
			BaseInstrument instrument = Activator.CreateInstance( type ) as BaseInstrument;

			if ( instrument == null )
				throw new Exception( String.Format( "Type {0} is not BaseInstrument or could not be instantiated.", type ) );

			int value = 50;

			if ( m_SlayerChance > Utility.RandomDouble() )
			{
				if ( creature != null )
					instrument.Slayer = SlayerGroup.GetLootSlayerType( creature.GetType() );
				else
					instrument.Slayer = BaseRunicTool.GetRandomSlayer();

				value += 100;
			}

			if ( 0.08 > Utility.RandomDouble() ) // chance that this is low quality
			{
				instrument.Quality = InstrumentQuality.Low;
				value -= 50;
			}

			return new Tuple<Item[], int>( new Item[]{ instrument }, value );
		}
	}
}