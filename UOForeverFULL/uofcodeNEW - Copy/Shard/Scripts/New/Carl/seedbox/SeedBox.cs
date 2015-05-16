using System;
using Server;
using Server.Items;
using Custom.Gumps;
using Server.Mobiles;
using Server.Engines.Plants;

namespace Server.Items
{

	[ FlipableAttribute( 0xE41, 0xE40 ) ]
    public class SeedBox : BaseContainer
    {
		private static int m_maxSeeds = 5000; //maximum amount of seeds a Seed Box can hold

		public int[ , ] m_counts = new int[ 17, 19 ];

        private static int[] m_Hues = new int[]
        {
            0x455, 0x481, 0x66D, 0x21,
            0x59B, 0x42, 0x53D, 0x5,
            0x8A5, 0x38, 0x46F, 0x2B,
            0xD, 0x10, 
            0x486, 0x48E, 0x489, 0x495, 
            0x0
        };

        [ Constructable ]
        public SeedBox() : base( 0xE41 )
        {
			Movable = true;
			Weight = 60.0;
			Hue = 0x10F;
			Name = "Seed Box";
        }

        public override bool DisplaysContent{ get{ return false; } }

        public override void OnDoubleClick( Mobile from )
        {
//			from.SendMessage ("The seed box is temporarily out of order.");
//			return;

            if ( Movable )
            {
                from.SendMessage( "You haven't locked it down!" );
                return ;
            }

            if ( !from.InRange( GetWorldLocation(), 2 ) )
                from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            else if ( from is PlayerMobile )
                from.SendGump( new SeedBoxGump( ( PlayerMobile ) from, this ) );
        }

        public SeedBox( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);
			list.Add( 1060662,"{0}\t{1}" ,"Seeds", SeedCount() );
		}


        public override void Serialize( GenericWriter writer )
        {
			base.Serialize( writer );

            writer.Write( ( int ) 0 ); // version
            for ( int i = 0;i < 17;i++ )
				for ( int j = 0;j < 19;j++ )
					writer.Write( ( int ) m_counts[ i, j ] );
        }

        public override void Deserialize( GenericReader reader )
        {
			base.Deserialize( reader );

            int version = reader.ReadInt(); // version

            if ( version == 0 )
				for ( int i = 0;i < 17;i++ )
					for ( int j = 0;j < 19;j++ )
						m_counts[ i, j ] = reader.ReadInt();
		}

        public static int GetSeedHue( int Hue )
        {
			for ( int i = 0;i < 19;i++ )
				if ( m_Hues[ i ] == Hue )
					return i;
            return 0;
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
			if ( Movable )
                    return false;

            if ( !( dropped is Seed ) )
            {
                from.SendMessage( "You can only store seeds in this box." );
                return false;
            }

			if( SeedCount() > m_maxSeeds )
			{
				from.SendMessage( "This seed box cannot hold any more seeds." );
				return false;
			}

            Seed seed = ( Seed ) dropped;

			if( !seed.ShowType )
			{
				from.SendMessage( "You cannot store that seed since you are are not sure of its type." );
				return false;
			}

            //int type = (int)seed.PlantType + 8;
            //int hue = GetSeedHue( seed.Hue );
			int type = ConvertType( seed.PlantType );
			int hue = ConvertHue( seed.PlantHue );

            m_counts[ type, hue ] ++;
            dropped.Delete();
			InvalidateProperties();
            return true;
        }

		public static int ConvertHue( PlantHue hue )
		{
//			  0x455, 0x481, 0x66D, 0x21,
//            0x59B, 0x42, 0x53D, 0x5,
//            0x8A5, 0x38, 0x46F, 0x2B,
//            0xD, 0x10, 
//            0x486, 0x48E, 0x489, 0x495, 
//            0x0

			//convert from distro plant hue
			switch( hue )
			{	
				case PlantHue.Black        : return 0 ;  //Black = 0x455
				case PlantHue.White        : return 1 ;  //White = 0x481,
				case PlantHue.Red          : return 2 ;  //Red = 0x66D,
				case PlantHue.BrightRed    : return 3 ;  //BrightRed = 0x21,
				case PlantHue.Green        : return 4 ;  //Green = 0x59B,
				case PlantHue.BrightGreen  : return 5 ;  //BrightGreen = 0x42,
				case PlantHue.Blue         : return 6 ;  //Blue = 0x53D,
				case PlantHue.BrightBlue   : return 7 ;  //BrightBlue = 0x5,
				case PlantHue.Yellow       : return 8 ;  //Yellow = 0x8A5,
				case PlantHue.BrightYellow : return 9 ;  //BrightYellow = 0x38,
				case PlantHue.Orange       : return 10;  //Orange = 0x46F,
				case PlantHue.BrightOrange : return 11;  //BrightOrange = 0x2B,
				case PlantHue.Purple       : return 12;  //Purple = 0xD,
				case PlantHue.BrightPurple : return 13;  //BrightPurple = 0x10,
				case PlantHue.Magenta      : return 14;  //RareMagenta = 0x486,
				case PlantHue.Pink         : return 15;  //RarePink = 0x48E,
				case PlantHue.FireRed      : return 16;  //RareFireRed = 0x489
				case PlantHue.Aqua         : return 17;  //RareAqua = 0x495,
				case PlantHue.Plain        : return 18;  //Plain = 0,
				default                    : return 18;  //Plain
			}	
		}

		public static int ConvertType( PlantType type )
		{
			//convert from distro plant type
			switch( type )
			{
				case PlantType.CampionFlowers   : return 0 ; // campion flowers 1st
				case PlantType.Poppies          : return 1 ; // poppies
				case PlantType.Snowdrops        : return 2 ; // snowdrops
				case PlantType.Bulrushes        : return 3 ; // bulrushes
				case PlantType.Lilies           : return 4 ; // lilies
				case PlantType.PampasGrass      : return 5 ; // pampas grass
				case PlantType.Rushes           : return 6 ; // rushes
				case PlantType.ElephantEarPlant : return 7 ; // elephant ear plant
				case PlantType.Fern             : return 8 ; // fern 1st
				case PlantType.PonytailPalm     : return 9 ; // ponytail palm
				case PlantType.SmallPalm        : return 10; // small palm
				case PlantType.CenturyPlant     : return 11; // century plant
				case PlantType.WaterPlant       : return 12; // water plant
				case PlantType.SnakePlant       : return 13; // snake plant
				case PlantType.PricklyPearCactus: return 14; // prickly pear cactus
				case PlantType.BarrelCactus     : return 15; // barrel cactus
				case PlantType.TribarrelCactus  : return 16; // tribarrel cactus 1st
				default                         : return 0 ;
			}
		}

		public int SeedCount()
		{
			int count = 0;
			
			for ( int i = 0;i < 17;i++ )
				for ( int j = 0;j < 19;j++ )
					count += m_counts[ i, j ];

			return count;
		}
    }
}
