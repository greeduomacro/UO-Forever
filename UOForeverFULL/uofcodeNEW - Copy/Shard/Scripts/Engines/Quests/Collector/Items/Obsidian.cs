using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Engines.Quests.Collector
{
	public class ObsidianStatue : Item
	{
		private static readonly string[] m_Names = new string[]
			{
				null,
				"an aggressive cavalier",
				"a beguiling rogue",
				"a benevolent physician",
				"a brilliant artisan",
				"a capricious adventurer",
				"a clever beggar",
				"a convincing charlatan",
				"a creative inventor",
				"a creative tinker",
				"a cunning knave",
				"a dauntless explorer",
				"a despicable ruffian",
				"an earnest malcontent",
				"an exultant animal tamer",
				"a famed adventurer",
				"a fanatical crusader",
				"a fastidious clerk",
				"a fearless hunter",
				"a festive harlequin",
				"a fidgety assassin",
				"a fierce soldier",
				"a fierce warrior",
				"a frugal magnate",
				"a glib pundit",
				"a gnomic shaman",
				"a graceful noblewoman",
				"a idiotic madman",
				"a imaginative designer",
				"an inept conjurer",
				"an innovative architect",
				"an inventive blacksmith",
				"a judicious mayor",
				"a masterful chef",
				"a masterful woodworker",
				"a melancholy clown",
				"a melodic bard",
				"a merciful guard",
				"a mirthful jester",
				"a nervous surgeon",
				"a peaceful scholar",
				"a prolific gardener",
				"a quixotic knight",
				"a regal aristocrat",
				"a resourceful smith",
				"a reticent alchemist",
				"a sanctified priest",
				"a scheming patrician",
				"a shrewd mage",
				"a singing minstrel",
				"a skilled tailor",
				"a squeamish assassin",
				"a stoic swordsman",
				"a studious scribe",
				"a thought provoking writer",
				"a treacherous scoundrel",
				"a troubled poet",
				"an unflappable wizard",
				"a valiant warrior",
				"a wayward fool"
			};

		public static string RandomName( Mobile from )
		{
			int index = Utility.Random( m_Names.Length );

			if ( m_Names[index] == null )
				return from.Name;
			else
				return m_Names[index];
		}

		private static readonly int Partial = 2;
		private static readonly int Completed = 10;

		private int m_Quantity;
		private int m_StatueName;
		private Mobile m_StatueOwner;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Quantity
		{
			get { return m_Quantity; }
			set
			{
				if ( value <= 1 )
					m_Quantity = 1;
				else if ( value >= Completed )
					m_Quantity = Completed;
				else
					m_Quantity = value;

				if ( m_Quantity < Partial )
					ItemID = 0x1EA7;
				else if ( m_Quantity < Completed )
					ItemID = 0x1F13;
				else
					ItemID = 0x12CB;

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StatueName
		{
			get { return m_StatueName; }
			set { m_StatueName = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile StatueOwner
		{
			get { return m_StatueOwner; }
			set { m_StatueOwner = value; InvalidateProperties(); }
		}

		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled && EraAOS; } }

		[Constructable]
		public ObsidianStatue() : base( 0x1EA7 )
		{
			Hue = 0x497;
			m_Quantity = 1;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_Quantity < Partial )
				list.Add( 1055137 ); // a section of an Obsidian statue
			else if ( m_Quantity < Completed )
				list.Add( 1055138 ); // a partially reconstructed Obsidian statue
			else if ( m_StatueName == 0 && m_StatueOwner != null )
				list.Add( 1055139, m_StatueOwner.Name );
			else
				list.Add( 1055139, m_Names[Math.Max( m_StatueName, 1)] ); // an Obsidian statue of ~1_STATUE_NAME~
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Quantity < Partial )
				LabelTo( from, 1055137 ); // a section of an Obsidian statue
			else if ( m_Quantity < Completed )
				LabelTo( from, 1055138 ); // a partially reconstructed Obsidian statue
			else if ( m_StatueName == 0 && m_StatueOwner != null )
				LabelTo( from, 1055139, m_StatueOwner.Name );
			else
				LabelTo( from, 1055139, m_Names[Math.Max( m_StatueName, 1)] ); // an Obsidian statue of ~1_STATUE_NAME~
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive && m_Quantity >= Partial && m_Quantity < Completed && IsChildOf( from.Backpack ) )
				list.Add( new DisassembleEntry( this ) );
		}

		private class DisassembleEntry : ContextMenuEntry
		{
			private ObsidianStatue m_Obsidian;

			public DisassembleEntry( ObsidianStatue Obsidian ) : base( 6142 )
			{
				m_Obsidian = Obsidian;
			}

			public override void OnClick()
			{
				Mobile from = Owner.From;
				if ( !m_Obsidian.Deleted && m_Obsidian.Quantity >= ObsidianStatue.Partial && m_Obsidian.Quantity < ObsidianStatue.Completed && m_Obsidian.IsChildOf( from.Backpack ) && from.CheckAlive() )
				{
					for ( int i = 0; i < m_Obsidian.Quantity - 1; i++ )
						from.AddToBackpack( new ObsidianStatue() );

					m_Obsidian.Quantity = 1;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Quantity < Completed )
			{
				if ( !IsChildOf( from.Backpack ) )
					from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x2C, 3, 500309, "", "" ) ); // Nothing Happens.
				else
					from.Target = new InternalTarget( this );
			}
		}

		private class InternalTarget : Target
		{
			private ObsidianStatue m_Obsidian;

			public InternalTarget( ObsidianStatue Obsidian ) : base( -1, false, TargetFlags.None )
			{
				m_Obsidian = Obsidian;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				ObsidianStatue targ = targeted as ObsidianStatue;

				if ( m_Obsidian.Deleted || m_Obsidian.Quantity >= ObsidianStatue.Completed || targ == null )
					return;

				if ( m_Obsidian.IsChildOf( from.Backpack ) && targ.IsChildOf( from.Backpack ) && targ != m_Obsidian )
				{
					ObsidianStatue targObsidian = (ObsidianStatue)targ;
					if ( targObsidian.Quantity < ObsidianStatue.Completed )
					{
						if ( targObsidian.Quantity + m_Obsidian.Quantity <= ObsidianStatue.Completed )
						{
							targObsidian.Quantity += m_Obsidian.Quantity;
							m_Obsidian.Delete();
						}
						else
						{
							int delta = ObsidianStatue.Completed - targObsidian.Quantity;
							targObsidian.Quantity += delta;
							m_Obsidian.Quantity -= delta;
						}

						if ( targObsidian.Quantity >= ObsidianStatue.Completed )
							targObsidian.StatueName = Utility.Random( m_Names.Length );

						from.Send( new AsciiMessage( targObsidian.Serial, targObsidian.ItemID, MessageType.Regular, 0x59, 3, m_Obsidian.Name, "Something Happened." ) );

						return;
					}
				}

				from.Send( new MessageLocalized( m_Obsidian.Serial, m_Obsidian.ItemID, MessageType.Regular, 0x2C, 3, 500309, m_Obsidian.Name, "" ) ); // Nothing Happens.
			}
		}

		public ObsidianStatue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( m_Quantity );
			writer.Write( m_StatueName );

			if ( m_StatueName == 0 && m_Quantity >= Completed )
				writer.Write( m_StatueOwner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Quantity = reader.ReadEncodedInt();
			m_StatueName = reader.ReadInt();

			if ( m_StatueName == 0 && m_Quantity >= Completed )
				m_StatueOwner = reader.ReadMobile();
		}
	}
}