using System;
using System.Collections.Generic;
using System.Text;
using Server.Ethics.Evil;
using Server.Factions;
using Server.Guilds;

namespace Server.Ethics.Hero
{
	public sealed class HeroEthic : Ethic
	{
		public HeroEthic()
		{
		    m_Definition = new EthicDefinition(
		        2955, 2049,
					"Hero", false, "(Spirit of the Just)",
					"I will defend the virtues",
					new Power[]
					{
						new HolyItem(),
						new SummonFamiliar(),	
						new HolySense(),
						new HolySteedPower(),
                        new HolyUnion(), 
						new HolyWord(),
                        new HolyBlade(),
					},
					new RankDefinition[]
					{
						new RankDefinition(     0,	"Paige"),
						new RankDefinition(    10,	"Squire"),
						new RankDefinition(    20,	"Errant"),
						new RankDefinition(    30,	"Lance"),
						new RankDefinition(    40,	"Sergeant"),
						new RankDefinition(    60,	"Knight"),
						new RankDefinition(    80,	"Knight-errant"),
						new RankDefinition(   100,	"Crusader"),
						new RankDefinition(   150,	"Prophet"),
						new RankDefinition(   200,	"Paladin"),
						new RankDefinition(   300,	"Avatar of Good")
					}
				);
		}
	}
}