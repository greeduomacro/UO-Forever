using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Server;
using Server.Accounting;
using Server.Network;

namespace Server.Engines.Jail
{
	/// <summary>
	/// Defines a single jailing
	/// </summary>
	public class JailEntry
	{
		private static TimeSpan ms_MaxJailTime = TimeSpan.FromDays(999);

		#region Variables

		/// <summary>
		/// The account of the player that has been jailed
		/// </summary>
		private Account m_Account;
		/// <summary>
		/// The mobile that has been jailed
		/// </summary>
		private Mobile m_Mobile;
		/// <summary>
		/// The moment of the jailing
		/// </summary>
		private DateTime m_JailTime;
		/// <summary>
		/// The duration of the jail sentence
		/// </summary>
		private TimeSpan m_Duration;
		/// <summary>
		/// A record identifying the jailer
		/// </summary>
		private string m_JailedBy;
		/// <summary>
		/// A short description of the reason of the jailing
		/// </summary>
		private string m_Reason;
		/// <summary>
		/// The staff comments on the jailing
		/// </summary>
		private List<string> m_Comments;
		/// <summary>
		/// A record to be used in the future, in case the account or mobile is no longer
		/// </summary>
		private string m_HistoryRecord;
		/// <summary>
		/// States whether the offender should be automatically released when the sentence is over
		/// </summary>
		private bool m_AutoRelease;
		/// <summary>
		/// Specifies if the full account should be jailed
		/// </summary>
		private bool m_FullJail;
		/// <summary>
		/// The IP address of the offender
		/// </summary>
		private string m_IP;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the account of the jailed player.
		/// This can be null if the account has been deleted
		/// </summary>
		public Account Account
		{
			get { return m_Account; }
		}

		/// <summary>
		/// Gets the offending mobile
		/// This can be null if the mobile has been deleted
		/// </summary>
		public Mobile Mobile
		{
			get { return m_Mobile; }
		}

		/// <summary>
		/// Gets the time of the jailing
		/// </summary>
		public DateTime JailTime
		{
			get { return m_JailTime; }
		}

		/// <summary>
		/// Gets or sets the duration of the sentence
		/// This value can be changed, but the original value is stored in the history record
		/// </summary>
		public TimeSpan Duration
		{
			get { return m_Duration; }
			set
			{
				if (value <= ms_MaxJailTime)
					m_Duration = value;
				else
					m_Duration = ms_MaxJailTime;
			}
		}

		/// <summary>
		/// Gets information about who performed this jailing
		/// </summary>
		public string JailedBy
		{
			get { return m_JailedBy; }
		}

		/// <summary>
		/// Gets the reason behind the jailing
		/// </summary>
		public string Reason
		{
			get { return m_Reason; }
		}

		/// <summary>
		/// Gets or sets the staff comments on this jailing
		/// </summary>
		public List<string> Comments
		{
			get { return m_Comments; }
			set { m_Comments = value; }
		}

		/// <summary>
		/// Gets a short description of this jailing
		/// </summary>
		public string HistoryRecord
		{
			get { return m_HistoryRecord; }
		}

		/// <summary>
		/// Gets or sets a value allowing the offender to be automatically released when the sentence is over
		/// </summary>
		public bool AutoRelease
		{
			get { return m_AutoRelease; }
			set { m_AutoRelease = value; }
		}

