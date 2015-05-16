using System;
using System.Collections.Generic;
using System.Text;
using Server.Guilds;
using Server.Factions;

namespace Server.Ethics.Evil
{
	public sealed class EvilEthic : Ethic
	{
		public EvilEthic()
		{
			m_Definition = new EthicDefinition(
                    1175, 1100,
					"Evil", true, "(Spawn of Evil)",
					"I am evil incarnate",
					new Power[]
					{
						new VileBlade(),
						new UnholyItem(),
						new SummonFamiliar(),
						new UnholySense(),
						new UnholySteedPower(),
						new UnholyUnion(),
						new UnholyWord()
					},
					new RankDefinition[]
					{
						new RankDefinition(     0,	"Neophyte"),
						new RankDefinition(    10,	"Grunt"),
						new RankDefinition(    20,	"Delinquent"),
						new RankDefinition(    30,	"Outlaw"),
						new RankDefinition(    40,	"Defiler"),
						new RankDefinition(    60,	"Invader"),
						new RankDefinition(    80,	"Plunderer"),
						new RankDefinition(   100,	"Conqueror"),
						new RankDefinition(   150,	"Harbinger of Evil"),
						new RankDefinition(   200,	"Lord of Chaos"),
						new RankDefinition(   300,	"Avatar of Evil")
					}
				);
		}
	}
}