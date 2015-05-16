using System;
using Server;

namespace Server.Items
{
	public class GreaterExplosionPotion : BaseExplosionPotion
	{
//		public override int MinDamage { get { return /*Core.AOS ? 20 :*/ 15; } }
//		public override int MaxDamage { get { return /*Core.AOS ? 40 :*/ 30; } }
		//public override int Damage{ get{ return Utility.Dice( 3, 9, 2 ); } } // 15 - 31
        public override int Damage
        {
            get
            {
                return Utility.RandomMinMax(ExplosionPotionController._GreaterExplosionPotDmgMin,
                                            ExplosionPotionController._GreaterExplosionPotDmgMax);
            }
        } // was 14-18, but then multiplied by 0.75
        public override double Delay { get { return ExplosionPotionController._GreaterExplosionPotTimeDelay; } }

		[Constructable]
		public GreaterExplosionPotion() : base( PotionEffect.ExplosionGreater )
		{
		}

		public GreaterExplosionPotion( Serial serial ) : base( serial )
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