		/// <summary>
		/// States whether the full account should be jailed
		/// When setting this value, the JailEntry will check for currently logged players
		/// and jail/unjail accordingly
		/// </summary>
		public bool FullJail
		{
			get { return m_FullJail; }
			set
			{
				if ( m_FullJail == value )
					return;

				m_FullJail = value;

				foreach( NetState ns in NetState.Instances )
				{
					Account acc = ns.Account as Account;

					if ( m_FullJail )
					{
						if ( acc == m_Account && !JailSystem.IsInJail( ns.Mobile ) )
						{
							JailSystem.FinalizeJail( ns.Mobile );
							break;
						}
					}
					else
					{
						if ( acc == m_Account && ns.Mobile != m_Mobile && JailSystem.IsInJail( ns.Mobile ) )
						{
							JailSystem.SetFree( ns.Mobile );
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// States whether the sentence of this jail entry has epired
		/// This value is valid always false if auto release is disabled
		/// </summary>
		public bool Expired
		{
			get
			{
				if ( !m_AutoRelease )
					return false;

				try
				{
					return (m_JailTime + m_Duration) <= DateTime.UtcNow;
				}
				catch
				{
					//Catch in case m_Duration is too high.
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the IP address of the offender
		/// </summary>
		public string IP
		{
			get { return m_IP; }
		}

		/// <summary>
		/// Gets the ending time of the sentence
		/// </summary>
		public DateTime EndTime
		{
			get { return m_JailTime + m_Duration; }
		}

		#endregion

		/// <summary>
		/// Creates a new jail entry
		/// </summary>
		/// <param name="offender">The Mobile being jailed</param>
		/// <param name="account">The account of the mobile being jailed</param>
		/// <param name="from">The Mobile that is performing the jailing</param>
		/// <param name="duration">The length of the sentence</param>
		/// <param name="reason">The reason of the jailing</param>
		/// <param name="comment">Additional comments about the jailing</param>
		/// <param name="autorelease">Specifies if the account should be autoreleased</param>
		/// <param name="fulljail">Specifies if the full account should be jailed or just the specified mobile</param>
		public JailEntry( Mobile offender, Account account, Mobile from, TimeSpan duration, string reason, string comment, bool autorelease, bool fulljail )
		{
			m_Account = account; // This can't be null at creation time
			m_Mobile = offender; // This can be null in case of offline jailings or players who delete the char as soon as they are teleported to jail

			// IP
			if ( offender != null && offender.NetState != null )
				m_IP = offender.NetState.Address.ToString();
			else if ( m_Account.LoginIPs.Count > 0 )
				m_IP = m_Account.LoginIPs[ m_Account.LoginIPs.Count - 1 ].ToString();
			else
				m_IP = "Unavailable";

			m_JailTime = DateTime.UtcNow;
			Duration = duration;
			m_JailedBy = string.Format( "{0} ({1}, acc: {2})",
				from.Name,
				from.AccessLevel.ToString(),
				( from.Account as Account ).Username );
			m_Reason = reason;

			m_Comments = new List<string>();
			if ( !String.IsNullOrEmpty( comment ) )
				m_Comments.Add( string.Format( "{0}: {1}", from.Name, comment ) );

			m_HistoryRecord = string.Format( "{0}, acc: {1}, IP: {2}, jailed for {3} on {4} for {5} by {6}",
				offender != null ? offender.Name : "All Characters",
				m_Account.Username,
				m_IP,
				m_Reason,
				m_JailTime.ToString(),
				m_Duration.ToString(),
				m_JailedBy );

			m_AutoRelease = autorelease;
			m_FullJail = fulljail;
		}

		/// <summary>
		/// Creates an empty JailEntry object
		/// </summary>
		private JailEntry()
		{
			m_Comments = new List<string>();
		}

		/// <summary>
		/// Releases the offender from jail. When using FullJail it will release any character
		/// logged on the account.
		/// </summary>
		public void Release()
		{
			if ( ! m_FullJail )
			{
				// Only a mobile has been jailed
				if ( m_Mobile != null ) // Might have been deleted
					JailSystem.SetFree( m_Mobile );
			}
			else
			{
				// All mobiles might have been jailed. Release the one that's online
				Mobile m = JailSystem.GetOnlineMobile( m_Account );

				if ( m != null )
					JailSystem.SetFree( m );
			}
		}

		/// <summary>
		/// Adds a comment to the comments list
		/// </summary>
		/// <param name="from">The user adding the comment</param>
		/// <param name="format">A string format for the comment</param>
		/// <param name="args">A list of parameters for the format</param>
		public void AddComment( Mobile from, string format, params object[] args )
		{
			AddComment( from, string.Format( format, args ) );
		}

		/// <summary>
		/// Adds a comment to the comments list
		/// </summary>
		/// <param name="from">The user adding the comment</param>
		/// <param name="comment">The comment text</param>
		public void AddComment( Mobile from, string comment )
		{
			m_Comments.Add( string.Format( "[{0}] {1}: {2}", DateTime.UtcNow.ToShortDateString(), from.Name, comment ) );
		}

		#region Xml Managment

		/// <summary>
		/// Saves the jailing into an Xml document
		/// </summary>
		/// <param name="dom">The XmlDocument used to save</param>
		/// <returns>An XmlNode corresponding to the jailing</returns>
		public XmlNode GetXmlNode( XmlDocument dom )
		{
			XmlNode xNode = dom.CreateElement( "Jailing" );

			XmlAttribute account = dom.CreateAttribute( "Account" );

			if ( m_Account != null )
				account.Value = m_Account.Username;
			else
				account.Value = null;

			xNode.Attributes.Append( account );

			XmlAttribute mobile = dom.CreateAttribute( "Mobile" );

			if ( m_Mobile != null )
				mobile.Value = m_Mobile.Serial.ToString();
			else
				mobile.Value = null;

			xNode.Attributes.Append( mobile );

			XmlAttribute reason = dom.CreateAttribute( "Reason" );
			reason.Value = m_Reason;
			xNode.Attributes.Append( reason );

			XmlAttribute jailtime = dom.CreateAttribute( "JailTime" );
			jailtime.Value = m_JailTime.ToString();
			xNode.Attributes.Append( jailtime );

			XmlAttribute duration = dom.CreateAttribute( "Duration" );
			duration.Value = m_Duration.ToString();
			xNode.Attributes.Append( duration );

			XmlAttribute jailedby = dom.CreateAttribute( "JailedBy" );
			jailedby.Value = m_JailedBy;
			xNode.Attributes.Append( jailedby );

			XmlAttribute history = dom.CreateAttribute( "HistoryRecord" );
			history.Value = m_HistoryRecord;
			xNode.Attributes.Append( history );

			XmlAttribute autorelease = dom.CreateAttribute( "AutoRelease" );
			autorelease.Value = m_AutoRelease.ToString();
			xNode.Attributes.Append( autorelease );

			XmlAttribute fulljail = dom.CreateAttribute( "FullJail" );
			fulljail.Value = m_FullJail.ToString();
			xNode.Attributes.Append( fulljail );

			if ( m_Comments.Count > 0 )
			{
				XmlNode xComments = dom.CreateElement( "Comments" );

				foreach( string c in m_Comments )
				{
					XmlNode xCom = dom.CreateElement( "Comment" );

					XmlAttribute com = dom.CreateAttribute( "Value" );
					com.Value = c;
					xCom.Attributes.Append( com );

					xComments.AppendChild( xCom );
				}

				xNode.AppendChild( xComments );
			}

			return xNode;
		}

		/// <summary>
		/// Loads a jailing from an Xml Node
		/// </summary>
		/// <param name="xNode">The XmlNode containing the jailing</param>
		/// <returns>A JailEntry object</returns>
		public static JailEntry Load( XmlNode xNode )
		{
			JailEntry jail = new JailEntry();

			// Account
			string account = xNode.Attributes[ "Account" ].Value;
			if ( !String.IsNullOrEmpty( account ) )
				jail.m_Account = (Account)Accounts.GetAccount( account );

			// Mobile
			int serial = Utility.ToInt32( xNode.Attributes[ "Mobile" ].Value );

			if ( serial > -1 )
				jail.m_Mobile = World.FindMobile( (Serial) serial );

			// Jail Time
			jail.m_JailTime = Utility.GetXMLDateTime( xNode.Attributes[ "JailTime" ].Value, DateTime.MinValue );

			// Duration
			jail.Duration = Utility.GetXMLTimeSpan( xNode.Attributes[ "Duration" ].Value, TimeSpan.Zero );

			// JailedBy
			jail.m_JailedBy = xNode.Attributes[ "JailedBy" ].Value;

			// Reason
			jail.m_Reason = xNode.Attributes[ "Reason" ].Value;

			// History Record
			jail.m_HistoryRecord = xNode.Attributes[ "HistoryRecord" ].Value;

			// Autorelease
			bool autorelease;
			bool.TryParse( xNode.Attributes[ "AutoRelease" ].Value, out autorelease );
			jail.m_AutoRelease = autorelease;

			// Full jail
			bool fulljail;
			bool.TryParse( xNode.Attributes[ "FullJail" ].Value, out fulljail );
			jail.m_FullJail = fulljail;

			if ( xNode.ChildNodes.Count > 0 )
			{
				XmlNode xComments = xNode.ChildNodes[ 0 ];

				foreach( XmlNode xCom in xComments.ChildNodes )
					jail.m_Comments.Add( xCom.Attributes[ "Value" ].Value );
			}

			return jail;
		}

		#endregion
	}
}