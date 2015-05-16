using System;
using Server;

namespace Server.Items
{
	public abstract class BaseSpear : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x23C; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType DefType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Pierce2H; } }

		public BaseSpear( int itemID ) : base( itemID )
		{
		}

		public BaseSpear( Serial serial ) : base( serial )
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

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			if ( !attacker.IsT2A && Layer == Layer.TwoHanded
                && (attacker.Skills[SkillName.Anatomy].Value / 100.0 * SpecialMovesController._SpearStunMaxChance) >= Utility.RandomDouble() 
                && Engines.ConPVP.DuelContext.AllowSpecialAbility( attacker, "Paralyzing Blow", false ) )
			{
				defender.SendLocalizedMessage( 1060164 ); // The attack has temporarily paralyzed you!
                defender.Freeze(TimeSpan.FromSeconds( SpecialMovesController._SpearStunDurationSeconds ));

				attacker.SendLocalizedMessage( 1060163 ); // You deliver a paralyzing blow!
				attacker.PlaySound( 0x11C );
			}

			if ( Poison != null && PoisonCharges > 0 && defender.Poison != Poison)
			{
			    bool chargeConsume = false;
                double chance = 
                    (attacker.Skills[SkillName.Tactics].Value + attacker.Skills[OldSkill].Value) / 200.0 * SpecialMovesController._PoisonChanceSpears
                    + (attacker.Skills[SkillName.Poisoning].Value / 100.0 * SpecialMovesController._PoisonSkillChanceMaxBonus);
				if ( chance > Utility.RandomDouble() ) // 50% chance to poison @ GM
				{
				    chargeConsume = true;
                    --PoisonCharges;
					defender.ApplyPoison( attacker, Poison );
				}
                if (!chargeConsume && 0.25 > Utility.RandomDouble())
                    --PoisonCharges;
			}
		}
	}
}