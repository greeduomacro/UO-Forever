#region References
using System;
using System.Linq;
using Server.Network;
using VitaNex.Text;
#endregion

namespace Server.Engines.Conquests
{
    public class SpeechConquestContainer
    {
		private readonly Mobile m_Mobile;
		private readonly MessageType m_Type;
		private readonly int m_Hue;
		private readonly int[] m_Keywords;

		public Mobile Mobile { get { return m_Mobile; } }
		public string Speech { get; set; }
		public MessageType Type { get { return m_Type; } }
		public int Hue { get { return m_Hue; } }
		public int[] Keywords { get { return m_Keywords; } }
		public bool Handled { get; set; }
		public bool Blocked { get; set; }

		public bool HasKeyword(int keyword)
		{
			return m_Keywords.Any(t => t == keyword);
		}

        public SpeechConquestContainer(Mobile mobile, string speech, MessageType type, int hue, int[] keywords)
		{
			m_Mobile = mobile;
			Speech = speech;
			m_Type = type;
			m_Hue = hue;
			m_Keywords = keywords;
		}
    }

	public class SpeechConquest : Conquest
	{
		public override string DefCategory { get { return "Social"; } }

		public virtual string DefPhrase { get { return String.Empty; } }
		public virtual StringSearchFlags DefSearch { get { return StringSearchFlags.Contains; } }

		public virtual bool DefIgnoreCase { get { return true; } }
		public virtual bool DefSpeechChangeReset { get { return true; } }

		[CommandProperty(Conquests.Access)]
		public string Phrase { get; set; }

		[CommandProperty(Conquests.Access)]
		public StringSearchFlags Search { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool IgnoreCase { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool SpeechChangeReset { get; set; }

		public SpeechConquest()
		{ }

		public SpeechConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			Phrase = DefPhrase;
			Search = DefSearch;

			IgnoreCase = DefIgnoreCase;
			SpeechChangeReset = DefSpeechChangeReset;
		}

		public override sealed int GetProgress(ConquestState state, object args)
		{
            return GetProgress(state, args as SpeechConquestContainer);
		}

        protected virtual int GetProgress(ConquestState state, SpeechConquestContainer args)
		{
            if (state.User == null)
                return 0;

			if (args == null || String.IsNullOrEmpty(args.Speech))
			{
				return 0;
			}

			if (!Search.Execute(args.Speech, Phrase, IgnoreCase))
			{
				if (SpeechChangeReset)
				{
					return -state.Progress;
				}

				return 0;
			}

			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(Phrase);
						writer.WriteFlag(Search);

						writer.Write(IgnoreCase);
						writer.Write(SpeechChangeReset);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						Phrase = reader.ReadString();
						Search = reader.ReadFlag<StringSearchFlags>();

						IgnoreCase = reader.ReadBool();
						SpeechChangeReset = reader.ReadBool();
					}
					break;
			}
		}
	}
}