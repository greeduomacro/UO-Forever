using System;
using Server;

namespace Server.Items
{
	public class LesserExplosionPotion : BaseExplosionPotion
	{
//		public override int MinDamage { get { return 5; } }
//		public override int MaxDamage { get { return 10; } }
	//	public override int Damage{ get{ return Utility.Dice( 1, 10, 0 ); } } // 4 - 10
        public override int Damage
        {
            get
            {
                return Utility.RandomMinMax(ExplosionPotionController._LesserExplosionPotDmgMin,
                                ExplosionPotionController._LesserExplosionPotDmgMax);
            }
        }
        public override double Delay { get { return ExplosionPotionController._LesserExplosionPotTimeDelay; } }

		[Constructable]
		public LesserExplosionPotion() : base( PotionEffect.ExplosionLesser )
		{
		}

		public LesserExplosionPotion( Serial serial ) : base( serial )
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