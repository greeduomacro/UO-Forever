/***************************************************************************
 *                                EventSink.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: EventSink.cs 644 2010-12-23 09:18:45Z asayre $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Server.Accounting;
using Server.Commands;
using Server.Guilds;
using Server.Items;
using Server.Network;
#endregion

namespace Server
{
	public delegate void CharacterCreatedEventHandler(CharacterCreatedEventArgs e);

	public delegate bool ValidatePlayerNameEventHandler(ValidatePlayerNameEventArgs e);

	public delegate void OpenDoorMacroEventHandler(OpenDoorMacroEventArgs e);

	public delegate void SpeechEventHandler(SpeechEventArgs e);

	public delegate void LoginEventHandler(LoginEventArgs e);

	public delegate void ServerListEventHandler(ServerListEventArgs e);

	public delegate void MovementEventHandler(MovementEventArgs e);

	public delegate void HungerChangedEventHandler(HungerChangedEventArgs e);

	public delegate void CrashedEventHandler(CrashedEventArgs e);

	public delegate void ShutdownEventHandler(ShutdownEventArgs e);

	public delegate void HelpRequestEventHandler(HelpRequestEventArgs e);

	public delegate void DisarmRequestEventHandler(DisarmRequestEventArgs e);

	public delegate void StunRequestEventHandler(StunRequestEventArgs e);

	public delegate void OpenSpellbookRequestEventHandler(OpenSpellbookRequestEventArgs e);

	public delegate void CastSpellRequestEventHandler(CastSpellRequestEventArgs e);

	public delegate void AnimateRequestEventHandler(AnimateRequestEventArgs e);

	public delegate void LogoutEventHandler(LogoutEventArgs e);

	public delegate void SocketConnectEventHandler(SocketConnectEventArgs e);

	public delegate void ConnectedEventHandler(ConnectedEventArgs e);

	public delegate void DisconnectedEventHandler(DisconnectedEventArgs e);

	public delegate void RenameRequestEventHandler(RenameRequestEventArgs e);

	public delegate void PlayerDeathEventHandler(PlayerDeathEventArgs e);

	public delegate void CreatureDeathEventHandler(CreatureDeathEventArgs e);

	public delegate void VirtueGumpRequestEventHandler(VirtueGumpRequestEventArgs e);

	public delegate void VirtueItemRequestEventHandler(VirtueItemRequestEventArgs e);

	public delegate void VirtueMacroRequestEventHandler(VirtueMacroRequestEventArgs e);

	//public delegate void InscribeMenuRequestEventHandler( InscribeMenuRequestEventArgs e );

	public delegate void ChatRequestEventHandler(ChatRequestEventArgs e);

	public delegate void AccountLoginEventHandler(AccountLoginEventArgs e);

	public delegate void PaperdollRequestEventHandler(PaperdollRequestEventArgs e);

	public delegate void ProfileRequestEventHandler(ProfileRequestEventArgs e);

	public delegate void ChangeProfileRequestEventHandler(ChangeProfileRequestEventArgs e);

	public delegate void AggressiveActionEventHandler(AggressiveActionEventArgs e);

	public delegate void GameLoginEventHandler(GameLoginEventArgs e);

	public delegate void DeleteRequestEventHandler(DeleteRequestEventArgs e);

	public delegate void WorldLoadEventHandler();

	public delegate void WorldSaveEventHandler(WorldSaveEventArgs e);

	public delegate void SetAbilityEventHandler(SetAbilityEventArgs e);

	public delegate void FastWalkEventHandler(FastWalkEventArgs e);

	public delegate void ServerStartedEventHandler();

	public delegate void CreateGuildHandler(CreateGuildEventArgs e);

	public delegate void GuildGumpRequestHandler(GuildGumpRequestArgs e);

	public delegate void QuestGumpRequestHandler(QuestGumpRequestArgs e);

	public delegate void ClientVersionReceivedHandler(ClientVersionReceivedArgs e);

	public delegate void LoginRecordFromDBHandler(LoginRecordFromDBArgs e);

	public delegate void LoginRecordToDBHandler(LoginRecordToDBArgs e);

	public delegate void OnItemUseEventHandler(OnItemUseEventArgs e);

	public delegate void OnEnterRegionEventHandler(OnEnterRegionEventArgs e);

	public delegate void OnConsumeEventHandler(OnConsumeEventArgs e);

	public delegate void OnPropertyChangedEventHandler(OnPropertyChangedEventArgs e);

    public delegate void MobileInvalidateEventHandler(MobileInvalidateEventArgs e);

    public class MobileInvalidateEventArgs : EventArgs
    {
        public Mobile Mobile { get; set; }

        public MobileInvalidateEventArgs(Mobile m)
        {
            Mobile = m;
        }
    }

	public class LoginRecordToDBArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly int m_ShardID;
		public NetState State { get { return m_State; } }
		public int ShardID { get { return m_ShardID; } }
		//Seed/AuthID get set in the State

		public LoginRecordToDBArgs(NetState state, int shardid)
		{
			m_ShardID = shardid;
			m_State = state;
		}
	}

	public class LoginRecordFromDBArgs : EventArgs
	{
		private readonly NetState m_State;
		public NetState State { get { return m_State; } }
		//Seed/AuthID get set in the State

		public LoginRecordFromDBArgs(NetState state)
		{
			m_State = state;
		}
	}

	public class ClientVersionReceivedArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly ClientVersion m_Version;

		public NetState State { get { return m_State; } }
		public ClientVersion Version { get { return m_Version; } }

		public ClientVersionReceivedArgs(NetState state, ClientVersion cv)
		{
			m_State = state;
			m_Version = cv;
		}
	}

	public class CreateGuildEventArgs : EventArgs
	{
		public int Id { get; set; }
		public BaseGuild Guild { get; set; }

		public CreateGuildEventArgs(int id)
		{
			Id = id;
		}
	}

	public class GuildGumpRequestArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public GuildGumpRequestArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class QuestGumpRequestArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public QuestGumpRequestArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class SetAbilityEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_Index;

		public Mobile Mobile { get { return m_Mobile; } }
		public int Index { get { return m_Index; } }

		public SetAbilityEventArgs(Mobile mobile, int index)
		{
			m_Mobile = mobile;
			m_Index = index;
		}
	}

	public class DeleteRequestEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly int m_Index;

		public NetState State { get { return m_State; } }
		public int Index { get { return m_Index; } }

		public DeleteRequestEventArgs(NetState state, int index)
		{
			m_State = state;
			m_Index = index;
		}
	}

	public class GameLoginEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly string m_Username;
		private readonly string m_Password;

		public NetState State { get { return m_State; } }
		public string Username { get { return m_Username; } }
		public string Password { get { return m_Password; } }
		public bool Accepted { get; set; }
		public CityInfo[] CityInfo { get; set; }

		public GameLoginEventArgs(NetState state, string un, string pw)
		{
			m_State = state;
			m_Username = un;
			m_Password = pw;
		}
	}

	public class AggressiveActionEventArgs : EventArgs
	{
		private Mobile m_Aggressed;
		private Mobile m_Aggressor;
		private bool m_Criminal;

		public Mobile Aggressed { get { return m_Aggressed; } }
		public Mobile Aggressor { get { return m_Aggressor; } }
		public bool Criminal { get { return m_Criminal; } }

		private static readonly Queue<AggressiveActionEventArgs> m_Pool = new Queue<AggressiveActionEventArgs>();

		public static AggressiveActionEventArgs Create(Mobile aggressed, Mobile aggressor, bool criminal)
		{
			AggressiveActionEventArgs args;

			if (m_Pool.Count > 0)
			{
				args = m_Pool.Dequeue();

				args.m_Aggressed = aggressed;
				args.m_Aggressor = aggressor;
				args.m_Criminal = criminal;
			}
			else
			{
				args = new AggressiveActionEventArgs(aggressed, aggressor, criminal);
			}

			return args;
		}

		private AggressiveActionEventArgs(Mobile aggressed, Mobile aggressor, bool criminal)
		{
			m_Aggressed = aggressed;
			m_Aggressor = aggressor;
			m_Criminal = criminal;
		}

		public void Free()
		{
			m_Pool.Enqueue(this);
		}
	}

	public class ProfileRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public ProfileRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class ChangeProfileRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;
		private readonly string m_Text;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }
		public string Text { get { return m_Text; } }

		public ChangeProfileRequestEventArgs(Mobile beholder, Mobile beheld, string text)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
			m_Text = text;
		}
	}

	public class PaperdollRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public PaperdollRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class AccountLoginEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly string m_Username;
		private readonly string m_Password;

		public NetState State { get { return m_State; } }
		public string Username { get { return m_Username; } }
		public string Password { get { return m_Password; } }
		public bool Accepted { get; set; }
		public ALRReason RejectReason { get; set; }

		public AccountLoginEventArgs(NetState state, string username, string password)
		{
			m_State = state;
			m_Username = username;
			m_Password = password;
		}
	}

	public class VirtueItemRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;
		private readonly int m_GumpID;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }
		public int GumpID { get { return m_GumpID; } }

		public VirtueItemRequestEventArgs(Mobile beholder, Mobile beheld, int gumpID)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
			m_GumpID = gumpID;
		}
	}

	public class VirtueGumpRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Beholder;
		private readonly Mobile m_Beheld;

		public Mobile Beholder { get { return m_Beholder; } }
		public Mobile Beheld { get { return m_Beheld; } }

		public VirtueGumpRequestEventArgs(Mobile beholder, Mobile beheld)
		{
			m_Beholder = beholder;
			m_Beheld = beheld;
		}
	}

	public class VirtueMacroRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_VirtueID;

		public Mobile Mobile { get { return m_Mobile; } }
		public int VirtueID { get { return m_VirtueID; } }

		public VirtueMacroRequestEventArgs(Mobile mobile, int virtueID)
		{
			m_Mobile = mobile;
			m_VirtueID = virtueID;
		}
	}
	/*
	public class InscribeMenuRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly Item m_Item;
		private readonly Point3D m_Location;

		public Mobile Mobile { get { return m_Mobile; } }
		public Item Item { get { return m_Item; } }
		public Point3D Location { get { return m_Location; } }

		public InscribeMenuRequestEventArgs(Mobile mobile, Item item, Point3D loc)
		{
			m_Mobile = mobile;
			m_Item = item;
			m_Location = loc;
		}
	}
	*/
	public class ChatRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public ChatRequestEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class PlayerDeathEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public Mobile Killer { get; private set; }
		public Container Corpse { get; private set; }

		public PlayerDeathEventArgs(Mobile mobile, Mobile killer, Container corpse)
		{
			Mobile = mobile;
			Killer = killer;
			Corpse = corpse;
		}
	}

	public class CreatureDeathEventArgs : EventArgs
	{
		public Mobile Creature { get; private set; }
		public Mobile Killer { get; private set; }
		public Container Corpse { get; private set; }

		public CreatureDeathEventArgs(Mobile creature, Mobile killer, Container corpse)
		{
			Creature = creature;
			Killer = killer;
			Corpse = corpse;
		}
	}

	public class RenameRequestEventArgs : EventArgs
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Target;
		private readonly string m_Name;

		public Mobile From { get { return m_From; } }
		public Mobile Target { get { return m_Target; } }
		public string Name { get { return m_Name; } }

		public RenameRequestEventArgs(Mobile from, Mobile target, string name)
		{
			m_From = from;
			m_Target = target;
			m_Name = name;
		}
	}

	public class LogoutEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public LogoutEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class SocketConnectEventArgs : EventArgs
	{
		private readonly Socket m_Socket;

		public Socket Socket { get { return m_Socket; } }
		public bool AllowConnection { get; set; }

		public SocketConnectEventArgs(Socket s)
		{
			m_Socket = s;
			AllowConnection = true;
		}
	}

	public class ConnectedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public ConnectedEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class DisconnectedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public DisconnectedEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class AnimateRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly string m_Action;

		public Mobile Mobile { get { return m_Mobile; } }
		public string Action { get { return m_Action; } }

		public AnimateRequestEventArgs(Mobile m, string action)
		{
			m_Mobile = m;
			m_Action = action;
		}
	}

	public class CastSpellRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly Item m_Spellbook;
		private readonly int m_SpellID;

		public Mobile Mobile { get { return m_Mobile; } }
		public Item Spellbook { get { return m_Spellbook; } }
		public int SpellID { get { return m_SpellID; } }

		public CastSpellRequestEventArgs(Mobile m, int spellID, Item book)
		{
			m_Mobile = m;
			m_Spellbook = book;
			m_SpellID = spellID;
		}
	}

	public class OpenSpellbookRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_Type;

		public Mobile Mobile { get { return m_Mobile; } }
		public int Type { get { return m_Type; } }

		public OpenSpellbookRequestEventArgs(Mobile m, int type)
		{
			m_Mobile = m;
			m_Type = type;
		}
	}

	public class StunRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public StunRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class DisarmRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public DisarmRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class HelpRequestEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public HelpRequestEventArgs(Mobile m)
		{
			m_Mobile = m;
		}
	}

	public class ShutdownEventArgs : EventArgs
	{ }

	public class CrashedEventArgs : EventArgs
	{
		private readonly Exception m_Exception;

		public Exception Exception { get { return m_Exception; } }
		public bool Close { get; set; }

		public CrashedEventArgs(Exception e)
		{
			m_Exception = e;
		}
	}

	public class HungerChangedEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;
		private readonly int m_OldValue;

		public Mobile Mobile { get { return m_Mobile; } }
		public int OldValue { get { return m_OldValue; } }

		public HungerChangedEventArgs(Mobile mobile, int oldValue)
		{
			m_Mobile = mobile;
			m_OldValue = oldValue;
		}
	}

	public class MovementEventArgs : EventArgs
	{
		private Mobile m_Mobile;
		private Direction m_Direction;
		private bool m_Blocked;

		public Mobile Mobile { get { return m_Mobile; } }
		public Direction Direction { get { return m_Direction; } }
		public bool Blocked { get { return m_Blocked; } set { m_Blocked = value; } }

		private static readonly Queue<MovementEventArgs> m_Pool = new Queue<MovementEventArgs>();

		public static MovementEventArgs Create(Mobile mobile, Direction dir)
		{
			MovementEventArgs args;

			if (m_Pool.Count > 0)
			{
				args = m_Pool.Dequeue();

				args.m_Mobile = mobile;
				args.m_Direction = dir;
				args.m_Blocked = false;
			}
			else
			{
				args = new MovementEventArgs(mobile, dir);
			}

			return args;
		}

		public MovementEventArgs(Mobile mobile, Direction dir)
		{
			m_Mobile = mobile;
			m_Direction = dir;
		}

		public void Free()
		{
			m_Pool.Enqueue(this);
		}
	}

	public class ServerListEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly IAccount m_Account;
		private readonly List<ServerInfo> m_Servers;

		public NetState State { get { return m_State; } }
		public IAccount Account { get { return m_Account; } }
		public bool Rejected { get; set; }
		public List<ServerInfo> Servers { get { return m_Servers; } }

		public void AddServer(string name, IPEndPoint address)
		{
			AddServer(name, 0, TimeZone.CurrentTimeZone, address);
		}

		public void AddServer(string name, int fullPercent, TimeZone tz, IPEndPoint address)
		{
			m_Servers.Add(new ServerInfo(name, fullPercent, tz, address));
		}

		public ServerListEventArgs(NetState state, IAccount account)
		{
			m_State = state;
			m_Account = account;
			m_Servers = new List<ServerInfo>();
		}
	}

	public struct SkillNameValue
	{
		private readonly SkillName m_Name;
		private readonly int m_Value;

		public SkillName Name { get { return m_Name; } }
		public int Value { get { return m_Value; } }

		public SkillNameValue(SkillName name, int value)
		{
			m_Name = name;
			m_Value = value;
		}
	}

	public class ValidatePlayerNameEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly IAccount m_Account;
		private readonly string m_Name;

		public NetState State { get { return m_State; } }
		public IAccount Account { get { return m_Account; } }
		public string Name { get { return m_Name; } }

		public ValidatePlayerNameEventArgs(NetState state, IAccount a, string name)
		{
			m_State = state;
			m_Account = a;
			m_Name = name;
		}
	}

	public class CharacterCreatedEventArgs : EventArgs
	{
		private readonly NetState m_State;
		private readonly IAccount m_Account;
		private readonly CityInfo m_City;
		private readonly SkillNameValue[] m_Skills;
		private readonly int m_ShirtHue;
		private readonly int m_PantsHue;
		private readonly int m_HairID;
		private readonly int m_HairHue;
		private readonly int m_BeardID;
		private readonly int m_BeardHue;
		private readonly string m_Name;
		private readonly bool m_Female;
		private readonly int m_Hue;
		private readonly int m_Str;
		private readonly int m_Dex;
		private readonly int m_Int;

		private readonly Race m_Race;

		public NetState State { get { return m_State; } }
		public IAccount Account { get { return m_Account; } }
		public Mobile Mobile { get; set; }
		public string Name { get { return m_Name; } }
		public bool Female { get { return m_Female; } }
		public int Hue { get { return m_Hue; } }
		public int Str { get { return m_Str; } }
		public int Dex { get { return m_Dex; } }
		public int Int { get { return m_Int; } }
		public CityInfo City { get { return m_City; } }
		public SkillNameValue[] Skills { get { return m_Skills; } }
		public int ShirtHue { get { return m_ShirtHue; } }
		public int PantsHue { get { return m_PantsHue; } }
		public int HairID { get { return m_HairID; } }
		public int HairHue { get { return m_HairHue; } }
		public int BeardID { get { return m_BeardID; } }
		public int BeardHue { get { return m_BeardHue; } }
		public int Profession { get; set; }
		public Race Race { get { return m_Race; } }

		public CharacterCreatedEventArgs(
			NetState state,
			IAccount a,
			string name,
			bool female,
			int hue,
			int str,
			int dex,
			int intel,
			CityInfo city,
			SkillNameValue[] skills,
			int shirtHue,
			int pantsHue,
			int hairID,
			int hairHue,
			int beardID,
			int beardHue,
			int profession,
			Race race)
		{
			m_State = state;
			m_Account = a;
			m_Name = name;
			m_Female = female;
			m_Hue = hue;
			m_Str = str;
			m_Dex = dex;
			m_Int = intel;
			m_City = city;
			m_Skills = skills;
			m_ShirtHue = shirtHue;
			m_PantsHue = pantsHue;
			m_HairID = hairID;
			m_HairHue = hairHue;
			m_BeardID = beardID;
			m_BeardHue = beardHue;
			Profession = profession;
			m_Race = race;
		}
	}

	public class OpenDoorMacroEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public OpenDoorMacroEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class SpeechEventArgs : EventArgs
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

		public SpeechEventArgs(Mobile mobile, string speech, MessageType type, int hue, int[] keywords)
		{
			m_Mobile = mobile;
			Speech = speech;
			m_Type = type;
			m_Hue = hue;
			m_Keywords = keywords;
		}
	}

	public class LoginEventArgs : EventArgs
	{
		private readonly Mobile m_Mobile;

		public Mobile Mobile { get { return m_Mobile; } }

		public LoginEventArgs(Mobile mobile)
		{
			m_Mobile = mobile;
		}
	}

	public class WorldSaveEventArgs : EventArgs
	{
		private readonly bool m_Msg;

		public bool Message { get { return m_Msg; } }

		public WorldSaveEventArgs(bool msg)
		{
			m_Msg = msg;
		}
	}

	public class OnItemUseEventArgs : EventArgs
	{
		public Mobile User { get; private set; }
		public Item Item { get; private set; }

		public OnItemUseEventArgs(Mobile user, Item item)
		{
			User = user;
			Item = item;
		}
	}

	public class OnEnterRegionEventArgs : EventArgs
	{
		public Mobile From { get; private set; }
		public Region Region { get; private set; }

		public OnEnterRegionEventArgs(Mobile from, Region region)
		{
			From = from;
			Region = region;
		}
	}

	public class OnConsumeEventArgs : EventArgs
	{
		public Mobile Consumer { get; private set; }
		public Item Consumed { get; private set; }
		public int Quantity { get; private set; }

		public OnConsumeEventArgs(Mobile consumer, Item consumed)
			: this(consumer, consumed, 1)
		{ }

		public OnConsumeEventArgs(Mobile consumer, Item consumed, int quantity)
		{
			Consumer = consumer;
			Consumed = consumed;
			Quantity = quantity;
		}
	}

	public class OnPropertyChangedEventArgs : EventArgs
	{
		public Mobile Mobile { get; private set; }
		public PropertyInfo Property { get; private set; }
		public object Instance { get; private set; }
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }

		public OnPropertyChangedEventArgs(Mobile m, object instance, PropertyInfo prop, object oldValue, object newValue)
		{
			Mobile = m;
			Property = prop;
			Instance = instance;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	public class FastWalkEventArgs
	{
		private readonly NetState m_State;

		public FastWalkEventArgs(NetState state)
		{
			m_State = state;
			Blocked = false;
		}

		public NetState NetState { get { return m_State; } }
		public bool Blocked { get; set; }
	}

	public static class EventSink
	{
        public static event MobileInvalidateEventHandler MobileInvalidate;
		public static event CharacterCreatedEventHandler CharacterCreated;
		public static event ValidatePlayerNameEventHandler ValidatePlayerName;
		public static event OpenDoorMacroEventHandler OpenDoorMacroUsed;
		public static event SpeechEventHandler Speech;
		public static event LoginEventHandler Login;
		public static event ServerListEventHandler ServerList;
		public static event MovementEventHandler Movement;
		public static event HungerChangedEventHandler HungerChanged;
		public static event CrashedEventHandler Crashed;
		public static event ShutdownEventHandler Shutdown;
		public static event HelpRequestEventHandler HelpRequest;
		public static event DisarmRequestEventHandler DisarmRequest;
		public static event StunRequestEventHandler StunRequest;
		public static event OpenSpellbookRequestEventHandler OpenSpellbookRequest;
		public static event CastSpellRequestEventHandler CastSpellRequest;
		public static event AnimateRequestEventHandler AnimateRequest;
		public static event LogoutEventHandler Logout;
		public static event SocketConnectEventHandler SocketConnect;
		public static event ConnectedEventHandler Connected;
		public static event DisconnectedEventHandler Disconnected;
		public static event RenameRequestEventHandler RenameRequest;
		public static event PlayerDeathEventHandler PlayerDeath;
		public static event CreatureDeathEventHandler CreatureDeath;
		public static event VirtueGumpRequestEventHandler VirtueGumpRequest;
		public static event VirtueItemRequestEventHandler VirtueItemRequest;
		public static event VirtueMacroRequestEventHandler VirtueMacroRequest;
		public static event ChatRequestEventHandler ChatRequest;
		public static event AccountLoginEventHandler AccountLogin;
		public static event PaperdollRequestEventHandler PaperdollRequest;
		public static event ProfileRequestEventHandler ProfileRequest;
		public static event ChangeProfileRequestEventHandler ChangeProfileRequest;
		public static event AggressiveActionEventHandler AggressiveAction;
		public static event CommandEventHandler Command;
		public static event GameLoginEventHandler GameLogin;
		public static event DeleteRequestEventHandler DeleteRequest;
		public static event WorldLoadEventHandler WorldLoad;
		public static event WorldSaveEventHandler WorldSave;
		public static event SetAbilityEventHandler SetAbility;
		public static event FastWalkEventHandler FastWalk;
		public static event CreateGuildHandler CreateGuild;
		public static event ServerStartedEventHandler ServerStarted;
		public static event GuildGumpRequestHandler GuildGumpRequest;
		public static event QuestGumpRequestHandler QuestGumpRequest;
		public static event ClientVersionReceivedHandler ClientVersionReceived;
		public static event LoginRecordFromDBHandler LoginRecordFromDB;
		public static event LoginRecordToDBHandler LoginRecordToDB;
		public static event OnItemUseEventHandler OnItemUse;
		public static event OnEnterRegionEventHandler OnEnterRegion;
		public static event OnConsumeEventHandler OnConsume;
		public static event OnPropertyChangedEventHandler OnPropertyChanged;

        public static void InvokeMobileInvalidate(MobileInvalidateEventArgs e)
        {
            if (MobileInvalidate != null)
            {
                MobileInvalidate(e);
            }
        }

		public static void InvokeLoginRecordToDB(LoginRecordToDBArgs e)
		{
			if (LoginRecordToDB != null)
			{
				LoginRecordToDB(e);
			}
		}

		public static void InvokeLoginRecordFromDB(LoginRecordFromDBArgs e)
		{
			if (LoginRecordFromDB != null)
			{
				LoginRecordFromDB(e);
			}
		}

		public static void InvokeClientVersionReceived(ClientVersionReceivedArgs e)
		{
			if (ClientVersionReceived != null)
			{
				ClientVersionReceived(e);
			}
		}

		public static void InvokeServerStarted()
		{
			if (ServerStarted != null)
			{
				ServerStarted();
			}
		}

		public static BaseGuild InvokeCreateGuild(CreateGuildEventArgs e)
		{
			if (CreateGuild != null)
			{
				CreateGuild(e);
			}

			return e.Guild;
		}

		public static void InvokeSetAbility(SetAbilityEventArgs e)
		{
			if (SetAbility != null)
			{
				SetAbility(e);
			}
		}

		public static void InvokeGuildGumpRequest(GuildGumpRequestArgs e)
		{
			if (GuildGumpRequest != null)
			{
				GuildGumpRequest(e);
			}
		}

		public static void InvokeQuestGumpRequest(QuestGumpRequestArgs e)
		{
			if (QuestGumpRequest != null)
			{
				QuestGumpRequest(e);
			}
		}

		public static void InvokeFastWalk(FastWalkEventArgs e)
		{
			if (FastWalk != null)
			{
				FastWalk(e);
			}
		}

		public static void InvokeDeleteRequest(DeleteRequestEventArgs e)
		{
			if (DeleteRequest != null)
			{
				DeleteRequest(e);
			}
		}

		public static void InvokeGameLogin(GameLoginEventArgs e)
		{
			if (GameLogin != null)
			{
				GameLogin(e);
			}
		}

		public static void InvokeCommand(CommandEventArgs e)
		{
			if (Command != null)
			{
				Command(e);
			}
		}

		public static void InvokeAggressiveAction(AggressiveActionEventArgs e)
		{
			if (AggressiveAction != null)
			{
				AggressiveAction(e);
			}
		}

		public static void InvokeProfileRequest(ProfileRequestEventArgs e)
		{
			if (ProfileRequest != null)
			{
				ProfileRequest(e);
			}
		}

		public static void InvokeChangeProfileRequest(ChangeProfileRequestEventArgs e)
		{
			if (ChangeProfileRequest != null)
			{
				ChangeProfileRequest(e);
			}
		}

		public static void InvokePaperdollRequest(PaperdollRequestEventArgs e)
		{
			if (PaperdollRequest != null)
			{
				PaperdollRequest(e);
			}
		}

		public static void InvokeAccountLogin(AccountLoginEventArgs e)
		{
			if (AccountLogin != null)
			{
				AccountLogin(e);
			}
		}

		public static void InvokeChatRequest(ChatRequestEventArgs e)
		{
			if (ChatRequest != null)
			{
				ChatRequest(e);
			}
		}

		public static void InvokeVirtueItemRequest(VirtueItemRequestEventArgs e)
		{
			if (VirtueItemRequest != null)
			{
				VirtueItemRequest(e);
			}
		}

		public static void InvokeVirtueGumpRequest(VirtueGumpRequestEventArgs e)
		{
			if (VirtueGumpRequest != null)
			{
				VirtueGumpRequest(e);
			}
		}

		public static void InvokeVirtueMacroRequest(VirtueMacroRequestEventArgs e)
		{
			if (VirtueMacroRequest != null)
			{
				VirtueMacroRequest(e);
			}
		}

		/*
		public static void InvokeInscribeMenuRequest(InscribeMenuRequestEventArgs e)
		{
			if (InscribeMenuRequest != null)
			{
				InscribeMenuRequest(e);
			}
		}
		*/

		public static void InvokePlayerDeath(PlayerDeathEventArgs e)
		{
			if (PlayerDeath != null)
			{
				PlayerDeath(e);
			}
		}

		public static void InvokeCreatureDeath(CreatureDeathEventArgs e)
		{
			if (CreatureDeath != null)
			{
				CreatureDeath(e);
			}
		}

		public static void InvokeRenameRequest(RenameRequestEventArgs e)
		{
			if (RenameRequest != null)
			{
				RenameRequest(e);
			}
		}

		public static void InvokeLogout(LogoutEventArgs e)
		{
			if (Logout != null)
			{
				Logout(e);
			}
		}

		public static void InvokeSocketConnect(SocketConnectEventArgs e)
		{
			if (SocketConnect != null)
			{
				SocketConnect(e);
			}
		}

		public static void InvokeConnected(ConnectedEventArgs e)
		{
			if (Connected != null)
			{
				Connected(e);
			}
		}

		public static void InvokeDisconnected(DisconnectedEventArgs e)
		{
			if (Disconnected != null)
			{
				Disconnected(e);
			}
		}

		public static void InvokeAnimateRequest(AnimateRequestEventArgs e)
		{
			if (AnimateRequest != null)
			{
				AnimateRequest(e);
			}
		}

		public static void InvokeCastSpellRequest(CastSpellRequestEventArgs e)
		{
			if (CastSpellRequest != null)
			{
				CastSpellRequest(e);
			}
		}

		public static void InvokeOpenSpellbookRequest(OpenSpellbookRequestEventArgs e)
		{
			if (OpenSpellbookRequest != null)
			{
				OpenSpellbookRequest(e);
			}
		}

		public static void InvokeDisarmRequest(DisarmRequestEventArgs e)
		{
			if (DisarmRequest != null)
			{
				DisarmRequest(e);
			}
		}

		public static void InvokeStunRequest(StunRequestEventArgs e)
		{
			if (StunRequest != null)
			{
				StunRequest(e);
			}
		}

		public static void InvokeHelpRequest(HelpRequestEventArgs e)
		{
			if (HelpRequest != null)
			{
				HelpRequest(e);
			}
		}

		public static void InvokeShutdown(ShutdownEventArgs e)
		{
			if (Shutdown != null)
			{
				Shutdown(e);
			}
		}

		public static void InvokeCrashed(CrashedEventArgs e)
		{
			if (Crashed != null)
			{
				Crashed(e);
			}
		}

		public static void InvokeHungerChanged(HungerChangedEventArgs e)
		{
			if (HungerChanged != null)
			{
				HungerChanged(e);
			}
		}

		public static void InvokeMovement(MovementEventArgs e)
		{
			if (Movement != null)
			{
				Movement(e);
			}
		}

		public static void InvokeServerList(ServerListEventArgs e)
		{
			if (ServerList != null)
			{
				ServerList(e);
			}
		}

		public static void InvokeLogin(LoginEventArgs e)
		{
			if (Login != null)
			{
				Login(e);
			}
		}

		public static void InvokeSpeech(SpeechEventArgs e)
		{
			if (Speech != null)
			{
				Speech(e);
			}
		}

		public static void InvokeCharacterCreated(CharacterCreatedEventArgs e)
		{
			if (CharacterCreated != null)
			{
				CharacterCreated(e);
			}
		}

		public static bool InvokeValidatePlayerName(ValidatePlayerNameEventArgs e)
		{
			if (ValidatePlayerName != null)
			{
				return ValidatePlayerName(e);
			}

			return false;
		}

		public static void InvokeOpenDoorMacroUsed(OpenDoorMacroEventArgs e)
		{
			if (OpenDoorMacroUsed != null)
			{
				OpenDoorMacroUsed(e);
			}
		}

		public static void InvokeWorldLoad()
		{
			if (WorldLoad != null)
			{
				WorldLoad();
			}
		}

		public static void InvokeWorldSave(WorldSaveEventArgs e)
		{
			if (WorldSave != null)
			{
				WorldSave(e);
			}
		}

		public static void InvokeOnItemUse(OnItemUseEventArgs e)
		{
			if (OnItemUse != null)
			{
				OnItemUse(e);
			}
		}

		public static void InvokeOnEnterRegion(OnEnterRegionEventArgs e)
		{
			if (OnEnterRegion != null)
			{
				OnEnterRegion(e);
			}
		}

		public static void InvokeOnConsume(OnConsumeEventArgs e)
		{
			if (OnConsume != null)
			{
				OnConsume(e);
			}
		}

		public static void InvokeOnPropertyChanged(OnPropertyChangedEventArgs e)
		{
			if (OnPropertyChanged != null)
			{
				OnPropertyChanged(e);
			}
		}

		public static void Reset()
		{
			CharacterCreated = null;
			ValidatePlayerName = null;
			OpenDoorMacroUsed = null;
			Speech = null;
			Login = null;
			ServerList = null;
			Movement = null;
			HungerChanged = null;
			Crashed = null;
			Shutdown = null;
			HelpRequest = null;
			DisarmRequest = null;
			StunRequest = null;
			OpenSpellbookRequest = null;
			CastSpellRequest = null;
			AnimateRequest = null;
			Logout = null;
			SocketConnect = null;
			Connected = null;
			Disconnected = null;
			RenameRequest = null;
			PlayerDeath = null;
			VirtueGumpRequest = null;
			VirtueItemRequest = null;
			VirtueMacroRequest = null;
			//InscribeMenuRequest = null;
			ChatRequest = null;
			AccountLogin = null;
			PaperdollRequest = null;
			ProfileRequest = null;
			ChangeProfileRequest = null;
			AggressiveAction = null;
			Command = null;
			GameLogin = null;
			DeleteRequest = null;
			WorldLoad = null;
			WorldSave = null;
			SetAbility = null;
			GuildGumpRequest = null;
			QuestGumpRequest = null;
			OnItemUse = null;
			OnEnterRegion = null;
			OnConsume = null;
			OnPropertyChanged = null;
		}
	}
}