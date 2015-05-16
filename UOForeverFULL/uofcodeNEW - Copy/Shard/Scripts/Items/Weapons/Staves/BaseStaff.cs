using System;
using Server;

namespace Server.Items
{
	public abstract class BaseStaff : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Staff; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

		public BaseStaff( int itemID ) : base( itemID )
		{
		}

		public BaseStaff( Serial serial ) : base( serial )
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

        public override int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
        {
            int stamdrained = (int)Math.Round((damage / 3) * SpecialMovesController._MaceStaminaDrainModifier, MidpointRounding.AwayFromZero);
            defender.Stam -= stamdrained;
            return base.AbsorbDamage(attacker, defender, damage);
        }
	}
}