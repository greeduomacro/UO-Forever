using System;
using Server.Mobiles;

namespace Server
{
	public class SkillGainMod
	{
		private PlayerMobile m_Owner;
		private SkillName m_Skill;
		private double m_Bonus;
		private TimeSpan m_Duration;
		private DateTime m_Added;
		private string m_Name;

		public PlayerMobile Owner{ get{ return m_Owner; } }
		public SkillName Skill{ get{ return m_Skill; } }
		public double Bonus{ get{ return m_Bonus; } }
		public TimeSpan Duration{ get{ return m_Duration; } } //If it doesn't expire, then it doesn't need to serialize.  It should get added through a different condition.
		public DateTime Added{ get{ return m_Added; } }
		public string Name{ get { return m_Name; } }

		public bool HasElapsed()
		{
			return m_Duration != TimeSpan.Zero && (DateTime.UtcNow - m_Added) >= m_Duration;
		}

		public SkillGainMod( PlayerMobile owner, string name, SkillName skill, double bonus, TimeSpan duration )
		{
			m_Owner = owner;
			m_Name = name;
			m_Skill = skill;
			m_Bonus = bonus;
			m_Duration = duration;
			m_Added = DateTime.UtcNow;
		}

		public SkillGainMod( PlayerMobile owner, GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_Owner = owner;
			m_Name = reader.ReadString();
			m_Skill = (SkillName)reader.ReadInt();
			m_Bonus = reader.ReadDouble();
			m_Duration = reader.ReadTimeSpan();
			m_Added = reader.ReadDateTime();
		}

		public virtual void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_Name );
			writer.Write( (int)m_Skill );
			writer.Write( m_Bonus );
			writer.Write( m_Duration );
			writer.Write( m_Added );
		}
	}
}