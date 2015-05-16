using System;
using Server.Items;

namespace Server
{
	public abstract class BaseBoneLootSet : BaseLootSet
	{
		private int m_MinIntensity;
		private int m_MaxIntensity;
		private Type m_BoneType;

		public BaseBoneLootSet( int minInt, int maxInt, Type boneType ) : base()
		{
			m_MinIntensity = minInt;
			m_MaxIntensity = maxInt;
			m_BoneType = boneType;
		}

		public override Tuple<Item[],int> GenerateLootItem( Mobile creature )
		{
			BaseBone bone = Activator.CreateInstance( m_BoneType ) as BaseBone;

			if ( bone == null )
				throw new Exception( String.Format( "Type {0} is not BaseBone or could not be instantiated.", m_BoneType ) );

			int value = BaseValue;

			bone.BoneType = (BoneType)GetBonus();

			switch ( bone.BoneType )
			{
				case BoneType.Preserved: value += 50; break;
				case BoneType.Ancient: value += 100; break;
				case BoneType.Fossilized: value += 250; break;
				case BoneType.Ethereal: value += 500; break;
				case BoneType.Demonic: value += 1500; break;
			}

			return new Tuple<Item[],int>( new Item[]{ bone }, value );
		}

		private int GetBonus()
		{
			if ( m_MinIntensity == 0 && m_MaxIntensity == 0 )
				return 0;

			int rnd = Utility.RandomMinMax( m_MinIntensity, m_MaxIntensity );

			if ( rnd > 99 ) //1%
				return 6;
			else if ( rnd > 94 ) //5%
				return 5;
			else if ( rnd > 79 ) //15%
				return 3;
			else if ( rnd > 59 ) //15%
				return 2;
			else if ( rnd > 34 ) //20%
				return 1;
			else //34%
				return 0;
		}
	}
}