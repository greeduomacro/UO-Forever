using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class RemovePowerscrolls
	{
		public static void Initialize()
		{
			CommandSystem.Register( "RemovePowerscrolls", AccessLevel.Owner, new CommandEventHandler( RemovePowerscrolls_OnCommand ) );
		}

		[Usage( "RemovePowerscrolls" )]
		[Description( "Removes powerscrolls and statscrolls." )]
		private static void RemovePowerscrolls_OnCommand( CommandEventArgs e )
		{
			foreach ( Mobile mob in World.Mobiles.Values )
				if ( mob.AccessLevel == AccessLevel.Player )
					FixPlayer( mob as PlayerMobile );
		}
		
		public static void FixPlayer( PlayerMobile mob )
		{
			if ( mob != null )
			{
				Server.Skills skills = mob.Skills;

				int totalDecrease = 0;

				for ( int i = 0; i < skills.Length; i++ )
				{
					Skill skill = skills[i];

					if ( skill.CapFixedPoint > 1000 )
						skill.CapFixedPoint = 1000;

					if ( skill.BaseFixedPoint > 1000 )
					{
						totalDecrease += skill.BaseFixedPoint - 1000;
						skill.BaseFixedPoint = 1000;
					}
				}

				mob.SkillsCap = 7000;

				int totalGiveSkill = Math.Min( totalDecrease, 7000 - skills.Total );

				if ( totalGiveSkill > 0 )
				{
					EtherealSoulStone stone = new EtherealSoulStone( mob.Account.Username );
					stone.SkillValue = (totalGiveSkill / 10.0);
					mob.AddToBackpack( stone );
				}
			}
		}
	}
}