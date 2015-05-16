using System;
using Server;

namespace Server.Items
{
	public abstract class BaseBashing : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

		public BaseBashing( int itemID ) : base( itemID )
		{
		}

		public BaseBashing( Serial serial ) : base( serial )
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
            int stamdrained = (int)Math.Round((damage / 2) * SpecialMovesController._MaceStaminaDrainModifier,MidpointRounding.AwayFromZero);
            defender.Stam -= stamdrained;
            return base.AbsorbDamage(attacker, defender, damage);
        }

		public override double GetBaseDamage( Mobile attacker )
		{
			double damage = base.GetBaseDamage( attacker );

			if (!attacker.IsT2A && Layer == Layer.TwoHanded 
                && (attacker.Skills[SkillName.Anatomy].Value / 100.0 * SpecialMovesController._CrushingBlowMaxChance) >= Utility.RandomDouble() 
                && Engines.ConPVP.DuelContext.AllowSpecialAbility( attacker, "Crushing Blow", false ) )
			{
				damage *= SpecialMovesController._CrushingBlowDamageMultiplier;

				attacker.SendLocalizedMessage( 1060090 ); // You have delivered a crushing blow!
				attacker.PlaySound( 0x11C );
			}

			return damage;
		}
	}
}