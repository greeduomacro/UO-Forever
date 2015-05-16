#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Items
{
	public class EtherealSoulStone : SoulStone
	{
		//private double m_Charge;

		public override string DefaultName { get { return "ethereal soulstone"; } }
		public override int LabelNumber { get { return 0; } }

		[Constructable]
		public EtherealSoulStone()
			: this(null)
		{ }

		[Constructable]
		public EtherealSoulStone(string account)
			: base(account)
		{
			Hue = 1159;
		}

		/*
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060658, "charge\t{0:0.0}", m_Charge ); // charge: ~1_val~
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double Charge{ get{ return m_Charge; } set{ m_Charge = value; InvalidateProperties(); } }
*/
		public override bool Nontransferable { get { return true; } }

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			LabelTo(from, String.Format("Available Skill Points: {0:0.0}%", SkillValue));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version

			//writer.Write( m_Charge );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadEncodedInt();

			//m_Charge = reader.ReadDouble();
		}

		public EtherealSoulStone(Serial serial)
			: base(serial)
		{ }

		/*
		protected override bool CheckUse( Mobile from )
		{
			bool canUse = base.CheckUse( from );

			if( canUse )
			{
				if( m_Charge <= 0 )
				{
					Delete();
					return false;
				}
			}

			return canUse;
		}
*/

		public override void OnDoubleClick(Mobile from)
		{
			if (CheckUse(from))
			{
				from.CloseGump(typeof(SelectSkillGump));
				from.CloseGump(typeof(ConfirmSkillGump));
				from.CloseGump(typeof(ConfirmTransferGump));
				from.CloseGump(typeof(ConfirmRemovalGump));
				from.CloseGump(typeof(ErrorGump));
				from.CloseGump(typeof(SkillTransferGump));

				if (IsEmpty)
				{
					from.SendMessage("The soulstone is empty.");
				}
					//from.SendGump( new SelectSkillGump( this, from ) );
				else
				{
					from.SendGump(new SkillTransferGump(this, from));
				}
				//from.SendGump( new ConfirmTransferGump( this, from ) );
			}
		}
	}

	public class SkillTransferGump : Gump
	{
		private const int FieldsPerPage = 14;

		private Skill m_Skill;
		private readonly EtherealSoulStone m_SoulStone;

		public SkillTransferGump(EtherealSoulStone soulstone, Mobile from)
			: base(20, 30)
		{
			m_SoulStone = soulstone;

			AddPage(0);
			AddBackground(0, 0, 260, 351, 5054);

			AddImageTiled(10, 10, 240, 23, 0x52);
			AddImageTiled(11, 11, 238, 21, 0xBBC);

			AddLabel(45, 11, 0, "Select a skill to raise");

			AddPage(1);

			int page = 1;
			int index = 0;

			Skills skills = from.Skills;

			int number;

			if (from.EraSA)
			{
				number = 0;
			}
			else if (from.EraML)
			{
				number = 3;
			}
			else if (from.EraSE)
			{
				number = 4;
			}
			else if (from.EraAOS)
			{
				number = 6;
			}
			else
			{
				number = 9;
			}

			for (int i = 0; i < (skills.Length - number); ++i)
			{
				if (index >= FieldsPerPage)
				{
					AddButton(231, 13, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1);

					++page;
					index = 0;

					AddPage(page);

					AddButton(213, 13, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1);
				}

				Skill skill = skills[i];

				if ( /*(skill.Base + m_SoulStone.SkillValue) <= skill.Cap*/ skill.Base < skill.Cap && skill.Lock != SkillLock.Locked &&
																			skill.Lock != SkillLock.Down)
				{
					AddImageTiled(10, 32 + (index * 22), 240, 23, 0x52);
					AddImageTiled(11, 33 + (index * 22), 238, 21, 0xBBC);

					AddLabelCropped(13, 33 + (index * 22), 150, 21, 0, skill.Name);
					AddImageTiled(180, 34 + (index * 22), 50, 19, 0x52);
					AddImageTiled(181, 35 + (index * 22), 48, 17, 0xBBC);
					AddLabelCropped(182, 35 + (index * 22), 234, 21, 0, skill.Base.ToString("F1"));

					if (from.AccessLevel >= AccessLevel.Player)
					{
						AddButton(231, 35 + (index * 22), 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0);
					}
					else
					{
						AddImage(231, 35 + (index * 22), 0x2622);
					}

					++index;
				}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if (from == null || m_SoulStone.Deleted || !m_SoulStone.IsChildOf(from.Backpack))
			{
				return;
			}

			if (info.ButtonID > 0)
			{
				m_Skill = from.Skills[(info.ButtonID - 1)];

				if (m_Skill == null)
				{
					return;
				}

				int count = from.Skills.Total;
				int cap = from.SkillsCap;
				var decreased = new List<Skill>();
				int freepool = cap - count;
				int decreaseamount = 0;
				int bonuscopy = Math.Min((int)(m_SoulStone.SkillValue * 10.0), m_Skill.CapFixedPoint - m_Skill.BaseFixedPoint);

				if ((count + bonuscopy) > cap)
				{
					foreach (Skill sk in from.Skills.Where(t => t.Lock == SkillLock.Down && t.Base > 0.0))
					{
						//from.SendMessage("{0} has {1}", from.Skills[i].Name, from.Skills[i].Base );
						decreased.Add(sk);
						decreaseamount += sk.BaseFixedPoint;
					}

					if (decreaseamount <= 0)
					{
						from.SendMessage("You have exceeded the skill cap and do not have enough skill set to be decreased.");
						return;
					}
				}

				if ( /*(m_Skill.Base + bonuscopy) <= m_Skill.Cap*/ m_Skill.Base < m_Skill.Cap && m_Skill.Lock != SkillLock.Locked &&
																   m_Skill.Lock != SkillLock.Down)
				{
					if (freepool + decreaseamount >= bonuscopy)
					{
						m_Skill.BaseFixedPoint += bonuscopy;

						if (freepool < bonuscopy)
						{
							int decreasebonus = bonuscopy;
							decreasebonus -= freepool;

							foreach (Skill s in decreased)
							{
								if (decreasebonus > 0)
								{
									if (s.BaseFixedPoint >= bonuscopy)
									{
										s.BaseFixedPoint -= decreasebonus;
										decreasebonus = 0;
									}
									else
									{
										decreasebonus -= s.BaseFixedPoint;
										s.BaseFixedPoint = 0;
									}
								}
							}
						}

						m_SoulStone.SkillValue -= (bonuscopy / 10.0);

						if (m_SoulStone.SkillValue <= 0.0)
						{
							m_SoulStone.Delete();
							from.SendMessage("Your soul stone has lost all of its charge.");
						}
						/*
						if ( ( m_SoulStone.Charge -= m_SoulStone.SkillValue ) <= 0.09 )
							m_SoulStone.Delete();
						else
						{
							m_SoulStone.SkillValue = 0.0;
							m_SoulStone.NextUse = DateTime.UtcNow + SoulStone.UseDelay;
						}
						*/
						from.SendLocalizedMessage(1070713); // You have successfully absorbed the Soulstone's skill points.

						Effects.SendLocationParticles(
							EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
						Effects.PlaySound(from.Location, from.Map, 0x243);

						Effects.SendMovingParticles(
							new Entity(Server.Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map),
							from,
							0x36D4,
							7,
							0,
							false,
							true,
							0x497,
							0,
							9502,
							1,
							0,
							(EffectLayer)255,
							0x100);

						Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
					}
					else
					{
						from.SendMessage("You must have enough skill set down to compensate for the skill gain.");
					}
				}
				else
				{
					from.SendMessage("You have to choose another skill.");
				}
			}
		}
	}
}